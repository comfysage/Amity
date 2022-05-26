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

  [SerializeField]
  [Header("Movement")]
  float movementSpeed = 3.2f;
  [SerializeField]
  bool speedCap = true;
  [Space(2f)]
  [Header("Jumping")]
  [SerializeField]
  float jumpForce = 6;
  [SerializeField]
  float jumpFatigue = 0.85f; // higher means less fatigue
  [SerializeField]
  int maxJump = 2;
  [SerializeField]
  float forgiveJump;

  float _forgiveJump;

  int jumpCount = 0;

  [Space(2f)]
  [Header("Debugging")]
  [SerializeField]
  bool drawGroundcheckRaycast;
  bool _isGrounded;


  //local variables
  Vector3 _localScale;

  void JumpAction(InputAction.CallbackContext context)
  {
    if (_isGrounded || jumpCount < maxJump || _forgiveJump > 0)
    {
      // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // jump height based on momentum (inconsistent and awkward)
      float _jumpForce = jumpCount == 0 ? jumpForce : (jumpForce * jumpFatigue);
      rb.velocity = new Vector2(rb.velocity.x, (Vector2.up * _jumpForce).y); // Consistent jump height
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

    _localScale = gameObject.transform.localScale;

  }

  void FixedUpdate()
  {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    CheckAnimationState(inputVector);

    rb.AddForce(new Vector2(inputVector.x, 0) * movementSpeed, ForceMode2D.Impulse);

    if (speedCap) CapVelocity();

    _isGrounded = isGrounded();
    if (!_isGrounded && jumpCount == 0)
    {
      jumpCount = 1;
      StartCoroutine(JumpForgive());
    }
    if (_isGrounded) jumpCount = 0;
  }

  void CheckAnimationState(Vector2 inputVector)
  {
    // Movement Input so flip character and set idle bool to false
    if (inputVector.x > 0)
    {
      transform.localScale = new Vector3(_localScale.x, _localScale.y, _localScale.z);
      anim.SetBool("isIdle", false);
    }
    if (inputVector.x < 0)
    {
      transform.localScale = new Vector3(-_localScale.x, _localScale.y, _localScale.z);
      anim.SetBool("isIdle", false);
    }

    // if it is not grounded, always set airborne to true 
    if (!_isGrounded)
    {
      anim.SetBool("isIdle", false);
      anim.SetBool("isRunning", false);
      anim.SetBool("isAirborne", true);
      return;
    };

    // is not airborne
    anim.SetBool("isAirborne", false);

    // if there is input and is not grounded (already checked) set running to true
    if (inputVector.x != 0 && rb.velocity.x != 0)
    {
      anim.SetBool("isIdle", false);
      anim.SetBool("isRunning", true);
      return;
    }

    // otherwise running is false
    anim.SetBool("isRunning", false);

    // if the character has no velocity, it is idle
    if (rb.velocity == new Vector2(0, 0))
    {
      anim.SetBool("isIdle", true);
    }
  }

  bool isGrounded()
  {
    float extraHeight = .1f;
    //RaycastHit2D raycastHit = Physics2D.Raycast(bc.bounds.center, Vector2.down, bc.bounds.extents.y + extraHeight, environmentMask);
    RaycastHit2D raycastHit = Physics2D.CircleCast(bc.bounds.center - new Vector3(bc.bounds.extents.x * 0.7f, bc.bounds.extents.y, 0), extraHeight, Vector2.right, bc.bounds.extents.x * 0.8f, environmentMask);

    Color rayColor;
    if (drawGroundcheckRaycast)
    { //Debug raycast
      if (raycastHit.collider != null)
        rayColor = Color.green;
      else
        rayColor = Color.red;
      //Debug.DrawRay(bc.bounds.center, Vector2.down * (bc.bounds.extents.y + extraHeight), rayColor);
      Debug.DrawRay(bc.bounds.center - new Vector3(bc.bounds.extents.x * 0.7f, bc.bounds.extents.y + extraHeight, 0), Vector2.right * (bc.bounds.extents.x * 0.8f + extraHeight), rayColor);
      Debug.DrawRay(bc.bounds.center - new Vector3(bc.bounds.extents.x * 0.7f, bc.bounds.extents.y - extraHeight, 0), Vector2.right * (bc.bounds.extents.x * 0.8f + extraHeight), rayColor);
    }

    return raycastHit.collider != null;
  }

  void CapVelocity()
  {
    float cappedXVelocity = Mathf.Min(Mathf.Abs(rb.velocity.x), movementSpeed) * Mathf.Sign(rb.velocity.x);

    rb.velocity = new Vector2(cappedXVelocity, rb.velocity.y);
  }

  IEnumerator JumpForgive()
  {
    _forgiveJump = forgiveJump;
    while (_forgiveJump > 0 && !_isGrounded)
    {
      _forgiveJump -= Time.deltaTime;
      yield return null;
    }
  }

}
