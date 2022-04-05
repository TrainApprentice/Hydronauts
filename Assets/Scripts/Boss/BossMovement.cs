using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public JointStorage leftShoulder, rightShoulder, leftHip, rightHip, leftElbow, rightElbow, leftKnee, rightKnee, baseSkeleton, baseBody;
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
        if (slamAttackDuration > 3f)
        {
            walkTimer = 0;
            doingSlam = false;
            SetNewLocation(prevLocation);
            slamAttackDuration = 0;
            controller.isAttackingSlam = false;
            ResetTimers();
        }
        else
        {
            float leftShoulderGoalRot = leftShoulder.startRot.eulerAngles.z;
            float rightShoulderGoalRot = rightShoulder.startRot.eulerAngles.z;
            float leftElbowGoalRot = leftElbow.startRot.eulerAngles.z;
            float rightElbowGoalRot = rightElbow.startRot.eulerAngles.z;
            float leftHipGoalRot = leftHip.startRot.eulerAngles.z;
            float rightHipGoalRot = rightHip.startRot.eulerAngles.z;
            float leftKneeGoalRot = leftKnee.startRot.eulerAngles.z;
            float rightKneeGoalRot = rightKnee.startRot.eulerAngles.z;
            float baseBodyGoalRot = baseBody.startRot.eulerAngles.z;

            Vector3 leftShoulderGoalPos = leftShoulder.startPos;
            Vector3 rightShoulderGoalPos = rightShoulder.startPos;
            Vector3 leftHipGoalPos = leftHip.startPos;
            Vector3 rightHipGoalPos = rightHip.startPos;
            Vector3 baseBodyGoalPos = baseBody.startPos;

            float moveTimer = 0;

            if (slamAttackDuration < .7f)
            {
                baseBodyGoalRot = 10;
                baseBodyGoalPos = new Vector3(-.57f, .14f);

                leftShoulderGoalPos = new Vector3(.46f, 1.48f);
                leftShoulderGoalRot = 100;

                leftElbowGoalRot = -60;

                rightShoulderGoalPos = new Vector3(-2f, 1.44f);
                rightShoulderGoalRot = 160;

                rightElbowGoalRot = -60;
                moveTimer = .01f;
            }
            else if(slamAttackDuration < 2.5f)
            {
                baseBodyGoalRot = -60;
                baseBodyGoalPos = new Vector3(1.58f, -1.43f);

                leftHipGoalPos = new Vector3(.43f, -2.43f);
                leftHipGoalRot = 13;

                leftKneeGoalRot = -40;

                rightHipGoalPos = new Vector3(0, -2,17f);
                rightHipGoalRot = -12;

                rightKneeGoalRot = -25;

                leftShoulderGoalPos = new Vector3(3.32f, -1.33f);
                leftShoulderGoalRot = -75;

                leftElbowGoalRot = -30;

                rightShoulderGoalPos = new Vector3(2, -1.5f);
                rightShoulderGoalRot = 35;

                rightElbowGoalRot = -60;
                moveTimer = .0001f;
            }
            else if(slamAttackDuration > 2.5f)
            {
                leftShoulderGoalRot = leftShoulder.startRot.eulerAngles.z;
                rightShoulderGoalRot = rightShoulder.startRot.eulerAngles.z;
                leftElbowGoalRot = leftElbow.startRot.eulerAngles.z;
                rightElbowGoalRot = rightElbow.startRot.eulerAngles.z;
                leftHipGoalRot = leftHip.startRot.eulerAngles.z;
                rightHipGoalRot = rightHip.startRot.eulerAngles.z;
                leftKneeGoalRot = leftKnee.startRot.eulerAngles.z;
                rightKneeGoalRot = rightKnee.startRot.eulerAngles.z;
                baseBodyGoalRot = baseBody.startRot.eulerAngles.z;

                leftShoulderGoalPos = leftShoulder.startPos;
                rightShoulderGoalPos = rightShoulder.startPos;
                leftHipGoalPos = leftHip.startPos;
                rightHipGoalPos = rightHip.startPos;
                baseBodyGoalPos = baseBody.startPos;
                moveTimer = .001f;
            }
            

            
            baseBody.EaseToNewPosition(baseBodyGoalPos, moveTimer);
            baseBody.EaseToNewRotation(baseBodyGoalRot, moveTimer);

            leftHip.EaseToNewPosition(leftHipGoalPos, moveTimer);
            leftHip.EaseToNewRotation(leftHipGoalRot, moveTimer);

            leftKnee.EaseToNewRotation(leftKneeGoalRot, moveTimer);

            rightHip.EaseToNewPosition(rightHipGoalPos, moveTimer);
            rightHip.EaseToNewRotation(rightHipGoalRot, moveTimer);

            rightKnee.EaseToNewRotation(rightKneeGoalRot, moveTimer);

            leftShoulder.EaseToNewPosition(leftShoulderGoalPos, moveTimer);
            leftShoulder.EaseToNewRotation(leftShoulderGoalRot, moveTimer);

            leftElbow.EaseToNewRotation(leftElbowGoalRot, moveTimer);

            rightShoulder.EaseToNewPosition(rightShoulderGoalPos, moveTimer);
            rightShoulder.EaseToNewRotation(rightShoulderGoalRot, moveTimer);

            rightElbow.EaseToNewRotation(rightElbowGoalRot, moveTimer);
            
          
        }
    }
    private void AnimShockwave()
    {
        ResetTimers();
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
            AnimWalk();
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
        ResetTimers("IDLE");
        animIdleTimer += Time.deltaTime;
        
        float wave = Mathf.Sin(animIdleTimer);

        float elbowWave = wave * 5;

        leftElbow.EaseToNewRotation(elbowWave, .0001f);
        rightElbow.EaseToNewRotation(elbowWave + 15, .0001f);

        float hipWavePos = wave * .15f - .15f;
        float hipWaveRot = wave * 10;

        rightHip.EaseToNewPosition(new Vector3(-1.61f, hipWavePos - 1), .0001f);
        rightHip.EaseToNewRotation(hipWaveRot - 30, .0001f);

        leftHip.EaseToNewPosition(new Vector3(.77f, hipWavePos - 1.45f), .0001f);
        leftHip.EaseToNewRotation(hipWaveRot - 15f, .0001f);

        float kneeWave = wave * -15f;

        rightKnee.EaseToNewRotation(kneeWave + 20f, .0001f);
        leftKnee.EaseToNewRotation(kneeWave - 10, .0001f);

        float bodyWave = wave * -.2f;
        baseSkeleton.EaseToNewPosition(new Vector3(0, bodyWave, 0), .0001f);
        baseBody.EaseToNewPosition(new Vector3(0, bodyWave / 2, 0), .0001f);
        
        
    }

    private void AnimWalk()
    {
        animWalkTimer += Time.deltaTime;
        
        float wave = Mathf.Sin(animWalkTimer * 7);
        float offsetWave = Mathf.Sin((animWalkTimer + Mathf.PI) * 7);

        
        float leftHipWave = wave * 25 - 5;
        float rightHipWave = offsetWave * 25 - 15;

        leftHip.EaseToNewRotation(leftHipWave, .0001f);
        rightHip.EaseToNewRotation(rightHipWave, .0001f);

        float leftKneeWave = wave * 15 - 25;
        float rightKneeWave = offsetWave * 15 + 20;

        leftKnee.EaseToNewRotation(leftKneeWave, .0001f);
        rightKnee.EaseToNewRotation(rightKneeWave, .0001f);

        float leftShoulderWave = wave * 5 - 75;
        float rightShoulderWave = offsetWave * 5 - 15;

        leftShoulder.EaseToNewRotation(leftShoulderWave, .0001f);
        rightShoulder.EaseToNewRotation(rightShoulderWave, .0001f);

        float leftElbowWave = wave * 10 - 50;
        float rightElbowWave = offsetWave * 10 - 50;

        leftElbow.EaseToNewRotation(leftElbowWave, .0001f);
        rightElbow.EaseToNewRotation(rightElbowWave, .0001f);
        
        
    }
    void ResetTimers(string currAnim = "none")
    {
        animIdleTimer = (currAnim == "IDLE") ? animIdleTimer : 0;
        animWalkTimer = (currAnim == "WALK") ? animWalkTimer : 0;
    }

    void EaseAllJointsToStart(float timer)
    {
        baseSkeleton.EaseToStartPosition(timer);
        baseSkeleton.EaseToStartRotation(timer);
        baseBody.EaseToStartPosition(timer);
        baseBody.EaseToStartRotation(timer);

        leftShoulder.EaseToStartPosition(timer);
        leftShoulder.EaseToStartRotation(timer);
        leftElbow.EaseToStartPosition(timer);
        leftElbow.EaseToStartRotation(timer);
        leftHip.EaseToStartPosition(timer);
        leftHip.EaseToStartRotation(timer);
        leftKnee.EaseToStartPosition(timer);
        leftKnee.EaseToStartRotation(timer);

        rightShoulder.EaseToStartPosition(timer);
        rightShoulder.EaseToStartRotation(timer);
        rightElbow.EaseToStartPosition(timer);
        rightElbow.EaseToStartRotation(timer);
        rightHip.EaseToStartPosition(timer);
        rightHip.EaseToStartRotation(timer);
        rightKnee.EaseToStartPosition(timer);
        rightKnee.EaseToStartRotation(timer);
    }
}
