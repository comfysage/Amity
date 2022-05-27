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
  float dashSpeed = 12f;
  [SerializeField]
  float dashTime = 1;
  float _dashTime = 1;

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
    while (_dashTime > 0)
    {
      transform.position += new Vector3(direction * dashSpeed * Time.deltaTime, 0, 0);
      _dashTime -= Time.deltaTime;
      yield return null;
    }
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
