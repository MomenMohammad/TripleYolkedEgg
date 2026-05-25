using UnityEngine;
using System.Collections;

public class SpeedsterController : CharacterController2D
{
    public override void InputLightAttack()
    {
        if (isAttacking) return;
        StartCoroutine(JabCombo());
    }

    private IEnumerator JabCombo()
    {
        AttackData jab1 = FindAttack("Jab 1");
        AttackData jab2 = FindAttack("Jab 2");
        
        if (jab1 == null) jab1 = FindAttack(AttackType.Light); // Fallback to any light attack

        // Hit 1
        if (jab1 != null)
        {
            yield return StartCoroutine(AttackSequence(jab1));
        }

        // Short window to follow up
        if (jab2 != null)
        {
            yield return StartCoroutine(AttackSequence(jab2));
        }
    }

    public override void InputSpecialAttack()
    {
        if (isAttacking) return;
        AttackData special = FindAttack("Special");
        if (special != null) StartCoroutine(SpinAttack(special));
    }

    private IEnumerator SpinAttack(AttackData attack)
    {
        isAttacking = true;
        animator.SetTrigger(AttackHash);
        
        // Startup
yield return new WaitForSeconds(attack.startupFrames / 60f);

        // Spin pulses
        hitbox.Initialize(attack, gameObject);
        for (int i = 0; i < 3; i++)
        {
            hitbox.Activate();
            yield return new WaitForSeconds(0.1f);
            hitbox.Deactivate();
            yield return new WaitForSeconds(0.05f);
        }

        // Recovery
        yield return new WaitForSeconds(attack.recoveryFrames / 60f);
        isAttacking = false;
    }
}
