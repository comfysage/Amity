using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField]
  LayerMask environmentMask;
  Rigidbody2D rb;
  BoxCollider2D bc;
  PlayerInputActions playerInputActions;
  [SerializeField]
  [Header("Movement")]
  float movementSpeed = 3.2f;
  [SerializeField]
  bool speedCap = true;
  [Space(2f)]
  [Header("Jumping")]
  [SerializeField]
  float jumpForce = 5;
  [SerializeField]
  int maxJump = 2;
  int jumpCount = 0;

  [Space(2f)]
  [Header("Debugging")]
  [SerializeField]
  bool drawGroundcheckRaycast;

  void JumpAction(InputAction.CallbackContext context)
  {
    if (isGrounded() || jumpCount < maxJump)
    {
      // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // jump height based on momentum (inconsistent and awkward)
      rb.velocity = new Vector2(rb.velocity.x, (Vector2.up * jumpForce).y); // Consistent jump height
      jumpCount++;
      return;
    }
  }

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    bc = GetComponent<BoxCollider2D>();


    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();
    playerInputActions.Player.Jump.performed += JumpAction;
  }

  void FixedUpdate()
  {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    if (inputVector.x > 0) transform.localScale = new Vector3(2, 2, 2);
    
    if (inputVector.x < 0) transform.localScale = new Vector3(-2, 2, 2);

    rb.AddForce(new Vector2(inputVector.x, 0) * movementSpeed, ForceMode2D.Impulse);

    if (speedCap) CapVelocity();
   

    if (!isGrounded() && jumpCount == 0) jumpCount = 1;
    if (isGrounded()) jumpCount = 0;
  }

  bool isGrounded()
  {
    float extraHeight = .1f;
    RaycastHit2D raycastHit = Physics2D.Raycast(bc.bounds.center, Vector2.down, bc.bounds.extents.y + extraHeight, environmentMask);

    Color rayColor;
    if(drawGroundcheckRaycast){ //Debug raycast
    if (raycastHit.collider != null)
      rayColor = Color.green;
    else 
      rayColor = Color.red;
    Debug.DrawRay(bc.bounds.center, Vector2.down * (bc.bounds.extents.y + extraHeight), rayColor);
    }

    return raycastHit.collider != null;
  }

  void CapVelocity()
  {
    float cappedXVelocity = Mathf.Min(Mathf.Abs(rb.velocity.x), movementSpeed) * Mathf.Sign(rb.velocity.x);

    rb.velocity = new Vector2(cappedXVelocity, rb.velocity.y);
  }
}
