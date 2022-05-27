using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
  public PlayerMovement pm;
  private PlayerInputActions playerInputActions;
  private Rigidbody2D rb;

  Vector2 _velocity;
  float _gravity;

  [SerializeField]
  float dashSpeed = 6f;
  [SerializeField]
  float dashTime = 0.4f;
  float _dashTime;

  void DashAction(InputAction.CallbackContext context)
  {
    StartCoroutine(Dash());
  }

  IEnumerator Dash()
  {
    float direction = Mathf.Sign(transform.localScale.x);
    _velocity = rb.velocity;
    rb.gravityScale = 0;
    _dashTime = dashTime;

    rb.velocity = new Vector2(direction * dashSpeed, 0);

    // wait till dash is over
    while (_dashTime > 0)
    {
      _dashTime -= Time.deltaTime;
      yield return null;
    }
    // reset gravity and x velocity
    rb.gravityScale = _gravity;
    rb.velocity = new Vector2(_velocity.x, 0);
  }

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    _gravity = rb.gravityScale;
  }
  void Start()
  {
    playerInputActions = pm.playerInputActions;
    playerInputActions.Player.Dash.performed += DashAction;
  }
}
