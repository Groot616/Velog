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

    /*public AttackRange attackRange;*/

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
        currentState?.Update(this);
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

    /*void HandleChase()
    {
        if (!detection.playerMovement2D.isDie)
        {
            basicInfo.IsTracing = true;
        
            if (detection.InAttackRange && detection.PlayerPos != null)
            {
                basicInfo.IsMoving = false;
                basicInfo.CurrentState = BasicInfo.State.Attack;
                return;
            }
            else if (detection.InRange || (detection.InTotalDetectionRange && basicInfo.IsTracing))
                MoveTowardsPlayer();
        }
    }

    void HandleAttack()
    {
        if (detection.playerMovement2D.isDie)
        {
            // 플레이어 죽었으니 공격 중단하고 이동 상태로 전환
            basicInfo.CurrentState = BasicInfo.State.Move;
            basicInfo.IsTracing = false;
            basicInfo.IsMoving = true;
            basicComponents.Animator.SetBool("isMoving", true);
            basicComponents.Animator.SetBool("isAttacking", false);
            detection.IsAttacking = false;
            return;
        }
        else
        {
            basicInfo.IsTracing = true;
            if (detection.IsAttacking || basicInfo.IsWaitingAttack || basicInfo.IsDie)
                return;

            if (basicInfo.Health <= 0)
            {
                basicInfo.CurrentState = BasicInfo.State.Die;
                return;
            }

            if (detection.InAttackRange && detection.PlayerPos != null)
            {
                if (!detection.playerMovement2D.isDie)
                {
                    basicComponents.Animator.SetBool("isMoving", false);
                    basicComponents.Animator.SetBool("isAttacking", false);
                    Attack();
                }
            }
            else if (detection.InTotalDetectionRange && detection.PlayerPos != null && basicInfo.IsTracing)
            {
                if (!detection.playerMovement2D.isDie)
                {
                    basicInfo.CurrentState = BasicInfo.State.Chase;
                    return;
                }
            }
            else
            {
                basicInfo.CurrentState = BasicInfo.State.Move;
                return;
            }
        }
    }

    void HandleDie()
    {
        Die();
    }
    */

    bool CheckPlayerInRange(Vector2 origin, float radius, LayerMask layer)
    {
        return Physics2D.OverlapCircle(origin, radius, layer) != null;
    }

    /*void MoveTowardsPlayer()
    {
        Vector2 direction = new Vector2(detection.PlayerPos.position.x - transform.position.x, 0f).normalized;
        basicInfo.MoveDirection = direction;
        basicInfo.IsTracing = true;
        basicInfo.IsMoving = true;
        basicComponents.Animator.SetBool("isMoving", basicInfo.IsMoving);
        basicComponents.SpriteRenderer.flipX = direction.x > 0;

        if(detection.InAttackRange)
        {
            basicInfo.CurrentState = BasicInfo.State.Attack;
        }
    }

    void Attack()
    {
        basicInfo.IsTracing = false;
        basicInfo.IsMoving = false;
        basicComponents.Animator.SetBool("isMoving", basicInfo.IsMoving);
        detection.IsAttacking = true;
        basicComponents.Animator.SetBool("isAttacking", detection.IsAttacking);

        Vector2 attackPoint = new Vector2(detection.PlayerPos.position.x, transform.position.y);
        Vector2 returnPoint = transform.position;
        if (!detection.IsTeleporting)
        {
            StartCoroutine(TeleportAndShake(attackPoint, returnPoint));
            StartChargeEffect();
        }
    }

    private IEnumerator TeleportAndShake(Vector2 targetPos, Vector2 returnPos)
    {
        detection.IsTeleporting = true;
        yield return new WaitForSeconds(basicInfo.WaitingAttack);
        transform.position = targetPos;
        StartCoroutine(CameraShake(0.3f, 0.1f));
        // 추가 코드
        if (basicInfo.attackRange != null)
        {
            basicInfo.attackRange.GetComponent<AttackRange>().EnableAttackerCollider();
        }
        yield return new WaitForSeconds(0.1f);
        // 추가 코드
        // 추가 코드
        if (basicInfo.attackRange != null)
            basicInfo.attackRange.GetComponent<AttackRange>().DisableAttackerCollider();
        // 추가 코드

        yield return new WaitForSeconds(1f);
        
        transform.position = returnPos;

        yield return new WaitForSeconds(1.7f);
        basicComponents.Animator.SetBool("isAttacking", detection.IsAttacking);
        detection.IsAttacking = false;
        detection.IsTeleporting = false;

        if (detection.InAttackRange && detection.PlayerPos != null && !basicInfo.IsDie)
        {
            basicInfo.CurrentState = BasicInfo.State.Attack;
            basicInfo.IsMoving = false;
            basicComponents.Animator.SetBool("isMoving", false);
            basicComponents.Animator.SetBool("isAttacking", true);
        }
        else
        {
            if (detection.InTotalDetectionRange && detection.PlayerPos != null)
            {
                basicInfo.CurrentState = BasicInfo.State.Chase;
                basicInfo.IsTracing = true;
                basicComponents.Animator.SetBool("isMoving", true);
                basicComponents.Animator.SetBool("isAttacking", false);
            }
            else
            {
                basicInfo.CurrentState = BasicInfo.State.Move;
                basicInfo.IsMoving = true;
                basicComponents.Animator.SetBool("isMoving", true);
                basicComponents.Animator.SetBool("isAttacking", false);
            }
            }
        }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        basicInfo.Cam = Camera.main;
        if (basicInfo.Cam == null) yield break;

        Vector3 OriginPos = basicInfo.Cam.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            basicInfo.Cam.transform.position = new Vector3(OriginPos.x + x, OriginPos.y + y, OriginPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        basicInfo.Cam.transform.position = OriginPos;
    }*/

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

    /*public void StartChargeEffect()
    {
        if (basicInfo.ChargeCoroutine != null)
            StopCoroutine(basicInfo.ChargeCoroutine);
        basicInfo.ChargeCoroutine = StartCoroutine(ChargeRedEffect());

    }

    private IEnumerator ChargeRedEffect()
    {
        basicInfo.OriginColor = basicComponents.SpriteRenderer.color;
        Color targetColor = new Color(1f, 0.4f, 0.4f);

        float elapsed = 0f;
        while (elapsed < basicInfo.WaitingAttack)
        {
            float t = elapsed / basicInfo.WaitingAttack;
            basicComponents.SpriteRenderer.color = Color.Lerp(basicInfo.OriginColor, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        basicComponents.SpriteRenderer.color = basicInfo.OriginColor;
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
            basicInfo.CurrentState = BasicInfo.State.Die;
            return;
        }

        basicInfo.CurrentState = BasicInfo.State.Chase;
    }

    public void Die()
    {
        if (basicInfo.IsDieAnimationTriggered)
            return;

        basicInfo.IsDieAnimationTriggered = true;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        basicComponents.Animator.SetTrigger("Die");

        StartCoroutine(DeathTeloportSequence());
    }

        private IEnumerator DeathTeloportSequence()
    {
        Vector2 OriginPos = transform.position;

        int repeatTwice = 0;
        while (repeatTwice < 7)
        {
            transform.position = OriginPos + new Vector2(-0.05f, 0f);
            yield return new WaitForSeconds(0.05f);

            transform.position = OriginPos + new Vector2(0.05f, 0f);
            yield return new WaitForSeconds(0.05f);
            ++repeatTwice;
        }
        transform.position = OriginPos;

        basicInfo.IsDie = true;
        basicInfo.IsMoving = false;
        basicInfo.IsWaitingAttack = false;
        basicInfo.IsTracing = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }*/

    public void ResetFlagsForIdle()
    {
        basicInfo.IsMoving = false;
        basicInfo.IsWaitingAttack = false;
        basicInfo.IsTracing = false;
        basicInfo.IsDie = false;
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
        detection.IsTeleporting = true;
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
        currentState?.Exit(this);

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

    public bool CanChase()
    {
        return detection.InRange
            && detection.PlayerPos != null
            && !basicInfo.IsDie
            && basicInfo.IsTracing
            && !basicInfo.IsWaitingAttack
            && !detection.IsAttacking
            && !detection.playerMovement2D.isDie;
    }

    public bool CanAttack()
    {
        return detection.InAttackRange 
            && detection.PlayerPos != null 
            && !basicInfo.IsDie 
            && basicInfo.IsTracing
            && !basicInfo.IsWaitingAttack
            && !detection.IsAttacking
            && !detection.playerMovement2D.isDie;
    }

    /*public bool CanDie()
    {

    }*/

    public void RunCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    
}