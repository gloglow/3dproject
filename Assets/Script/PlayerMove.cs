using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Targetting targetting;
    [SerializeField] private Weapon weapon;

    private Animator animator;
    [SerializeField] private GameObject bodyCollider;
    private Rigidbody rigidBody;
    private MeshRenderer[] meshRenderers;
    private SkinnedMeshRenderer[] meshRenderers2;

    //　基本情報
    public PlayerData playerData;
    public int crtHP;

    //　プレイヤー入力関連
    private float horizontal;
    private float vertical;
    private bool atkKeyDown;
    private bool jKeyDown;
    private bool chgTargetKeyDown;
    private bool telpoKeyDown;
    private bool dodgeKeyDown;

    //　移動関連
    [SerializeField] private float moveSpd;　//　移動速度
    private Vector3 moveVec;　
    private bool isBorder = false;　//　壁に接しているかどうか
    [SerializeField] private float rayForwardDistance;　//　壁を感知する距離

    //　ジャンプ関連
    [SerializeField] private float jPower;　//　ジャンプ距離
    [SerializeField] private bool onJ = false;　//　ジャンプしているかどうか

    //　攻撃関連
    public Transform target;
    public Ease ease;
    [SerializeField] private float telpoDistance;　//　瞬間移動距離
    [SerializeField] private float atkDelay;　//　クールタイム記録
    private bool isAtkReady;　//　攻撃可能であるかどうか
    private int atkPtn = 0;　//　攻撃パターン

    public bool isDodge = false;
    [SerializeField] private float dodgeDistance;
    [SerializeField] private float dodgeTime;
    
    //　攻撃されること関連
    public bool isDamage;
    [SerializeField] private float unavailableTime;　//　行動不能時間
    [SerializeField] private float godModeTime;　//　無敵状態時間

    //　スキル
    public float[] coolTimes = {1f, 2f}; // dodge, telpo
    public float[] crtCoolTimes = {0f, 0f};

    public UnityEvent stageFail;
    public UnityEvent<int> playerDamaged;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        meshRenderers2 = GetComponentsInChildren<SkinnedMeshRenderer>();

        playerData = DataManager.Instance.playerData;
        crtHP = playerData.HP;
    }

    private void Update()
    {
        //　ユーザー入力を感知
        GetInput();
        //　移動
        Move();
        //　ジャンプ
        Jump();
        target = targetting.GetTarget();
        //　ターゲットを変更
        ChangeTarget();
        Telpo();
        StartCoroutine(Dodge());
        //　攻撃
        Attack();

        UpdateCoolTime();
    }

    private void UpdateCoolTime()
    {
        for(int i=0; i<coolTimes.Length; i++)
        {
            if (crtCoolTimes[i] <= 0)
                crtCoolTimes[i] = 0;
            else
                crtCoolTimes[i] -= Time.deltaTime;
        }
    }

    private void GetInput()
    {
        //　マウス・キーボード入力を感知
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        jKeyDown = Input.GetKeyDown(KeyCode.X);
        chgTargetKeyDown = Input.GetKeyDown(KeyCode.LeftControl);
        telpoKeyDown = Input.GetKeyDown(KeyCode.S);
        dodgeKeyDown = Input.GetKeyDown(KeyCode.A);
        atkKeyDown = Input.GetButtonDown("Fire1");
    }
    
    private void Move()
    {
        if (isDodge)
            return;
        moveVec = new Vector3(horizontal, 0, vertical).normalized;
        
        //　‐　移動しない
        //　攻撃している時
        //　敵の攻撃を受けた瞬間
        if (!isAtkReady || isDamage)
            moveVec = Vector3.zero;

        //　‐　移動アニメーションはあるが、移動しない
        //　壁に接している時
        if(!isBorder)
            transform.position += moveVec * moveSpd * Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);

        //　移動方向に向かう
        transform.LookAt(transform.position + moveVec);
    }

    private void ChangeTarget()
    {
        if (chgTargetKeyDown && target != null)
        {
            target = targetting.ChangeTarget();
        }
    }

    private void Attack()
    {
        //　weapon.rate：攻撃のクールタイム
        //　クールタイム以上の時間が経ったか？
        atkDelay += Time.deltaTime;
        isAtkReady = weapon.rate < atkDelay;

        //　‐　攻撃する
        //　1.　攻撃入力があり、
        //　2.　攻撃可能な時間であり、
        //　3.　ジャンプ中・攻撃された時じゃなければ
        if(atkKeyDown && isAtkReady && !isDamage && !onJ)
        {
            
            if (target != null)
            {
                //　ターゲット側に向かう
                transform.LookAt(target.position);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            }

            ActualAttack();
        }
    }

    private void Telpo()
    {
        if (telpoKeyDown && target != null && crtCoolTimes[1] == 0)
        {
            //　ターゲットに向かって移動するための方向
            Vector3 dirVec = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            //　ターゲットまでの距離が瞬間移動の距離以上ならば、その分移動する
            if (distance > telpoDistance)
            {
                Vector3 destination = transform.position + dirVec * telpoDistance;
                transform.position = destination;
            }
            //　じゃなければ（近ければ）、武器で攻撃可能な位置に移動
            else
            {
                Vector3 destination = transform.position + dirVec * (distance - weapon.reach);
                transform.position = destination;
            }

            crtCoolTimes[1] = coolTimes[1];
        }
    }

    private IEnumerator Dodge()
    {
        if (dodgeKeyDown && crtCoolTimes[0] == 0 && moveVec != Vector3.zero)
        {
            isDodge = true;
            animator.SetBool("isDodge", true);
            Vector3 destination = transform.position + moveVec * dodgeDistance;
            transform.DOMove(destination, dodgeTime).SetEase(ease);
            OnGodMode();

            yield return new WaitForSeconds(dodgeTime);
            animator.SetBool("isDodge", false);
            isDodge = false;
            crtCoolTimes[0] = coolTimes[0];
            OffGodMode();
        }
    }

    private void ActualAttack()
    {
        //　持っている武器を使用
        weapon.Use();

        //　攻撃アニメーション
        animator.SetTrigger("doAtk");
        animator.SetInteger("atkPtn", atkPtn);

        //　攻撃クールタイムを初期化
        atkDelay = 0;

        //　攻撃パターンを更新
        atkPtn++;
        if (atkPtn > 2)
            atkPtn = 0;
    }

    private void Jump()
    {
        //　‐　ジャンプする
        //　ジャンプ入力があり、ジャンプ中じゃなければ
        if (jKeyDown && !onJ)
        {
            onJ = true;
            animator.SetTrigger("isJump");
        }
    }

    public void EndJump()
    {
        onJ = false;
    }

    private void FreezeRotation()
    {
        //　要らない物理作用を無効化
        rigidBody.angularVelocity = Vector3.zero;
    }

    private void OnWall()
    {
        //　壁に接しているかを確認
        isBorder = Physics.Raycast(transform.position, transform.forward, rayForwardDistance, LayerMask.GetMask("Wall"));
    }

    private void OnFloor()
    {
        //　床の下に落ちないようにする
        if(!onJ)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        //　要らない物理作用を無効化
        FreezeRotation();
        //　壁に接しているかを確認
        OnWall();
        //　床の下に落ちないようにする
        OnFloor();
    }

    private void OnTriggerEnter(Collider other)
    {
        //　敵に攻撃されたら
        if(other.tag == "EnemyAtk")
        {
            //　既に攻撃されて無敵状態になっていなければ
            if (!isDamage)
            {
                Attack atk = other.GetComponent<Attack>();
                crtHP -= atk.damage;

                if(crtHP > 0)
                {
                    //　攻撃される
                    StartCoroutine(OnDamage());
                }
                else
                {
                    Die();
                }
            }
        }
    }

    IEnumerator OnDamage()
    {
        //　攻撃され、行動不能になる
        isDamage = true;
        animator.SetTrigger("isDamage");
        DamagedEffect();
        playerDamaged?.Invoke(crtHP);

        yield return new WaitForSeconds(0.1f);

        //　攻撃され、ターゲッティングできない状態（無敵状態）に入る
        OnGodMode();

        yield return new WaitForSeconds(unavailableTime);

        //　行動不能を解除
        isDamage = false;

        yield return new WaitForSeconds(godModeTime);

        //　無敵状態を解除
        OffGodMode();
    }

    private void DamagedEffect()
    {
        //　キャラクターを赤くする
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = new Color(1f, 0f, 0f);
        }
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshRenderers2)
        {
            skinnedMeshRenderer.material.color = new Color(1f, 0f, 0f);
        }
    }

    private void OnGodMode()
    {
        //　キャラクターを暗くする（無敵状態）
        bodyCollider.gameObject.layer = 7;
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = new Color(0.5f, 0.5f, 0.5f);
        }
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshRenderers2)
        {
            skinnedMeshRenderer.material.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    private void OffGodMode()
    {
        //　無敵状態を解除
        bodyCollider.layer = 6;
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = new Color(1f, 1f, 1f);
        }
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshRenderers2)
        {
            skinnedMeshRenderer.material.color = new Color(1f, 1f, 1f);
        }
    }

    private void Die()
    {
        OnGodMode();
        animator.SetTrigger("isDead");
        stageFail?.Invoke();
    }
}