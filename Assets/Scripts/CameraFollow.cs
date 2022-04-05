using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 goalPosition;
    private bool targetIsPlayer = true;

    private float shakeTimer = 0f;
    private float shakeAmount = 0f;
    private float shakeFalloff = 0;
    private float shakeStartTime = 0;
    private Vector3 shakeBase;


    // Update is called once per frame
    void LateUpdate()
    {
        goalPosition.z = -10;
        if (targetIsPlayer)
        {
            
            float distanceToPlayer = Vector3.Distance(transform.position, goalPosition);
            if (distanceToPlayer > 3) transform.position = AnimMath.Ease(transform.position, goalPosition, .01f);
        }
        else
        {
            if (shakeTimer > 0) transform.position = AnimMath.Ease(goalPosition, shakeBase, .001f);
            else transform.position = AnimMath.Ease(transform.position, goalPosition, .001f);
            
        }
        if(shakeTimer > 0) UpdateShake();
    }

    public void SetNewTarget(Vector3 newPos, bool targetPlayer = false)
    {
        goalPosition = newPos;
        targetIsPlayer = targetPlayer;
    }
    void UpdateShake()
    {
        shakeTimer -= Time.deltaTime;
        shakeFalloff = AnimMath.Map(shakeTimer, shakeStartTime, 0, 1, 0f);
        print(shakeFalloff);

        Vector3 offset = new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f)) * shakeAmount;

        goalPosition += offset * shakeFalloff;

        if (shakeTimer <= 0) goalPosition = shakeBase;
    }

    public void Shake(float time, float shakeAmt)
    {
        if (time > shakeTimer) shakeTimer = time;
        shakeAmount = shakeAmt;
        shakeBase = transform.position;
        shakeFalloff = 1;
        shakeStartTime = time;
    }

    
}
