using UnityEngine;

public class MoveState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.ResetFlagsForMove();
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
