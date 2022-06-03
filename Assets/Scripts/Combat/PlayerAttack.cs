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

  public float slashTime = 0.38f;
  public float _slashTime;

  public LayerMask enemyLayers;

  void Awake()
  {
    playerInputActions.Player.Slash.performed += SlashAction;
  }

  void SlashAction(InputAction.CallbackContext context)
  {
    if (currentState == playerState.Normal)
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
  }

  void OnDrawGizmosSelected()
  {
    if (attackPoint == null)
      return;

    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
  }
}
