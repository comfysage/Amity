using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Player;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public LayerMask environmentMask;
    Rigidbody2D rb;
    BoxCollider2D bc;
    Vector2 inputVector;
    public Animator anim;

    [Header("Movement")]
    [SerializeField, Range(0f, 12f)] private float _maxSpeed = 8f;
    [SerializeField, Range(0f, 50f)] private float _maxAcceleration = 40f;
    [SerializeField, Range(0f, 50f)] private float _maxAirAcceleration = 45f;
    private float _maxSpeedChange, _acceleration;
    private Vector2 _direction, _desiredVelocity, _velocity;
    [SerializeField]
    public float speedCapFactor = 1;
    [Space(2f)]
    [Header("Jumping")]
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
    float _jumpSpeed;
    [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float _upwardMovementMultiplier = 1.7f;
    private float _defaultGravityScale;
    [SerializeField]
    float jumpFatigue = 0.85f; // higher means less fatigue
    [SerializeField]
    int maxJump = 2;
    [SerializeField]
    float forgiveJump, jumpQueueTime;

    float _forgiveJump, _jumpQueueTime;
    
    int jumpCount = 0;

    [Space(2f)]
    [Header("Debugging")]
    [SerializeField]
    bool drawGroundcheckRaycast;
    [SerializeField]
    float forgiveTime;
    float _timeLastJump = -1;
    bool _isGrounded;

    void JumpAction(InputAction.CallbackContext context)
    {
        Jump();
    }
    bool canJump(){
      return ((_isGrounded || jumpCount < maxJump || _forgiveJump > 0) && currentState == playerState.Normal);
    }


    void Jump(bool queue=true)
    {
      if (canJump()){
      // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // jump height based on momentum (inconsistent and awkward)
      _velocity = rb.velocity;

      _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight * (jumpCount == 0 ? 1: jumpFatigue));

      if (_velocity.y > 0f)
      {
        _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
      }
      else if (_velocity.y < 0f)
      {
        _jumpSpeed += Mathf.Abs(rb.velocity.y);
      }

      _velocity.y += _jumpSpeed;
      rb.velocity = _velocity;

      _timeLastJump = Time.unscaledTime;
      if (_forgiveJump < 0)
          jumpCount++;
      return;
      } else if (queue) StartCoroutine(QueueJump());
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim.SetBool("isIdle", true);
        anim.SetBool("isRunning", false);

        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += JumpAction;

        _defaultGravityScale = 1f;
    }

    void Update()
    {
        inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        _direction.x = inputVector.x;
        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - 2f, 0f);
    }

    void FixedUpdate()
    {
        CheckAnimationState(inputVector);
        _isGrounded = isGrounded();

        // rb.AddForce(new Vector2(inputVector.x, 0) * movementSpeed, ForceMode2D.Impulse);
        if (currentState == playerState.Normal){
          _velocity = rb.velocity;
          _acceleration = _isGrounded ? _maxAcceleration : _maxAirAcceleration;
          _maxSpeedChange = _acceleration * Time.deltaTime;
          _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

          rb.velocity = _velocity;
            // rb.velocity = new Vector2(inputVector.x * Time.fixedDeltaTime * movementSpeed, rb.velocity.y);
        }

        // CapVelocity();

        if (!_isGrounded && jumpCount == 0)
        {
            jumpCount = 1;
            StartCoroutine(JumpForgive());
        }
        if (_isGrounded && _timeLastJump + forgiveTime <= Time.unscaledTime)
        {
            jumpCount = 0;
        }

        if (rb.velocity.y > 0)
        {
          rb.gravityScale = _upwardMovementMultiplier;
        }
        else if (rb.velocity.y < 0)
        {
          rb.gravityScale = _downwardMovementMultiplier;
        }
        else if(rb.velocity.y == 0)
        {
          rb.gravityScale = _defaultGravityScale;
        }

    }

    void CheckAnimationState(Vector2 inputVector)
    {
        // Movement Input so flip character and set idle bool to false
        if (inputVector.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            anim.SetBool("isIdle", false);
        }
        if (inputVector.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
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
        // float cappedXVelocity = Mathf.Min(Mathf.Abs(rb.velocity.x), (speedCapFactor * movementSpeed)) * Mathf.Sign(rb.velocity.x);

        // rb.velocity = new Vector2(cappedXVelocity, rb.velocity.y);
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

    IEnumerator QueueJump()
    {
        _jumpQueueTime = jumpQueueTime;
        while (_jumpQueueTime > 0)
        {
            _jumpQueueTime -= Time.deltaTime;
            if(canJump()){
              Jump(false);
              _jumpQueueTime=-1f;
            }
            yield return null;
        }
    }

}
