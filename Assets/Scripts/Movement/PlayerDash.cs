using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
  public PlayerMovement pm;
  private PlayerInputActions playerInputActions;
  private Rigidbody2D rb;

  float dashSpeed = 1.2f;

  void DashAction(InputAction.CallbackContext context)
  {
    rb.velocity = new Vector2(rb.velocity.x + (transform.localScale.x * dashSpeed), 0);
  }

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }
  void Start()
  {
    playerInputActions = pm.playerInputActions;
    playerInputActions.Player.Dash.performed += DashAction;
  }
}
