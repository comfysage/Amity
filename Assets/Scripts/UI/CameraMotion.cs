using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
  [SerializeField]
  public Transform target;
  Vector3 offset = new Vector3(0, 0.8f, -10f);
  float smoothingFactor = 6;

  void FixedUpdate()
  {
    transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothingFactor * Time.fixedDeltaTime);
  }
}
