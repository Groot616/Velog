using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ChaseState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy e)
    {
        enemy = e;
        enemy.ResetFlagsForChase();

        enemy.basicComponents.Rigidbody.MovePosition(enemy.basicComponents.Rigidbody.position + enemy.basicInfo.MoveDirection * enemy.basicInfo.SpeedForTracing * Time.fixedDeltaTime);
    }

    public void Exit()
    {
        enemy.basicComponents.Rigidbody.MovePosition(enemy.basicComponents.Rigidbody.position + enemy.basicInfo.MoveDirection * enemy.basicInfo.Speed * Time.fixedDeltaTime);
    }

    public void Update()
    {
        if (enemy.CanAttack())
        {
            enemy.ChangeState(enemy.AttackState);
            return;
        }
        else if (!enemy.detection.playerMovement2D.isDie && enemy.CanChase())
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = new Vector2(enemy.detection.PlayerPos.position.x - enemy.transform.position.x, 0f).normalized;
        enemy.basicInfo.MoveDirection = direction;
        enemy.basicInfo.IsTracing = true;
        enemy.basicInfo.IsMoving = true;
        enemy.basicComponents.Animator.SetBool("isMoving", enemy.basicInfo.IsMoving);
        enemy.basicComponents.SpriteRenderer.flipX = direction.x > 0;
        enemy.FlipDetectionCenterAccordingToDetection();

        enemy.basicComponents.Rigidbody.MovePosition(enemy.basicComponents.Rigidbody.position + enemy.basicInfo.MoveDirection * enemy.basicInfo.SpeedForTracing * Time.fixedDeltaTime);
    }
}
