using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    PlayerMovement2D playerMovement2D;
    Animator animator;

    void Awake()
    {
        playerMovement2D = GetComponentInParent<PlayerMovement2D>();
        if (playerMovement2D == null)
        {
            Debug.LogError("playerMovement2D component not found in parent!");
        }
        animator = GetComponentInParent<Animator>();
    }

    public void OnAttackEnd()
    {
        if (playerMovement2D != null)
        {
            playerMovement2D.isAttacking = false;
        }
    }
}