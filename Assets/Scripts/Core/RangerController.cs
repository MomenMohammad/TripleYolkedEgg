using UnityEngine;
using System.Collections;

public class RangerController : CharacterController2D
{
    [Header("Ranger Specific")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float teleportDistance = 5f;

    public override void InputSpecialAttack()
{
        if (isAttacking) return;
        AttackData special = FindAttack("Special");
        if (special != null) StartCoroutine(TeleportDash(special));
    }

    private IEnumerator TeleportDash(AttackData attack)
    {
        isAttacking = true;
        isInvincible = true;
        animator.SetTrigger(AttackHash);
        float facing = Facing;

        yield return new WaitForSeconds(attack.startupFrames / 60f);

        // Leave hitbox at exit point
        hitbox.Initialize(attack, gameObject);
        hitbox.Activate();

        transform.position += new Vector3(facing * teleportDistance, 0, 0);

        yield return new WaitForSeconds(attack.activeFrames / 60f);
        hitbox.Deactivate();

        yield return new WaitForSeconds(attack.recoveryFrames / 60f);
        isAttacking = false;
        isInvincible = false;
    }
}
