using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private PlayerController controller;

    [Header("Actions")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference attack;

    void OnEnable()
    {
        if (move != null) move.action.Enable();
        if (jump != null) jump.action.Enable();
        if (attack != null) attack.action.Enable();

        if (jump != null)
        {
            jump.action.performed += OnJump;
            jump.action.canceled += OnJump;
        }
        if (attack != null) attack.action.performed += OnAttack;
    }

    void OnDisable()
    {
        if (jump != null)
        {
            jump.action.performed -= OnJump;
            jump.action.canceled -= OnJump;
        }
        if (attack != null) attack.action.performed -= OnAttack;

        if (move != null) move.action.Disable();
        if (jump != null) jump.action.Disable();
        if (attack != null) attack.action.Disable();
    }

    void Update()
    {
        if (controller == null || move == null) return;

        Vector2 moveValue = move.action.ReadValue<Vector2>();
        controller.SetMoveInput(moveValue.x);
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (controller == null) return;
        if (ctx.performed) controller.Jump();
        if (ctx.canceled) controller.OnJumpUp();
    }

    void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) controller.Attack();
    }

    public void DoJump() => controller.Jump();
    public void DoAttack() => controller.Attack();
}