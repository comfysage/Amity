using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public enum playerState
  {
    Normal,
    Dashing,
    Slashing,
  }
  public static playerState currentState = playerState.Normal;

  public static PlayerInputActions playerInputActions { get; private set; }

  void Awake()
  {
    playerInputActions = new PlayerInputActions();
  }
}
