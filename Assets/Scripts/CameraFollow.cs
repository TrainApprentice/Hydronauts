using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private PlayerMain playerRef;
    private Vector3 goalPosition;
    private bool targetIsPlayer = true;

    private float shakeTimer = 0f;
    private float shakeAmount = 0f;
    private float shakeFalloff = 0;
    private float shakeStartTime = 0;
    private Vector3 shakeBase;

    private void Start()
    {
        playerRef = FindObjectOfType<PlayerMain>();
        goalPosition = playerRef.transform.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        goalPosition.z = -10;
        // If the target is the player, follow them
        if (targetIsPlayer)
        {
            goalPosition = playerRef.transform.position - new Vector3(0, 0, 10);
            float distanceToPlayer = Vector3.Distance(transform.position, goalPosition);
            if (distanceToPlayer > 3) transform.position = AnimMath.Ease(transform.position, goalPosition, .01f);
        }
        // Otherwise, keep the camera at the determined target
        else
        {
            if (shakeTimer > 0) transform.position = AnimMath.Ease(goalPosition, shakeBase, .001f);
            else transform.position = AnimMath.Ease(transform.position, goalPosition, .001f);
            
        }
        // If the camera is supposed to shake, shake it
        if(shakeTimer > 0) UpdateShake();
    }
    /// <summary>
    /// Set the camera's target to a position, and if it's the player, update that accordingly
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="targetPlayer"></param>
    public void SetNewTarget(Vector3 newPos, bool targetPlayer = false)
    {
        goalPosition = newPos;
        targetIsPlayer = targetPlayer;
    }
    /// <summary>
    /// Shake the camera for its set time and at the set intensity
    /// </summary>
    void UpdateShake()
    {
        shakeTimer -= Time.deltaTime;
        shakeFalloff = AnimMath.Map(shakeTimer, shakeStartTime, 0, 1, 0f);

        Vector3 offset = new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f)) * shakeAmount;

        goalPosition += offset * shakeFalloff;

        if (shakeTimer <= 0) goalPosition = shakeBase;
    }
    /// <summary>
    /// Set the camera's shake values at a certain intensity for a certain length of time
    /// </summary>
    /// <param name="time"></param>
    /// <param name="shakeAmt"></param>
    public void Shake(float time, float shakeAmt)
    {
        if (time > shakeTimer) shakeTimer = time;
        shakeAmount = shakeAmt;
        shakeBase = transform.position;
        shakeFalloff = 1;
        shakeStartTime = time;
    }

    
}
