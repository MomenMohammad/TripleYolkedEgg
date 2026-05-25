using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    private AttackData currentAttack;
    private GameObject owner;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public void Initialize(AttackData attack, GameObject ownerObject)
    {
        currentAttack = attack;
        owner = ownerObject;
        hitTargets.Clear();

        // Apply size and offset to collider
        if (col is BoxCollider2D boxCol)
        {
            boxCol.size = attack.hitboxSize;
            // Adjust offset based on owner facing direction
            SpriteRenderer ownerSr = owner.GetComponentInChildren<SpriteRenderer>();
            float facing = (ownerSr != null && ownerSr.flipX) ? -1f : 1f;
            boxCol.offset = new Vector2(attack.hitboxOffset.x * facing, attack.hitboxOffset.y);
            Debug.Log($"{owner.name} Hitbox Init: Facing={facing}, Offset={boxCol.offset}, flipX={ownerSr?.flipX}");
        }
    }

    public void Activate()
    {
        col.enabled = true;
        hitTargets.Clear();
    }

    public void Deactivate()
    {
        col.enabled = false;
        hitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return; // Ignore other hitboxes
        
        GameObject targetRoot = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (targetRoot == owner) return;
        if (hitTargets.Contains(targetRoot)) return;

        FighterHealth health = targetRoot.GetComponent<FighterHealth>();
        if (health != null)
        {
            Debug.Log($"{owner.name} hit {targetRoot.name} with {currentAttack.attackName}");
            health.TakeHit(currentAttack, owner.transform.position);
            hitTargets.Add(targetRoot);
        }
    }
}
