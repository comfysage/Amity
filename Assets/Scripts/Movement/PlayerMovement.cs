using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField]
  Rigidbody2D rb;

  public void JumpAction(InputAction.CallbackContext context)
  {
    rb.AddForce(Vector2.up * 1.8f, ForceMode2D.Impulse);
  }

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();

    PlayerInputActions playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();
    playerInputActions.Player.Jump.performed += JumpAction;
  }

  void Update()
  {

  }
}
