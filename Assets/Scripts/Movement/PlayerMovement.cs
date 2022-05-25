using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField]
  Rigidbody2D rb;

  public void Jump()
  {
    rb.AddForce(Vector2.up * 1.8f, ForceMode2D.Impulse);
  }

  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  void Update()
  {

  }
}
