using UnityEngine;
using System.Collections;

public class BruiserController : CharacterController2D
{
    [Header("Bruiser Specific")]
    [SerializeField] private float bruiserDashForce = 15f;
    [SerializeField] private float bruiserDashDuration = 0.3f;

    public override void InputSpecialAttack()
{
        if (isAttacking) return;

        AttackData special = FindAttack("Special");
        if (special == null) return;

        StartCoroutine(ChargeDash(special));
    }

    private IEnumerator ChargeDash(AttackData attack)
    {
        isAttacking = true;
        animator.SetTrigger(AttackHash);
        float facing = Facing;

        // Startup
yield return new WaitForSeconds(attack.startupFrames / 60f);

        // Active Dash
        hitbox.Initialize(attack, gameObject);
        hitbox.Activate();
        
        float timer = 0;
        while (timer < bruiserDashDuration)
        {
            rb.linearVelocity = new Vector2(facing * bruiserDashForce, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox.Deactivate();
        rb.linearVelocity = Vector2.zero;

        // Recovery
        yield return new WaitForSeconds(attack.recoveryFrames / 60f);
        isAttacking = false;
    }
}
