using UnityEngine;

public class AttackRange : MonoBehaviour
{
    private PlayerMovement2D player;
    private float damage;

    //private void Awake()
    //{
    //    player = GetComponentInParent<PlayerMovement2D>();
    //    if(player == null)
    //    {
    //        Debug.LogError("player component not found in parent!");
    //    }
    //}

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (damage <= 0f)
                {
                    Debug.LogWarning("Damage is zero or less. Did you forget to set damage?");
                }
                enemy.TakeDamage(damage);
            }
        }
    }
}
