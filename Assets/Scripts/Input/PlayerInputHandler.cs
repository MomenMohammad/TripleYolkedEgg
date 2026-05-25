using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private CharacterController2D controller;
    private PlayerInput playerInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();
        if (move != Vector2.zero) Debug.Log($"{gameObject.name} PlayerInput OnMove: {move}");
        controller.SetMoveInput(move);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log($"{gameObject.name} PlayerInput OnJump");
            controller.InputJump();
        }
    }

    public void OnLightAttack(InputValue value)
    {
        if (value.isPressed)
        {
            controller.InputLightAttack();
        }
    }

    public void OnSpecial(InputValue value)
{
        if (value.isPressed)
        {
            controller.InputSpecialAttack();
        }
    }

    public void OnDodge(InputValue value)
    {
        if (value.isPressed)
        {
            controller.InputDodge();
        }
    }
}
