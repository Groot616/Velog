using UnityEngine;

public class DieState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.ResetFlagsForDie();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
