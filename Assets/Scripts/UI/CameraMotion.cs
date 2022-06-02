using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
  [SerializeField]
  public Transform target;
  Vector3 offset = new Vector3(0, 0.8f, -10f);
  Vector3 _offset;
  Vector2 _cameraOffset;
  float smoothingFactor = 6;
  PlayerInputActions playerInputActions;

  void Start()
  {
    playerInputActions = new PlayerInputActions();
    playerInputActions.Camera.Enable();
  }

  void FixedUpdate()
  {
    _cameraOffset = playerInputActions.Camera.MoveCamera.ReadValue<Vector2>() * 1.5f;
    _offset = new Vector3(offset.x + _cameraOffset.x, offset.y + _cameraOffset.y, offset.z);
    transform.position = Vector3.Lerp(transform.position, target.position + _offset, smoothingFactor * Time.fixedDeltaTime);
  }
}
