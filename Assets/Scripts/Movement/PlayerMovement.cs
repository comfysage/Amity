using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField]
  Rigidbody2D rb;
  PlayerInputActions playerInputActions;

  float movementSpeed = 3.2f;
  bool speedCap = true;

  void JumpAction(InputAction.CallbackContext context)
  {
    rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
  }

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();

    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();
    playerInputActions.Player.Jump.performed += JumpAction;
  }

  void FixedUpdate()
  {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    rb.AddForce(new Vector2(inputVector.x, 0) * movementSpeed, ForceMode2D.Impulse);
    if (speedCap)
    {
      CapVelocity();
    }
  }

  void CapVelocity()
  {
    float cappedXVelocity = Mathf.Min(Mathf.Abs(rb.velocity.x), movementSpeed) * Mathf.Sign(rb.velocity.x);

    rb.velocity = new Vector2(cappedXVelocity, rb.velocity.y);
  }
}
