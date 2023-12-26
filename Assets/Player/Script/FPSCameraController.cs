#region

using UnityEngine;
using UnityEngine.InputSystem;

#endregion

public class FPSCameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference aim;
    [SerializeField] private Transform fpsCameraPivot;
    [SerializeField] private float horizontalAngularSpeed = 100f;
    [SerializeField] private float verticalAngularSpeed = 40f;
    [SerializeField] private float maxVerticalAngle = 85f;

    private void OnEnable()
    {
        aim.action.Enable();
    }

    private void Update()
    {
        Vector2 aimDelta = aim.action.ReadValue<Vector2>();
        transform.Rotate(Vector3.up, aimDelta.x * horizontalAngularSpeed * Time.deltaTime);

        float currentAngle = Vector3.SignedAngle(transform.forward, fpsCameraPivot.forward, transform.right);
        float rotationToApply = aimDelta.y * verticalAngularSpeed * Time.deltaTime;
        if (currentAngle + rotationToApply > maxVerticalAngle)
        {
            fpsCameraPivot.Rotate(Vector3.right, maxVerticalAngle - currentAngle);
        }
        else if (currentAngle + rotationToApply < -maxVerticalAngle)
        {
            fpsCameraPivot.Rotate(Vector3.right, -(currentAngle + maxVerticalAngle));
        }
        else
        {
            fpsCameraPivot.Rotate(Vector3.right, rotationToApply);
        }
    }

    private void OnDisable()
    {
        aim.action.Disable();
    }
}