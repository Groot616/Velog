using UnityEngine;

public class AttackRange : MonoBehaviour
{
    private enum AttackerType { Player, Enemy };
    private AttackerType attackerType;
    private float damage;
    private Collider2D attackCollider;

    //private PlayerMovement2D player;
    //private float damage;

    private void Awake()
    {
        attackCollider = GetComponent<Collider2D>();
        attackCollider.enabled = false;

        if (transform.root.CompareTag("Player"))
            attackerType = AttackerType.Player;
        else if (transform.root.CompareTag("Enemy"))
            attackerType = AttackerType.Enemy;

        Debug.Log("AttackerType : " + attackerType);
    }
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void EnableAttackerCollider()
    {
        attackCollider.enabled = true;
    }

    public void DisableAttackerCollider()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackerType == AttackerType.Player && collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                Debug.Log("AttackRange class\\TakeDamage() is called!");
                enemy.TakeDamage(damage);
            }
        }
        else if(attackerType == AttackerType.Enemy && collision.CompareTag("Player"))
        {
            PlayerMovement2D player = collision.GetComponent<PlayerMovement2D>();
            if(player != null)
            {
                player.TakeDamage(damage);
            }

        }
        //if (collision.CompareTag("Enemy"))
        //{
        //    Enemy enemy = collision.GetComponent<Enemy>();
        //    if (enemy != null)
        //    {
        //        if (damage <= 0f)
        //        {
        //            Debug.LogWarning("Damage is zero or less. Did you forget to set damage?");
        //        }
        //        enemy.TakeDamage(damage);
        //    }
        //}
    }
}