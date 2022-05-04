using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointStorage : MonoBehaviour
{
    /// <summary>
    /// Stores the starting rotation of the joint as a Quaternion
    /// </summary>
    [HideInInspector]
    public Quaternion startRot;

    /// <summary>
    /// Stores the starting position of the joint as a Vector3
    /// </summary>
    [HideInInspector]
    public Vector3 startPos;

    private void Start()
    {
        SetNewStart();
    }

    /// <summary>
    /// Hard sets a new rotation of the joint using a new z value
    /// </summary>
    /// <param name="zValue"></param>
    public void SetCurrentRotation(float zValue)
    {
        transform.localRotation = Quaternion.Euler(0, 0, zValue);
    }
    /// <summary>
    /// Hard sets a new position for the joint using a new Vector3
    /// </summary>
    /// <param name="newPos"></param>
    public void SetCurrentPosition(Vector3 newPos)
    {
        transform.localPosition = newPos;
    }
    /// <summary>
    /// Ease the joint back to its starting position with a given percent left after 1 second
    /// </summary>
    /// <param name="percent"></param>
    public void EaseToStartPosition(float percent)
    {
        transform.localPosition = AnimMath.Ease(transform.localPosition, startPos, percent);
    }
    /// <summary>
    /// Ease the joint back to its starting rotation with a given percent left after 1 second
    /// </summary>
    /// <param name="percent"></param>
    public void EaseToStartRotation(float percent)
    {
        transform.localRotation = AnimMath.Ease(transform.localRotation, startRot, percent);
    }
    /// <summary>
    /// Ease the joint toward a new position with a given percent left after 1 second
    /// </summary>
    /// <param name="percent"></param>
    public void EaseToNewPosition(Vector3 newPos, float percent)
    {
        transform.localPosition = AnimMath.Ease(transform.localPosition, newPos, percent);
    }
    /// <summary>
    /// Ease the joint toward a new rotation with a given percent left after 1 second
    /// </summary>
    /// <param name="percent"></param>
    public void EaseToNewRotation(float zValue, float percent)
    {
        Quaternion newRot = Quaternion.Euler(0, 0, zValue);
        transform.localRotation = AnimMath.Ease(transform.localRotation, newRot, percent);
    }
    
    /// <summary>
    /// Hard set the joint back to its starting rotation and position
    /// </summary>
    public void ResetToStart()
    {
        transform.localRotation = startRot;
        transform.localPosition = startPos;
    }
    /// <summary>
    /// Set a new starting position and rotation for the joint
    /// </summary>
    public void SetNewStart()
    {
        startRot = transform.localRotation;
        startPos = transform.localPosition;
    }
}
