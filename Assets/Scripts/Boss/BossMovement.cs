using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public Transform leftShoulder, rightShoulder, leftHip, rightHip, leftElbow, rightElbow, leftKnee, rightKnee, baseSkeleton, baseBody;
    private float animIdleTimer = 0;
    private float animWalkTimer = 0;

    private float walkTimer = 0f;
    private float timeBetweenMovements = 2f;
    private bool slowMovement = false;
    private Transform playerRef;
    private BossAI controller;

    private Vector3 currLocation;
    private Vector3 nextLocation;
    private Vector3 prevLocation;

    private bool doingSlam = false;
    private bool doingShockwave = false;
    private bool doingRush = false;
    private bool doingFlames = false;
    private int currFlamePattern = 0;
    private bool setRush = false;
    private float slamAttackDuration = 0;
    private float shockwaveAttackDuration = 0;
    private float rushAttackDuration = 0;
    private float flameAttackDuration = 0;

    // Start is called before the first frame update
    void Start()
    {
        currLocation = transform.position;
        prevLocation = currLocation;
        nextLocation = currLocation;
        playerRef = FindObjectOfType<PlayerMain>().transform;
        controller = GetComponent<BossAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currLocation != nextLocation)
        {
            if (walkTimer < 1) walkTimer += (slowMovement) ? Time.deltaTime / 2 : Time.deltaTime;
            else walkTimer = 1;
            currLocation = AnimMath.Lerp(prevLocation, nextLocation, walkTimer);
        }
        transform.position = currLocation;

        if(walkTimer == 1 || walkTimer == 0)
        {
            if (doingSlam) AnimSlam();
            else if (doingShockwave) AnimShockwave();
            else if (doingFlames) AnimFlamethrower();
            else AnimIdle();
        }
        if (doingRush) AnimRush();

        if (walkTimer != 0 && walkTimer != 1) AnimWalk();
    }

    public void SetNewLocation(Vector3 newPos, int attackType = 0, bool resetWalk = true)
    {
        if(resetWalk) walkTimer = 0;
        nextLocation = newPos;
        prevLocation = currLocation;
        

        if(attackType > 0)
        {
            switch(attackType)
            {
                case 1:
                    doingSlam = true;
                    break;
                case 2:
                    doingShockwave = true;
                    break;
                default:
                    doingFlames = true;
                    currFlamePattern = attackType - 5;
                    break;
            }
        }
    }

    public void BeginRush()
    {
        doingRush = true;
    }
    private void AnimSlam()
    {
        slamAttackDuration += Time.deltaTime;
        print("SLAMMING");
        if (slamAttackDuration > 1)
        {
            walkTimer = 0;
            doingSlam = false;
            SetNewLocation(prevLocation);
            slamAttackDuration = 0;
            controller.isAttackingSlam = false;
        }
    }
    private void AnimShockwave()
    {
        shockwaveAttackDuration += Time.deltaTime;
        print("SHOCKWAVE");
        if(shockwaveAttackDuration > 1)
        {
            walkTimer = 0;
            doingShockwave = false;
            SetNewLocation(prevLocation);
            shockwaveAttackDuration = 0;
            controller.isAttackingShockwave = false;
        }
    }

    private void AnimRush()
    {
        rushAttackDuration += Time.deltaTime;
        print("RUSHING");
        if(rushAttackDuration < 2)
        {
            Vector3 readyPos = new Vector3(transform.position.x, playerRef.position.y + 2f);
            SetNewLocation(readyPos, 0, false);
        }
        else
        {
            if(!setRush)
            {
                SetNewLocation(transform.position + new Vector3(30, 0, 0));
                setRush = true;
            }
        }
        if(rushAttackDuration > 3)
        {
            walkTimer = 0;
            doingRush = false;
            setRush = false;
            SetNewLocation(prevLocation);
            controller.isAttackingRush = false;
            rushAttackDuration = 0;
            slowMovement = true;
        }
    }
    private void AnimFlamethrower()
    {
        if(flameAttackDuration == 0) controller.isAttackingFire = true;
        flameAttackDuration += Time.deltaTime;
        if(currFlamePattern == 1)
        {
            
        }
        else if(currFlamePattern == 2)
        {
            
        }
        else if(currFlamePattern == 3)
        {
            
        }
        if(flameAttackDuration > 2 || !controller.isAttackingFire)
        {
            walkTimer = 0;
            doingFlames = false;
            SetNewLocation(prevLocation);
            flameAttackDuration = 0;
        }
    }

    private void AnimIdle()
    {
        animIdleTimer += Time.deltaTime;
        float wave = Mathf.Sin(animIdleTimer);

        float elbowWave = wave * 5;

        leftElbow.localRotation = Quaternion.Euler(0, 0, elbowWave);
        rightElbow.localRotation = Quaternion.Euler(0, 0, elbowWave + 15);

        float hipWavePos = wave * .15f - .15f;
        float hipWaveRot = wave * 10;

        rightHip.localPosition = new Vector3(-1.61f, hipWavePos - 1);
        rightHip.localRotation = Quaternion.Euler(0, 0, hipWaveRot - 30);

        leftHip.localPosition = new Vector3(.77f, hipWavePos - 1.45f);
        leftHip.localRotation = Quaternion.Euler(0, 0, hipWaveRot - 15f);

        float kneeWave = wave * -15f;

        rightKnee.localRotation = Quaternion.Euler(0, 0, kneeWave + 20f);
        leftKnee.localRotation = Quaternion.Euler(0, 0, kneeWave - 10);

        float bodyWave = wave * -.2f;
        baseSkeleton.localPosition = new Vector3(0, bodyWave, 0);
        baseBody.localPosition = new Vector3(0, bodyWave / 2, 0);
    }

    private void AnimWalk()
    {

    }
}
