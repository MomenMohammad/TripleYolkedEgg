using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Fighter/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Tooltip("Display name of the character")]
    public string characterName;

    [Tooltip("Movement speed multiplier")]
    public float speed = 5f;

    [Tooltip("Weight of the character (affects knockback)")]
    public float weight = 1f;

    [Tooltip("Jump force multiplier")]
    public float jumpForce = 10f;

    [Tooltip("Placeholder for sprite sheet reference")]
    public Texture2D spriteSheet;

    [Tooltip("List of attacks for this character")]
    public List<AttackData> attacks;
}
