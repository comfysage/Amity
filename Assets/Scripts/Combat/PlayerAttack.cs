using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Player;

public class PlayerAttack : MonoBehaviour
{
  public Animator animator;

  public Vector2 attackPointOffset = new Vector2(0, .1f);
  public Vector2 attackPointRadius = new Vector2(.3f, .2f);
  public float attackRange = 1f;
  public float attackSize = .8f;

  private Vector2 _inputVector;
  private Vector2 _direction;
  private Vector3 _activePoint;

  [SerializeField]
  public float slashTime = 0.12f;
  private float _slashTime;
  [SerializeField]
  public float slashFatigueWaitTime = 0.18f;
  private float _slashFatigue;
  public LayerMask enemyLayers;

  void Awake()
  {
    playerInputActions.Player.Slash.performed += SlashAction;
    _direction = new Vector2(1, 0);
  }

  void FixedUpdate()
  {
    _inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    _direction = new Vector2(Mathf.Sign(transform.rotation.y), _inputVector.y);
    _activePoint = transform.position + new Vector3(attackPointOffset.x, attackPointOffset.y, 0);

    _activePoint.y += _inputVector.y * attackPointRadius.y;
    _activePoint.x += (_inputVector.y != 0 ? _inputVector.x : _direction.x) * attackPointRadius.x;
  }

  void SlashAction(InputAction.CallbackContext context)
  {
    if (currentState != playerState.Dashing && _slashFatigue == 0)
    {
      currentState = playerState.Slashing;
      // animator.SetTrigger("Attack"); // Add Attack animation
      StartCoroutine(Slash());
    }
  }
  IEnumerator Slash()
  {
    bool _vertical = _direction.y != 0;

    float LinearOffset = _vertical ? attackRange * _direction.y : attackRange * _direction.x;
    float SizeOffset = _vertical ? attackSize : attackSize;

    float _xOffset = _vertical ? SizeOffset / 2 : LinearOffset;
    float _yOffset = _vertical ? LinearOffset : SizeOffset / 2;
    Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(
      new Vector2(_activePoint.x + (_vertical ? _xOffset : 0), _activePoint.y + (_vertical ? 0 : _yOffset)),
      new Vector2(_activePoint.x + (_vertical ? -_xOffset : _xOffset), _activePoint.y + (_vertical ? _yOffset : -_yOffset)), enemyLayers);

    foreach (Collider2D enemy in hitEnemies)
    {
      Debug.Log("enemy hit: " + enemy.name);
    }

    _slashTime = slashTime;
    while (_slashTime > 0)
    {
      _slashTime -= Time.deltaTime;
      yield return null;
    }

    currentState = playerState.Normal;
    _slashFatigue = slashFatigueWaitTime;
    while (_slashFatigue > 0)
    {
      _slashFatigue -= Time.deltaTime;
      yield return null;
    }
    _slashFatigue = 0;
  }

  void OnDrawGizmosSelected()
  {
    if (_activePoint == null)
      return;

    bool _vertical = _direction.y != 0;

    float LinearOffset = _vertical ? attackRange * _direction.y : attackRange * _direction.x;
    float SizeOffset = _vertical ? attackSize : attackSize;

    float _xOffset = _vertical ? SizeOffset / 2 : LinearOffset;
    float _yOffset = _vertical ? LinearOffset : SizeOffset / 2;

    Gizmos.DrawLine(
      new Vector3(_activePoint.x + (_vertical ? _xOffset : 0), _activePoint.y + (_vertical ? 0 : _yOffset), _activePoint.z),
      new Vector3(_activePoint.x + (_vertical ? -_xOffset : _xOffset), _activePoint.y + (_vertical ? _yOffset : -_yOffset), _activePoint.z)
      );
  }
}
