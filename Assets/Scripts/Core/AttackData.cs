using UnityEngine;

public enum AttackType { Light, Special, Up, Down }

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Fighter/AttackData")]
public class AttackData : ScriptableObject
{
    [Tooltip("Type of the attack")]
    public AttackType type;

    [Tooltip("Name of the attack")]
    public string attackName;

    [Tooltip("Damage dealt by the attack")]
    public float damage;

    [Tooltip("Base knockback power")]
    public float knockbackPower;

    [Tooltip("Angle of knockback (in degrees)")]
    public float knockbackAngle;

    [Tooltip("Number of frames the target is in hitstun")]
    public int hitstunFrames;

    [Tooltip("Number of frames before the hitbox becomes active")]
    public int startupFrames;

    [Tooltip("Number of frames the hitbox is active")]
    public int activeFrames;

    [Tooltip("Number of frames after the hitbox is gone before the character can act")]
    public int recoveryFrames;

    [Tooltip("Offset of the hitbox relative to the character center")]
    public Vector2 hitboxOffset;

    [Tooltip("Size of the hitbox")]
    public Vector2 hitboxSize;
}
