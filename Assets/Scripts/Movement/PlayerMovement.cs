using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField]
  Rigidbody2D rb;

  public void Jump() { }

  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  void Update()
  {

  }
}
