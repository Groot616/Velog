using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.ResetFlagsForIdle();
    }

    public void Exit(Enemy enemy)
    {
        
    }

    public void Update(Enemy enemy)
    {
        enemy.basicInfo.MoveDirection = Vector2.zero;

        // player가 공격 범위 내 존재할 경우 attack state 전환
        if (enemy.CanAttack())
        {
            enemy.ChangeState(enemy.AttackState);
            return;
        }
        // player 발견시 chase state 전환
        else if (enemy.CanChase())
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }
    }
}
