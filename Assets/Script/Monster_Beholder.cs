using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Beholder : Monster
{
    //　攻撃パターン
    //　1.　一般攻撃（物理、近接）：2回
    //  2.　突進攻撃（物理、近接）：1回
    //　3.　毒弾発射（魔法、弾丸）：1回
    //　ターゲットとの距離が近い　→　1&2
    //　ターゲットとの距離が遠い　→　3

    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private int bulletCnt;
    [SerializeField] private Transform bulletParent;

    [SerializeField] private float atkPtnDistance;
    private float targetDistance;
    [SerializeField] Vector2[] targettingInfo;　//　攻撃に使うcastのradius、攻撃に使うcastの長さ
    Attack_Melee atkMelee;
    private int atkPtnCnt = 0;
    [SerializeField] private float bulletGap;

    Attack_Bullet[] bulletArr;

    Coroutine myCoroutine;

    protected override void Initialize()
    {
        base.Initialize();
        bulletArr = new Attack_Bullet[bulletCnt];
        for(int i = 0; i < bulletCnt; i++)
        {
            Attack_Bullet bullet = Instantiate(prefabBullet).GetComponent<Attack_Bullet>();
            bulletArr[i] = bullet;
            bullet.gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        //　ナビゲーションコンポーネントが活性化されている時だけ移動
        if (navMeshAgent.enabled && target != null)
        {
            navMeshAgent.SetDestination(target.position);
            navMeshAgent.isStopped = !isMove;
            animator.SetBool("isWalk", isMove);
        }
    }

    protected override void DecideAtkPtn()
    {
        //　攻撃パターンを決定
        targetDistance = Vector3.Distance(transform.position, target.position);

        //　ターゲットとの距離が近ければ
        if(targetDistance < atkPtnDistance)
        {
            crtAtkPtn = atkPtnCnt < 2 ? 0 : 1;
        }
        //　遠ければ
        else
        {
            crtAtkPtn = 2;
        }
        crtAtk = monsterAtkPtns[crtAtkPtn];
    }

    protected override void Targetting()
    {
        //　‐　攻撃する
        //　前側に球を投げ、その範囲にプレイヤーが入り、
        //　攻撃していなければ
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, targettingInfo[crtAtkPtn].x, transform.forward, targettingInfo[crtAtkPtn].y, LayerMask.GetMask("Player"));
        if (raycastHits.Length > 0 && !isAtk && crtHP > 0)
        {
            //　移動を止め、攻撃を開始
            isAtk = true;
            isMove = false;
            transform.LookAt(raycastHits[0].transform);

            StartAttack();
        }
    }

    protected override void StartAttack()
    {
        //　アニメーションセッティング
        animator.SetTrigger("isAtk");
        animator.SetInteger("atkPtn", crtAtkPtn);
    }

    public override void StartHit()
    {
        //　攻撃
        switch (crtAtk.weapon)
        {
            case Attack.Weapon.Melee:
                atkMelee = crtAtk.GetComponent<Attack_Melee>();
                atkMelee.Attack();
                break;
            case Attack.Weapon.Bullet:
                myCoroutine = StartCoroutine(ShootBullet());
                break;
        }
    }

    public override void EndHit()
    {
        switch (crtAtk.weapon)
        {
            case Attack.Weapon.Melee:
                atkMelee.EndAttack();
                break;
            case Attack.Weapon.Bullet:
                StopCoroutine(myCoroutine);
                break;
        }
        Invoke("EndAttack", afterAtkDelay);
    }

    protected override void EndAttack()
    {
        //　攻撃パターンを決定
        DecideAtkPtn();

        //　攻撃終了
        isMove = true;
        isAtk = false;
    }

    private IEnumerator ShootBullet()
    {
        Vector3 dirVec = target.position - bulletParent.position;
        dirVec = new Vector3(dirVec.x, 0, dirVec.z);
        dirVec.Normalize();
        for (int i = 0; i < bulletArr.Length; i++)
        {
            bulletArr[i].transform.position = bulletParent.position;
            bulletArr[i].Shoot(dirVec);
            yield return new WaitForSeconds(bulletGap);
        }
    }
}
