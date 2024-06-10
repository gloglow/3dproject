using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Slime : Monster
{
    //　攻撃パターン
    //　1.　一般攻撃（物理、近接）：3回
    //　2.　突進攻撃（物理、近接）：1回
    //　→　繰り返し

    [SerializeField] Vector2[] targettingInfo;　//　攻撃に使うcastのradius、攻撃に使うcastの長さ
    Attack_Melee atkMelee;
    private int atkPtnCnt = 0;

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
        atkPtnCnt = atkPtnCnt <= 3 ? atkPtnCnt + 1 : 0;
        crtAtkPtn = atkPtnCnt < 3 ? 0 : 1;
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
        atkMelee = crtAtk.GetComponent<Attack_Melee>();
        atkMelee.Attack();
    }

    public override void EndHit()
    {
        atkMelee.EndAttack();
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
}
