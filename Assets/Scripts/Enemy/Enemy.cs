using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BasicComponents
{
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private Animator animator;
    public Animator Animator => animator;

    private Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    public void Init(GameObject go)
    {
        spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
        animator = go.GetComponentInChildren<Animator>();
        rb = go.GetComponent<Rigidbody2D>();
    }
}

[System.Serializable]
public class BasicInfo
{
    public enum State
    {
        Idle,
        Move,
        Chase,
        Attack,
        Die
    }

    [SerializeField]
    private State currentState;
    public State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    [SerializeField]
    private float speed = 2f;
    public float Speed => speed;

    [SerializeField]
    private bool isMoving = true;
    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    [SerializeField]
    private float waitingTime = 2f;
    public float WaitingTime => waitingTime;

    [SerializeField]
    private float speedForTracing = 4f;
    public float SpeedForTracing => speedForTracing;

    [SerializeField]
    private float waitingAttack = 0.5f;
    public float WaitingAttack => waitingAttack;

    [SerializeField]
    private bool isWaitingAttack = false;
    public bool IsWaitingAttack
    {
        get => isWaitingAttack;
        set => isWaitingAttack = value;
    }

    [SerializeField]
    private bool isTracing = false;
    public bool IsTracing
    {
        get => isTracing;
        set
        {
            if (isTracing != value)
            {
                isTracing = value;
            }
        }
    }

    private Vector2 moveDirection;

    private Camera cam;

    private Color originColor;

    private Coroutine chargeCoroutine;

    [SerializeField]
    private float health = 100f;
    public float Health
    {
        get => health;
        set => health = value;
    }

    [SerializeField]
    private bool isDie = false;
    public bool IsDie
    {
        get => isDie;
        set => isDie = value;
    }

    public Vector2 MoveDirection
    {
        get => moveDirection;
        set => moveDirection = value;
    }

    public Camera Cam
    {
        get => cam;
        set => cam = value;
    }

    public Color OriginColor
    {
        get => originColor;
        set => originColor = value;
    }

    public Coroutine ChargeCoroutine
    {
        get => chargeCoroutine;
        set => chargeCoroutine = value;
    }

    [SerializeField]
    private bool isAttacked = false;
    public bool IsAttacked
    {
        get => isAttacked;
        set => isAttacked = value;
    }

    private bool isDieAnimationTriggered = false;
    public bool IsDieAnimationTriggered
    {
        get => isDieAnimationTriggered;
        set => isDieAnimationTriggered = value;
    }

    [SerializeField]
    private float damage = 10;
    public float Damage
    {
        get => damage;
        set => damage = value;
    }

    public AttackRange attackRange;

    private bool isWaiting = false;
    public bool IsWaiting
    {
        get => isWaiting;
        set => isWaiting = value;
    }
}

[System.Serializable]
public class Detection
{
    public PlayerMovement2D playerMovement2D;

    private Vector2 detectionCenterOriginalLocalPos;
    public Vector2 DetectionCenterOriginalLocalPos
    {
        get => detectionCenterOriginalLocalPos;
        set => detectionCenterOriginalLocalPos = value;
    }

    private Vector2 attackCenterOriginalLocalPos;
    public Vector2 AttackCenterOriginalLocalPos
    {
        get => attackCenterOriginalLocalPos;
        set => attackCenterOriginalLocalPos = value;
    }

    [SerializeField]
    private Transform pointA;
    public Transform PointA => pointA;

    [SerializeField]
    private Transform pointB;
    public Transform PointB => pointB;

