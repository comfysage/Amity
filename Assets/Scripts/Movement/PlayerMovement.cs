using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField]
  public LayerMask environmentMask;
  Rigidbody2D rb;
  BoxCollider2D bc;
  PlayerInputActions playerInputActions;
  public Animator anim;

  float movementSpeed = 3.2f;
  bool speedCap = true;

  float jumpForce = 5;
  int jumpCount = 0;

  void JumpAction(InputAction.CallbackContext context)
  {
    if (isGrounded() || jumpCount < 2)
    {
      // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // jump height based on momentum (inconsistent and awkward)
      anim.SetBool("isAirborne", true);
      anim.SetBool("isRunning", false);
      rb.velocity = new Vector2(rb.velocity.x, (Vector2.up * jumpForce).y); // Consistent jump height
      jumpCount++;
      return;
    }
  }

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    bc = GetComponent<BoxCollider2D>();
    anim.SetBool("isIdle", true);
    anim.SetBool("isRunning", false);


    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();
    playerInputActions.Player.Jump.performed += JumpAction;
  }

  void FixedUpdate()
  {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    CheckAnimationState(inputVector);
    rb.AddForce(new Vector2(inputVector.x, 0) * movementSpeed, ForceMode2D.Impulse);
    if (speedCap)
    {
      CapVelocity();
    }

    if (!isGrounded() && jumpCount == 0) jumpCount = 1;
    if (isGrounded()) jumpCount = 0;
  }

  void CheckAnimationState(Vector2 inputVector)
  {
    Debug.Log(inputVector.x);

    if (inputVector.x > 0)
    {
      transform.localScale = new Vector3(2, 2, 2);
      anim.SetBool("isIdle", false);
    }
    if (inputVector.x < 0)
    {
      transform.localScale = new Vector3(-2, 2, 2);
      anim.SetBool("isIdle", false);
    }

    if (!isGrounded())
    {
      anim.SetBool("isIdle", false);
      anim.SetBool("isRunning", false);
      anim.SetBool("isAirborne", true);
      return;
    };

    anim.SetBool("isAirborne", false); // is not airborne

    if (inputVector.x != 0)
    {
      anim.SetBool("isIdle", false);
      anim.SetBool("isRunning", true);
      return;
    }

    anim.SetBool("isRunning", false);

    if (rb.velocity == new Vector2(0, 0))
    {
      anim.SetBool("isIdle", true);
    }
  }

  bool isGrounded()
  {
    float extraHeight = .1f;
    RaycastHit2D raycastHit = Physics2D.Raycast(bc.bounds.center, Vector2.down, bc.bounds.extents.y + extraHeight, environmentMask);

    Color rayColor;
    if (raycastHit.collider != null)
    {
      rayColor = Color.green;
    }
    else
    {
      rayColor = Color.red;
    }
    Debug.DrawRay(bc.bounds.center, Vector2.down * (bc.bounds.extents.y + extraHeight), rayColor);

    return raycastHit.collider != null;
  }

  void CapVelocity()
  {
    float cappedXVelocity = Mathf.Min(Mathf.Abs(rb.velocity.x), movementSpeed) * Mathf.Sign(rb.velocity.x);

    rb.velocity = new Vector2(cappedXVelocity, rb.velocity.y);
  }
}
