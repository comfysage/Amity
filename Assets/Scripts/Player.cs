using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public enum playerState
  {
    Normal,
    Dashing,
  }
  public static playerState currentState = playerState.Normal;

}
