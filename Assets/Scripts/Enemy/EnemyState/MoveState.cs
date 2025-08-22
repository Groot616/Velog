using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy e)
    {
        enemy = e;
        enemy.ResetFlagsForMove();
    }

    public void Exit() { }

    public void Update()
    {

        if (enemy.CanChase())
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }
        else if(enemy.CanAttack())
        {
            enemy.ChangeState(enemy.AttackState);
            return;
        }
        else if (enemy.CanMove())
        {
            Patrol();
        }
    }

    public void Patrol()
    {
        Vector2 targetPos = new Vector2(enemy.detection.Target.position.x, enemy.transform.position.y);
        Vector2 direction = (targetPos - (Vector2)enemy.transform.position).normalized;
        enemy.basicInfo.MoveDirection = direction;

        if (enemy.basicInfo.IsMoving)
        {
            enemy.basicComponents.SpriteRenderer.flipX = targetPos.x > enemy.transform.position.x;
            enemy.FlipDetectionCenterAccordingToDetection();
        }

        if (Vector2.Distance(enemy.transform.position, targetPos) < 0.1f)
        {
            enemy.basicInfo.MoveDirection = Vector2.zero;
            enemy.RunCoroutine(WaitCoroutine());
            enemy.detection.Target = (enemy.detection.Target == enemy.detection.PointA) ? enemy.detection.PointB : enemy.detection.PointA;
        }
    }

    private IEnumerator WaitCoroutine()
    {
        enemy.basicInfo.CurrentState = BasicInfo.State.Idle;
        enemy.basicInfo.IsMoving = false;
        enemy.basicComponents.Animator.SetBool("isMoving", enemy.basicInfo.IsMoving);
        yield return new WaitForSeconds(enemy.basicInfo.WaitingTime);

        enemy.basicInfo.IsMoving = true;
        enemy.basicComponents.Animator.SetBool("isMoving", enemy.basicInfo.IsMoving);
        enemy.basicInfo.CurrentState = BasicInfo.State.Move;
    }
}
