using UnityEngine;
using System.Collections;

public class AttackState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy e)
    {
        enemy = e;
        enemy.ResetFlagsForAttack();
    }

    public void Exit() { }

    public void Update()
    {
        if (enemy.CanChase())
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }
        
        if(enemy.CanAttack())
        {
            Attack();
        }

        if(enemy.CanMove())
        {
            enemy.ChangeState(enemy.MoveState);
            return;
        }
    }

    void Attack()
    {
        enemy.basicInfo.IsMoving = false;
        enemy.basicComponents.Animator.SetBool("isMoving", enemy.basicInfo.IsMoving);
        enemy.detection.IsAttacking = true;
        enemy.basicComponents.Animator.SetBool("isAttacking", enemy.detection.IsAttacking);

        Vector2 attackPoint = new Vector2(enemy.detection.PlayerPos.position.x, enemy.transform.position.y);
        Vector2 returnPoint = enemy.transform.position;

        if (attackPoint.x < enemy.transform.position.x)
            attackPoint.x += enemy.basicInfo.AttackOffset;
        else
            attackPoint.x -= enemy.basicInfo.AttackOffset;

        if (!enemy.detection.IsTeleporting)
        {
            enemy.RunCoroutine(TeleportAndShake(attackPoint, returnPoint));
            StartChargeEffect();
        }
    }

    private IEnumerator TeleportAndShake(Vector2 targetPos, Vector2 returnPos)
    {
        enemy.detection.IsTeleporting = true;
        yield return new WaitForSeconds(enemy.basicInfo.WaitingAttack);
        enemy.transform.position = targetPos;
        enemy.RunCoroutine(CameraShake(0.3f, 0.1f));
        
        if (enemy.basicInfo.attackRange != null)
        {
            enemy.basicInfo.attackRange.GetComponent<AttackRange>().EnableAttackerCollider();
        }
        yield return new WaitForSeconds(0.1f);
        if (enemy.basicInfo.attackRange != null)
            enemy.basicInfo.attackRange.GetComponent<AttackRange>().DisableAttackerCollider();

        yield return new WaitForSeconds(1f);
        
        enemy.transform.position = returnPos;

        yield return new WaitForSeconds(1.7f);
        enemy.basicComponents.Animator.SetBool("isAttacking", enemy.detection.IsAttacking);
        enemy.detection.IsAttacking = false;
        enemy.detection.IsTeleporting = false;

        if (enemy.detection.InAttackRange && enemy.detection.PlayerPos != null && !enemy.basicInfo.IsDie)
        {
            enemy.basicInfo.CurrentState = BasicInfo.State.Attack;
            enemy.basicInfo.IsMoving = false;
            enemy.basicComponents.Animator.SetBool("isMoving", false);
            enemy.basicComponents.Animator.SetBool("isAttacking", true);
        }
        else
        {
            if (enemy.detection.InTotalDetectionRange && enemy.detection.PlayerPos != null)
            {
                enemy.basicInfo.CurrentState = BasicInfo.State.Chase;
                enemy.basicInfo.IsTracing = true;
                enemy.basicComponents.Animator.SetBool("isMoving", true);
                enemy.basicComponents.Animator.SetBool("isAttacking", false);
            }
            else
            {
                enemy.basicInfo.CurrentState = BasicInfo.State.Move;
                enemy.basicInfo.IsMoving = true;
                enemy.basicComponents.Animator.SetBool("isMoving", true);
                enemy.basicComponents.Animator.SetBool("isAttacking", false);
            }
            }
        }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        enemy.basicInfo.Cam = Camera.main;
        if (enemy.basicInfo.Cam == null) yield break;

        Vector3 OriginPos = enemy.basicInfo.Cam.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            enemy.basicInfo.Cam.transform.position = new Vector3(OriginPos.x + x, OriginPos.y + y, OriginPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        enemy.basicInfo.Cam.transform.position = OriginPos;
    }

    private void StartChargeEffect()
    {
        if (enemy.basicInfo.ChargeCoroutine != null)
            enemy.StopCoroutine(enemy.basicInfo.ChargeCoroutine);
        enemy.basicInfo.ChargeCoroutine = enemy.StartCoroutine(ChargeRedEffect());

    }

    private IEnumerator ChargeRedEffect()
    {
        enemy.basicInfo.OriginColor = enemy.basicComponents.SpriteRenderer.color;
        Color targetColor = new Color(1f, 0.4f, 0.4f);

        float elapsed = 0f;
        while (elapsed < enemy.basicInfo.WaitingAttack)
        {
            float t = elapsed / enemy.basicInfo.WaitingAttack;
            enemy.basicComponents.SpriteRenderer.color = Color.Lerp(enemy.basicInfo.OriginColor, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        enemy.basicComponents.SpriteRenderer.color = enemy.basicInfo.OriginColor;
    }
}
