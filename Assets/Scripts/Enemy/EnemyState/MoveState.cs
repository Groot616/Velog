using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.ResetFlagsForMove();
    }

    public void Exit(Enemy enemy) { }

    public void Update(Enemy enemy)
    {
        if(enemy.CanMove())
        {
            Patrol(enemy);
            if(enemy.CanChase())
            {
                enemy.ChangeState(enemy.ChaseState);
            }
        }
    }

    public void Patrol(Enemy enemy)
    {
        Vector2 targetPos = new Vector2(enemy.detection.Target.position.x, enemy.transform.position.y);
        Vector2 direction = (targetPos - (Vector2)enemy.transform.position).normalized;
        enemy.basicInfo.MoveDirection = direction;

        if (enemy.basicInfo.IsMoving)
        {
            enemy.basicComponents.SpriteRenderer.flipX = targetPos.x > enemy.transform.position.x;
            FlipDetectionCenterAccordingToDetection(enemy);
        }

        if (Vector2.Distance(enemy.transform.position, targetPos) < 0.1f)
        {
            enemy.basicInfo.MoveDirection = Vector2.zero;
            enemy.RunCoroutine(WaitCoroutine(enemy));
            enemy.detection.Target = (enemy.detection.Target == enemy.detection.PointA) ? enemy.detection.PointB : enemy.detection.PointA;
        }
    }

    private IEnumerator WaitCoroutine(Enemy enemy)
    {
        enemy.basicInfo.CurrentState = BasicInfo.State.Idle;
        enemy.basicInfo.IsMoving = false;
        enemy.basicComponents.Animator.SetBool("isMoving", enemy.basicInfo.IsMoving);
        yield return new WaitForSeconds(enemy.basicInfo.WaitingTime);

        enemy.basicInfo.IsMoving = true;
        enemy.basicComponents.Animator.SetBool("isMoving", enemy.basicInfo.IsMoving);
        enemy.basicInfo.CurrentState = BasicInfo.State.Move;
    }

    void FlipDetectionCenterAccordingToDetection(Enemy enemy)
    {
        if (enemy.detection == null || enemy.detection.DetectionCenter == null || enemy.detection.AttackCenter == null)
            return;

        Vector2 detectionOriginalPos = enemy.detection.DetectionCenterOriginalLocalPos;
        Vector2 attackOriginalPos = enemy.detection.AttackCenterOriginalLocalPos;
        bool facingRight = enemy.basicComponents.SpriteRenderer.flipX;
        if (facingRight)
        {
            // 오른쪽 바라볼 때는 x를 양수로 유지, y는 그대로
            enemy.detection.DetectionCenter.localPosition = new Vector2(Mathf.Abs(detectionOriginalPos.x), detectionOriginalPos.y);
            enemy.detection.AttackCenter.localPosition = new Vector2(Mathf.Abs(attackOriginalPos.x), attackOriginalPos.y);
        }
        else
        {
            // 왼쪽 바라볼 때는 x를 음수로 뒤집고, y는 그대로
            enemy.detection.DetectionCenter.localPosition = new Vector2(-Mathf.Abs(detectionOriginalPos.x), detectionOriginalPos.y);
            enemy.detection.AttackCenter.localPosition = new Vector2(-Mathf.Abs(attackOriginalPos.x), attackOriginalPos.y);
        }
    }
}
