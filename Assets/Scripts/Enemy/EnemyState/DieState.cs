using UnityEngine;
using System.Collections;

public class DieState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy e)
    {
        enemy = e;
        enemy.ResetFlagsForDie();

        Die();
    }

    public void Exit() { }

    public void Update() { }

    private void Die()
    {
        if (enemy.basicInfo.IsDieAnimationTriggered)
            return;

        enemy.basicInfo.IsDieAnimationTriggered = true;
        Collider2D collider = enemy.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        enemy.basicComponents.Animator.SetTrigger("Die");

        enemy.RunCoroutine(DeathTeloportSequence());
    }

        private IEnumerator DeathTeloportSequence()
    {
        Vector2 OriginPos = enemy.transform.position;

        int repeatTwice = 0;
        while (repeatTwice < 7)
        {
            enemy.transform.position = OriginPos + new Vector2(-0.05f, 0f);
            yield return new WaitForSeconds(0.05f);

            enemy.transform.position = OriginPos + new Vector2(0.05f, 0f);
            yield return new WaitForSeconds(0.05f);
            ++repeatTwice;
        }
        enemy.transform.position = OriginPos;

        enemy.basicInfo.IsDie = true;
        enemy.basicInfo.IsMoving = false;
        enemy.basicInfo.IsWaitingAttack = false;
        enemy.basicInfo.IsTracing = false;
        yield return new WaitForSeconds(2f);
        UnityEngine.Object.Destroy(enemy.gameObject);
    }
}
