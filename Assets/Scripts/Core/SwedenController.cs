using UnityEngine;
using System.Collections;

public class SwedenController : CharacterController2D
{
    private static readonly int AttackTypeHash = Animator.StringToHash("AttackType");

    public override void InputLightAttack()
    {
        if (isAttacking) return;

        AttackData attackToUse = null;
        int attackTypeIndex = 0;

        if (moveInput.y > 0.5f)
        {
            attackToUse = FindAttack(AttackType.Up);
            attackTypeIndex = 1;
        }
        else if (moveInput.y < -0.5f)
        {
            attackToUse = FindAttack(AttackType.Down);
            attackTypeIndex = 2;
        }
        else
        {
            attackToUse = FindAttack(AttackType.Light);
            attackTypeIndex = 0;
        }

        if (attackToUse != null)
        {
            animator.SetInteger(AttackTypeHash, attackTypeIndex);
            StartCoroutine(AttackSequence(attackToUse));
        }
    }
}

