using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointStorage : MonoBehaviour
{
    [HideInInspector]
    public Quaternion startRot;
    [HideInInspector]
    public Vector3 startPos;

    private void Start()
    {
        SetNewStart();
    }

    public void SetCurrentRotation(float zValue)
    {
        transform.localRotation = Quaternion.Euler(0, 0, zValue);
    }
    public void SetCurrentPosition(Vector3 newPos)
    {
        transform.localPosition = newPos;
    }
    public void EaseToStartPosition(float percent)
    {
        transform.localPosition = AnimMath.Ease(transform.localPosition, startPos, percent);
    }
    public void EaseToStartRotation(float percent)
    {
        transform.localRotation = AnimMath.Ease(transform.localRotation, startRot, percent);
    }
    public void EaseToNewPosition(Vector3 newPos, float percent)
    {
        transform.localPosition = AnimMath.Ease(transform.localPosition, newPos, percent);
    }
    public void EaseToNewRotation(float zValue, float percent)
    {
        Quaternion newRot = Quaternion.Euler(0, 0, zValue);
        transform.localRotation = AnimMath.Ease(transform.localRotation, newRot, percent);
    }
    

    public void ResetToStart()
    {
        transform.localRotation = startRot;
        transform.localPosition = startPos;
    }

    public void SetNewStart()
    {
        print("Set!");
        startRot = transform.localRotation;
        startPos = transform.localPosition;
    }
}
