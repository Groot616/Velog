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
}

[System.Serializable]
public class Detection
{
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

    private Transform player;
    public Transform Player
    {
        get => player;
        set => player = value;
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
    private BasicComponents basicComponents = new BasicComponents();

    [Header("BasicInfo")]
    [SerializeField]
    private BasicInfo basicInfo = new BasicInfo();

    [Header("Detection")]
    [SerializeField]
    private Detection detection = new Detection();

    void Start()
    {
        basicInfo.CurrentState = BasicInfo.State.Move;
        basicComponents.Init(gameObject);

        Vector2 detectionPos = detection.DetectionCenter.localPosition;
        detectionPos.x = Mathf.Abs(detectionPos.x);
        //detectionPos.y = Mathf.Abs(detectionPos.y);
        detection.DetectionCenterOriginalLocalPos = detectionPos;

        Vector2 attackPos = detection.AttackCenter.localPosition;
        attackPos.x = Mathf.Abs(attackPos.x);
        //attackPos.y = Mathf.Abs(attackPos.y);
        detection.AttackCenterOriginalLocalPos = attackPos;

        if (detection == null)
            return;
        detection.Target = detection.PointB;

        basicComponents.Animator.SetBool("isMoving", basicInfo.IsMoving);
        detection.Player = GameObject.FindWithTag("Player")?.transform;

        basicComponents.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }
    void Update()
    {
        FlipDetectionCenterAccordingToDetection();
        detection.InAttackRange = CheckPlayerInRange(detection.AttackCenter.position, detection.AttackRadius, detection.PlayerLayer);

        if (basicInfo.IsAttacked && !detection.InTotalDetectionRange)
            basicInfo.IsAttacked = false;

        switch (basicInfo.CurrentState)
        {
            case BasicInfo.State.Idle:
                HandleIdle();
                break;
            case BasicInfo.State.Move:
                HandleMove();
                break;
            case BasicInfo.State.Chase:
                HandleChase();
                break;
            case BasicInfo.State.Attack:
                HandleAttack();
                break;
            case BasicInfo.State.Die:
                HandleDie();
                break;
        }

        if (!detection.InTotalDetectionRange)
            basicInfo.IsTracing = false;
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

    void HandleIdle()
    {
        basicInfo.MoveDirection = Vector2.zero;

        // player가 공격 범위 내 존재할 경우 attack state 전환
        if (detection.InAttackRange && detection.Player != null && !basicInfo.IsDie && basicInfo.IsTracing)
        {
            basicInfo.CurrentState = BasicInfo.State.Attack;
            return;
        }
        // player 발견시 chase state 전환
        else if (detection.InRange && detection.Player != null && !basicInfo.IsDie && basicInfo.IsTracing)
        {
            basicInfo.CurrentState = BasicInfo.State.Chase;
            return;
        }

        // TODO: 조건 확인해서 FSM 완성
        //else if (basicInfo.IsAttacked)

        //if (detection.InRange && detection.Player != null && !basicInfo.IsDie)
        //{
        //    basicInfo.CurrentState = BasicInfo.State.Chase;
        //}
        //if (detection.InAttackRange && detection.Player != null && !basicInfo.IsDie)
        //{
        //    basicInfo.CurrentState = BasicInfo.State.Attack;
        //}
    }

    void HandleMove()
    {
        if (!basicInfo.IsTracing && !basicInfo.IsDie)
            Patrol();

        if (detection.InRange || (detection.InTotalDetectionRange && basicInfo.IsAttacked && basicInfo.IsTracing))
        {
            basicInfo.CurrentState = BasicInfo.State.Chase;
            return;
        }
    }

    void HandleChase()
    {
        basicInfo.IsTracing = true;

        if (detection.InAttackRange && detection.Player != null)
        {
            basicInfo.IsMoving = false;
            basicInfo.CurrentState = BasicInfo.State.Attack;
            return;
        }
        else if (detection.InRange || (detection.InTotalDetectionRange && basicInfo.IsTracing))
            MoveTowardsPlayer();
    }

    void HandleAttack()
    {
        basicInfo.IsTracing = true;
        if (detection.IsAttacking || basicInfo.IsWaitingAttack || basicInfo.IsDie)
            return;

        if(basicInfo.Health <= 0)
        {
            basicInfo.CurrentState = BasicInfo.State.Die;
            return;
        }

        if (detection.InAttackRange && detection.Player != null)
        {
            basicComponents.Animator.SetBool("isMoving", false);
            basicComponents.Animator.SetBool("isAttacking", false);
            Attack();
        }
        else if(detection.InTotalDetectionRange && detection.Player != null && basicInfo.IsTracing)
        {
            basicInfo.CurrentState = BasicInfo.State.Chase;
            return;
        }
        else
        {
            basicInfo.CurrentState = BasicInfo.State.Move;
            return;
        }
    }

    void HandleDie()
    {
        Die();
    }

    void FlipDetectionCenterAccordingToDetection()
    {
        if (detection.DetectionCenter == null || detection.AttackCenter == null)
            return;

        Vector2 detectionOriginalPos = detection.DetectionCenterOriginalLocalPos;
        Vector2 attackOriginalPos = detection.AttackCenterOriginalLocalPos;
        bool facingRight = basicComponents.SpriteRenderer.flipX;
        if (facingRight)
        {
            // 오른쪽 바라볼 때는 x를 양수로 유지, y는 그대로
            detection.DetectionCenter.localPosition = new Vector2(Mathf.Abs(detectionOriginalPos.x), detectionOriginalPos.y);
            detection.AttackCenter.localPosition = new Vector2(Mathf.Abs(attackOriginalPos.x), attackOriginalPos.y);
        }
        else
        {
            // 왼쪽 바라볼 때는 x를 음수로 뒤집고, y는 그대로
            detection.DetectionCenter.localPosition = new Vector2(-Mathf.Abs(detectionOriginalPos.x), detectionOriginalPos.y);
            detection.AttackCenter.localPosition = new Vector2(-Mathf.Abs(attackOriginalPos.x), attackOriginalPos.y);
        }
    }

    void Patrol()
    {
        Vector2 targetPos = new Vector2(detection.Target.position.x, transform.position.y);
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        basicInfo.MoveDirection = direction;
        basicComponents.SpriteRenderer.flipX = targetPos.x > transform.position.x;

        //// 코드 추가
        //Vector2 localPosDetection = detection.DetectionCenter.localPosition;
        //localPosDetection.x = Mathf.Abs(localPosDetection.x) * (basicComponents.SpriteRenderer.flipX ? 1 : -1);
        //detection.DetectionCenter.localPosition = localPosDetection;

        //Vector2 localPosAttack = detection.AttackCenter.localPosition;
        //localPosAttack.x = Mathf.Abs(localPosAttack.x) * (basicComponents.SpriteRenderer.flipX ? 1 : -1);
        //detection.AttackCenter.localPosition = localPosAttack;
        ////
        FlipDetectionCenterAccordingToDetection();

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            basicInfo.MoveDirection = Vector2.zero;
            StartCoroutine(Wait());
            detection.Target = (detection.Target == detection.PointA) ? detection.PointB : detection.PointA;
        }
    }

    private IEnumerator Wait()
    {
        basicInfo.CurrentState = BasicInfo.State.Idle;
        basicInfo.IsMoving = false;
        basicComponents.Animator.SetBool("isMoving", basicInfo.IsMoving);
        yield return new WaitForSeconds(basicInfo.WaitingTime);

        basicInfo.IsMoving = true;
        basicComponents.Animator.SetBool("isMoving", basicInfo.IsMoving);
        basicInfo.CurrentState = BasicInfo.State.Move;
    }

    bool CheckPlayerInRange(Vector2 origin, float radius, LayerMask layer)
    {
        return Physics2D.OverlapCircle(origin, radius, layer) != null;
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = new Vector2(detection.Player.position.x - transform.position.x, 0f).normalized;
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

        Vector2 attackPoint = new Vector2(detection.Player.position.x, transform.position.y);
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

        yield return new WaitForSeconds(1f);
        transform.position = returnPos;

        yield return new WaitForSeconds(1.7f);
        basicComponents.Animator.SetBool("isAttacking", detection.IsAttacking);
        detection.IsAttacking = false;
        detection.IsTeleporting = false;

        if (detection.InAttackRange && detection.Player != null && !basicInfo.IsDie)
        {
            basicInfo.CurrentState = BasicInfo.State.Attack;
            basicInfo.IsMoving = false;
            basicComponents.Animator.SetBool("isMoving", false);
            basicComponents.Animator.SetBool("isAttacking", true);
        }
        else
        {
            if (detection.InTotalDetectionRange && detection.Player != null)
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

    public void StartChargeEffect()
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
    }
}