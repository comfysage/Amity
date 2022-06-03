using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Player;

public class PlayerAttack : MonoBehaviour
{
  public Animator animator;

  public Transform attackPoint;
  public float attackRange = 0.5f;

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
  }

  void SlashAction(InputAction.CallbackContext context)
  {
    if (currentState == playerState.Normal && _slashFatigue == 0)
    {
      currentState = playerState.Slashing;
      // animator.SetTrigger("Attack"); // Add Attack animation
      StartCoroutine(Slash());
    }
  }
  IEnumerator Slash()
  {
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

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
    if (attackPoint == null)
      return;

    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
  }
}
