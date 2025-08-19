using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy e)
    {
        enemy = e;
        enemy.ResetFlagsForIdle();
        enemy.basicInfo.MoveDirection = Vector2.zero;
    }

    public void Exit() { }

    public void Update()
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
