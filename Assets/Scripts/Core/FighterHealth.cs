using UnityEngine;
using UnityEngine.Events;

public class FighterHealth : MonoBehaviour
{
    [SerializeField, Tooltip("Current damage percentage")]
    private float currentDamage = 0f;

    [SerializeField, Tooltip("Scaling factor for knockback based on damage")]
    private float knockbackScaling = 0.12f;

    public UnityEvent<float> onDamageChanged;
    public UnityEvent<Vector2> onKnockbackReceived;

    private Rigidbody2D rb;
    private CharacterController2D controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<CharacterController2D>();
    }

    public void TakeHit(AttackData attack, Vector2 attackerPos)
    {
        if (controller != null && controller.IsInvincible)
        {
            Debug.Log($"{gameObject.name} is invincible! Dodged {attack.attackName}");
            return;
        }

        // Apply damage
currentDamage += attack.damage;
        onDamageChanged?.Invoke(currentDamage);
        
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateDamage(controller.PlayerNumber, currentDamage);
        }

        // Apply Hitstun
        if (controller != null)
        {
            controller.ApplyHitstun(attack.hitstunFrames);
        }

        // Calculate knockback direction
        Vector2 direction = (Vector2)transform.position - attackerPos;
        direction.Normalize();
        
        float finalKnockbackPower = (attack.knockbackPower + currentDamage * knockbackScaling);
        Vector2 knockbackVector = direction * finalKnockbackPower;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackVector, ForceMode2D.Impulse);
        
        onKnockbackReceived?.Invoke(knockbackVector);
        
        Debug.Log($"{gameObject.name} took {attack.damage}% damage. Total: {currentDamage}%. Knockback: {finalKnockbackPower}");
    }

    public void ResetDamage()
    {
        currentDamage = 0f;
        onDamageChanged?.Invoke(currentDamage);
        
        if (HUDManager.Instance != null && controller != null)
        {
            HUDManager.Instance.UpdateDamage(controller.PlayerNumber, currentDamage);
        }
    }

    public float GetCurrentDamage() => currentDamage;
}
