using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class Monster : MonoBehaviour
{
    protected Targetting targetting;
    protected Rigidbody rigidBody;
    protected NavMeshAgent navMeshAgent;　//　ターゲットを追跡するためのコンポーネント
    protected Animator animator;
    protected MeshRenderer[] meshRenderers;
    protected SkinnedMeshRenderer[] meshRenderers2;

    public int id;
    public int level;
    public Transform center;
    public int defaultLayer;
    public int nonTargettedLayer;

    [SerializeField] protected HpBar hpBar;
    //　初期HPと現実HP
    [SerializeField] protected int maxHP;
    [SerializeField] protected int crtHP;

    //　移動関連
    protected bool isMove;　//　移動するかどうかを決定
    [SerializeField] protected float initialWaitTime;　//　生成され、移動開始するまでのギャップ

    //　攻撃関連
    public Attack[] monsterAtkPtns;
    protected Attack crtAtk;
    protected bool isAtk;　//　攻撃しているかどうか
    protected Transform target;　//　攻撃ターゲット
    public string targetTag;
    public int crtAtkPtn = 0;
    public float afterAtkDelay;　//　攻撃後のディレイ（次の攻撃までの待機時間)

    //　攻撃を受けること関連
    [SerializeField] protected float reactDistance;　//　死ぬとき反動する距離
    [SerializeField] protected float dieDelay;　//　死体が残る時間

    public UnityEvent<Monster> monsterDead;

    protected void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        meshRenderers2 = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    protected void Start()
    {
        Initialize();
    }

    virtual protected void Initialize()
    {
        crtHP = maxHP;
        hpBar.Initialize(maxHP);
        crtAtk = monsterAtkPtns[crtAtkPtn];
        Invoke("MoveStart", initialWaitTime);
    }

    public void DecideTarget()
    {
        target = GameObject.FindWithTag("Player").transform; //　プレイヤーを探し、ターゲットにする
    }

    protected void MoveStart()
    {
        //　移動開始
        isMove = true;
        animator.SetBool("isWalk", true);
    }

    protected void FixedUpdate()
    {
        if(target != null)
            Targetting();
        //　要らない物理作用を無効化
        FreezeVelocity();
    }

    protected void FreezeVelocity()
    {
        //　要らない物理作用を無効化
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
    }

    abstract protected void StartAttack();

    abstract protected void DecideAtkPtn();

    abstract public void StartHit();

    abstract public void EndHit();

    abstract protected void EndAttack();

    virtual protected void Targetting()
    {

    }

    public void Damaged(int hpChg, Vector3 reactVec)
    {
        crtHP += hpChg;
        hpBar.ChangeValue(crtHP);

        //　まだ生きている
        if (crtHP > 0)
        {
            StartCoroutine(DamagedEffect());
        }
        //　死んだ
        else
        {
            StartCoroutine(Die(reactVec));
        }
    }

    protected IEnumerator DamagedEffect()
    {
        //　体を一瞬赤くする
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = new Color(1f, 0f, 0f);
        }
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshRenderers2)
        {
            skinnedMeshRenderer.material.color = new Color(1f, 0f, 0f);
        }

        yield return new WaitForSeconds(0.1f);

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = new Color(1f, 1f, 1f);
        }
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshRenderers2)
        {
            skinnedMeshRenderer.material.color = new Color(1f, 1f, 1f);
        }
    }

    private IEnumerator Die(Vector3 reactVec)
    {
        //　ターゲッティングできなくする
        gameObject.layer = nonTargettedLayer;

        //　移動・攻撃終了
        navMeshAgent.enabled = false;

        animator.SetTrigger("isDead");

        //　攻撃された反対方向に反動
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;
        rigidBody.AddForce(reactVec * reactDistance, ForceMode.Impulse);

        monsterDead?.Invoke(this);

        yield return new WaitForSeconds(dieDelay);
        gameObject.SetActive(false);
    }
}
