using System.Collections;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator animator;
    private Rigidbody2D rb;
    private AttackRange attackRg;
    private float moveInput;

    private bool isMoving = false;
    public bool isAttacking = false;

    [SerializeField]
    private float jumpSpeed = 5f;
    public bool isJumping = false;

    [SerializeField]
    private float health = 100f;
    [SerializeField]
    private float damage = 10f;
    [SerializeField]
    private GameObject attackRange;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackRg = GetComponentInChildren<AttackRange>();
        attackRg.SetDamage(damage);

        if (attackRange != null)
            attackRange.SetActive(false);

        Debug.Log("Collision occured? : " + attackRange.activeSelf);
    }

    void Update()
    {
        if (!isAttacking)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            moveInput = 0f;
        }
        isMoving = (moveInput != 0);
        if (isMoving)
        {
            spriteRenderer.flipX = moveInput < 0;
        }
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.C) && !isAttacking)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking && !isJumping)
        {
            Jump();
        }

        if (!isJumping && rb.linearVelocity.y < -0.1f)
        {
            isJumping = true;
            animator.SetBool("isJumping", isJumping);
            animator.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetTrigger("Attack");

        if (attackRange != null)
        {
            attackRange.SetActive(true);
        }

        Invoke(nameof(DisableAttackRange), 1f);
    }

    private void DisableAttackRange()
    {
        if (attackRange != null)
            attackRange.SetActive(false);
        isAttacking = false;
    }

    private void Jump()
    {
        if (isJumping) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        isJumping = true;
        animator.SetBool("isJumping", isJumping);
        animator.SetTrigger("Jump");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
    }
}