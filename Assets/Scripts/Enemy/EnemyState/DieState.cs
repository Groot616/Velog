using UnityEngine;

public class DieState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.ResetFlagsForDie();
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
