using UnityEngine;

public class AttackState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.ResetFlagsForAttack();
    }

    public void Exit(Enemy enemy)
    {
        throw new System.NotImplementedException();
    }

    public void Update(Enemy enemy)
    {
        throw new System.NotImplementedException();
    }
}