    private Transform target;
    public Transform Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField]
    private float detectRadius = 3f;
    public float DetectRadius => detectRadius;

    [SerializeField]
    private LayerMask playerLayer;
    public LayerMask PlayerLayer => playerLayer;

    [SerializeField]
    private Transform playerPos;
    public Transform PlayerPos
    {
        get => playerPos;
        set => playerPos = value;
    }

    [SerializeField]
    private Transform detectionCenter;
    public Transform DetectionCenter => detectionCenter;

    [SerializeField]
    private Transform attackCenter;
    public Transform AttackCenter => attackCenter;

    [SerializeField]
    private float attackRadius = 2.5f;
    public float AttackRadius => attackRadius;

    [SerializeField]
    private bool isAttacking = false;
    public bool IsAttacking
    {
        get => isAttacking;
        set => isAttacking = value;
    }

    [SerializeField]
    private bool isTeleporting = false;
    public bool IsTeleporting
    {
        get => isTeleporting;
        set => isTeleporting = value;
    }

    [SerializeField]
    private bool inRange = false;
    public bool InRange
    {
        get => inRange;
        set => inRange = value;
    }

    [SerializeField]
    private bool inAttackRange = false;
    public bool InAttackRange
    {
        get => inAttackRange;
        set => inAttackRange = value;
    }

    [SerializeField]
    private Transform totalDetectionCenter;
    public Transform TotalDetectionCenter
    {
        get => totalDetectionCenter;
        set => totalDetectionCenter = value;
    }

    [SerializeField]
    private float totalDetectionRadius = 6.9f;
    public float TotalDetectionRadius
    {
        get => totalDetectionRadius;
        set => totalDetectionRadius = value;
    }

    [SerializeField]
    private bool inTotalDetectionRange = false;
    public bool InTotalDetectionRange
    {
        get => inTotalDetectionRange;
        set => inTotalDetectionRange = value;
    }
}

public class Enemy : MonoBehaviour
{
    [Header("BasicComponents")]
    [SerializeField]
    public BasicComponents basicComponents = new BasicComponents();

    [Header("BasicInfo")]
    [SerializeField]
    public BasicInfo basicInfo = new BasicInfo();

    [Header("Detection")]
    [SerializeField]
    public Detection detection = new Detection();

    private IEnemyState currentState;
    private IEnemyState idleState = new IdleState();
    private IEnemyState moveState = new MoveState();
    private IEnemyState chaseState = new ChaseState();
    private IEnemyState attackState = new AttackState();
    private IEnemyState dieState = new DieState();

    public IEnemyState IdleState => idleState;
    public IEnemyState MoveState => moveState;
    public IEnemyState ChaseState => chaseState;
    public IEnemyState AttackState => attackState;
    public IEnemyState DieState => dieState;

    void Start()
    {
        currentState = moveState;
        currentState.Enter(this);

        detection.InAttackRange = GetComponent<AttackRange>();

        detection.playerMovement2D = GameObject.FindWithTag("Player").GetComponent<PlayerMovement2D>();

        basicInfo.CurrentState = BasicInfo.State.Move;
        basicComponents.Init(gameObject);
        /*basicInfo.attackRange.GetComponent<AttackRange>().SetDamage(basicInfo.Damage);*/

        Vector2 detectionPos = detection.DetectionCenter.localPosition;
        detectionPos.x = Mathf.Abs(detectionPos.x);
        detection.DetectionCenterOriginalLocalPos = detectionPos;

        Vector2 attackPos = detection.AttackCenter.localPosition;
        attackPos.x = Mathf.Abs(attackPos.x);
        detection.AttackCenterOriginalLocalPos = attackPos;

        if (detection == null)
            return;
        detection.Target = detection.PointB;

        basicComponents.Animator.SetBool("isMoving", basicInfo.IsMoving);
        detection.PlayerPos = GameObject.FindWithTag("Player")?.transform;

        basicComponents.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }
    void Update()
    {
        currentState?.Update();
        //Debug.Log("Current State : " + currentState.ToString());
    }

    void FixedUpdate()
    {
        detection.InRange = CheckPlayerInRange(detection.DetectionCenter.position, detection.DetectRadius, detection.PlayerLayer);
        detection.InAttackRange = CheckPlayerInRange(detection.AttackCenter.position, detection.AttackRadius, detection.PlayerLayer);
        detection.InTotalDetectionRange = CheckPlayerInRange(detection.TotalDetectionCenter.position, detection.TotalDetectionRadius, detection.PlayerLayer);

        if (basicInfo.IsMoving && !detection.IsTeleporting)
        {
            if (basicInfo.IsTracing)
                basicComponents.Rigidbody.MovePosition(basicComponents.Rigidbody.position + basicInfo.MoveDirection * basicInfo.SpeedForTracing * Time.fixedDeltaTime);
            else
                basicComponents.Rigidbody.MovePosition(basicComponents.Rigidbody.position + basicInfo.MoveDirection * basicInfo.Speed * Time.fixedDeltaTime);
        }
    }

