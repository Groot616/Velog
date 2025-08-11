using UnityEngine;

public class ChaseState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.ResetFlagsForChase();
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
