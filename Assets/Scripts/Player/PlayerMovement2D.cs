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

    [SerializeField]
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

    public bool isDie = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackRg = GetComponentInChildren<AttackRange>();
        attackRg.SetDamage(damage);

        //if (attackRange != null)
        //    attackRange.SetActive(false);

        Debug.Log("Collision occured? : " + attackRange.activeSelf);
    }

    void Update()
    {
        if (!isDie)
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
            //if (isMoving)
            //{
            //    spriteRenderer.flipX = moveInput < 0;
            //}
            if (isMoving)
            {
                bool newFlipX = moveInput < 0;
                if (spriteRenderer.flipX != newFlipX)
                {
                    spriteRenderer.flipX = newFlipX;

                    // AttackRange를 Scale로 반전 (Position 만지지 마라)
                    Vector3 attackScale = attackRange.transform.localScale;
                    attackScale.x = spriteRenderer.flipX ? -1 : 1;
                    attackRange.transform.localScale = attackScale;
                }
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
    }

    void FixedUpdate()
    {
        if (!isAttacking && !isDie)
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
            //attackRange.SetActive(true);
            attackRange.GetComponent<AttackRange>().EnableAttackerCollider();
        }

        Invoke(nameof(DisableAttackRange), 1f);
    }

    private void DisableAttackRange()
    {
        if (attackRange != null)
        {
            // attackRange.SetActive(false);
            attackRange.GetComponent<AttackRange>().DisableAttackerCollider();
        }
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

    public void TakeDamage(float dmg)
    {
        Debug.Log("Player's TakeDamge() is called!");
        if (isDie)
            return;

        health -= dmg;
        if (health <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            Die();
        }
    }

    private void Die()
    {
        isDie = true;
        isMoving = false;
        animator.SetBool("isDie", true);
        animator.SetBool("isMoving", false);
        StartCoroutine(PlayDieAnimation());
    }
    
    private IEnumerator PlayDieAnimation()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}