    bool CheckPlayerInRange(Vector2 origin, float radius, LayerMask layer)
    {
        return Physics2D.OverlapCircle(origin, radius, layer) != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (detection.DetectionCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detection.DetectionCenter.position, detection.DetectRadius);
        }

        if (detection.AttackCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detection.AttackCenter.position, detection.AttackRadius);
        }

        if(detection.TotalDetectionCenter != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(detection.TotalDetectionCenter.position, detection.TotalDetectionRadius);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (basicInfo.IsDie)
            return;

        basicInfo.Health -= dmg;
        basicInfo.IsAttacked = true;
        basicInfo.IsTracing = true;

        if (basicInfo.Health <= 0)
        {
            basicInfo.MoveDirection = Vector2.zero;
            ChangeState(DieState);
            return;
        }

        basicInfo.CurrentState = BasicInfo.State.Chase;
    }

    public void ResetFlagsForIdle()
    {
        basicInfo.IsMoving = false;
        basicInfo.IsWaitingAttack = false;
        basicInfo.IsTracing = false;
        basicInfo.IsDie = true;
        basicInfo.IsAttacked = false;
        basicInfo.IsDieAnimationTriggered = false;
        detection.IsAttacking = false;
        detection.IsTeleporting = false;
    }

    public void ResetFlagsForMove()
    {
        basicInfo.IsMoving = true;
        basicInfo.IsWaitingAttack = false;
        basicInfo.IsTracing = false;
        basicInfo.IsDie = false;
        basicInfo.IsAttacked = false;
        basicInfo.IsDieAnimationTriggered = false;
        detection.IsAttacking = false;
        detection.IsTeleporting = false;
    }

    public void ResetFlagsForChase()
    {
        basicInfo.IsMoving = true;
        basicInfo.IsWaitingAttack = false;
        basicInfo.IsTracing = true;
        basicInfo.IsDie = false;
        basicInfo.IsAttacked = false;
        basicInfo.IsDieAnimationTriggered = false;
        detection.IsAttacking = false;
        detection.IsTeleporting = false;
    }

    public void ResetFlagsForAttack()
    {
        basicInfo.IsMoving = false;
        basicInfo.IsWaitingAttack = false;
        basicInfo.IsTracing = true;
        basicInfo.IsDie = false;
        basicInfo.IsAttacked = false;
        basicInfo.IsDieAnimationTriggered = false;
        detection.IsAttacking = true;
        detection.IsTeleporting = false;
    }

    public void ResetFlagsForDie()
    {
        basicInfo.IsMoving = false;
        basicInfo.IsWaitingAttack = false;
        basicInfo.IsTracing = false;
        basicInfo.IsDie = true;
        basicInfo.IsAttacked = false;
        basicInfo.IsDieAnimationTriggered = false;
        detection.IsAttacking = false;
        detection.IsTeleporting = false;
    }

    public void ChangeState(IEnemyState newState)
    {
        // 기존 상태가 있으면 Exit() 호출
        currentState?.Exit();

        // 상태 변경
        currentState = newState;

        // 새 상태의 Enter() 호출
        currentState.Enter(this);
    }

    public bool CanMove()
    {
        return !detection.InRange
            && !basicInfo.IsDie
            && !basicInfo.IsTracing
            && !basicInfo.IsWaitingAttack
            && !detection.IsAttacking;
    }

    // InRange 내부에 있을 경우
    public bool CanChase()
    {
        bool common = !detection.InAttackRange
            && detection.PlayerPos != null
            && !basicInfo.IsDie
            && !basicInfo.IsWaitingAttack
            && !detection.IsAttacking
            && !detection.playerMovement2D.isDie;

        bool first = detection.InRange;
        bool second = detection.InTotalDetectionRange && basicInfo.IsTracing;
        return common & (first || second);
    }

    public bool CanAttack()
    {
        return detection.InAttackRange 
            && detection.PlayerPos != null 
            && !basicInfo.IsDie 
            && basicInfo.IsTracing
            && !detection.playerMovement2D.isDie;
    }

    public void RunCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    
}