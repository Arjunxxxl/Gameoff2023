using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform pivot;
    [SerializeField] private Camera cam;

    [Header("Follow Data")]
    [SerializeField] private Vector3 followDestination;
    [SerializeField] private float followSpeed;

    [Header("Rotation Data")]
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private float rotationSpeed;

    [Header("Pivot Offset Data")]
    [SerializeField] private Vector3 pivotOffSet;
    [SerializeField] private float pivotMoveSpeed;

    [Header("Pivot Rotation Data")]
    [SerializeField] private float minXRotationAngle;
    [SerializeField] private float maxXRotationAngle;
    [SerializeField] private Vector3 pivotTargetRotation;
    [SerializeField] private float pivotRotationSpeed;

    UserInput userInput;

    // Start is called before the first frame update
    void Start()
    {
        userInput = UserInput.Instance;

        pivotTargetRotation = pivot.localRotation.eulerAngles;
    }

    void LateUpdate()
    {
        Follow();
        Rotate();

        UpdatePivotPos();
        RotatePivot();
    }

    #region Follow/Rotate
    private void Follow()
    {
        followDestination = target.position;
        transform.position = Vector3.MoveTowards(transform.position, followDestination, Time.deltaTime * followSpeed);
    }

    private void Rotate()
    {
        targetRotation = target.rotation.eulerAngles;
        targetRotation.x = 0;
        targetRotation.z = 0;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotationSpeed);
    }
    #endregion

    #region Pivot
    private void UpdatePivotPos()
    {
        pivot.localPosition = Vector3.MoveTowards(pivot.localPosition, pivotOffSet, Time.deltaTime * pivotMoveSpeed);
    }

    private void RotatePivot()
    {
        //pivotTargetRotation = pivot.localRotation.eulerAngles;
        pivotTargetRotation.x += userInput.MouseInputDir.y * -1;

        if(pivotTargetRotation.x < minXRotationAngle)
        {
            pivotTargetRotation.x = minXRotationAngle;
        }

        if (pivotTargetRotation.x > maxXRotationAngle)
        {
            pivotTargetRotation.x = maxXRotationAngle;
        }

        pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(pivotTargetRotation), Time.deltaTime * pivotRotationSpeed);
    }
    #endregion
}
