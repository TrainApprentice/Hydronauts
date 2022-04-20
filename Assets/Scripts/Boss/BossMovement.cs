using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public JointStorage leftShoulder, rightShoulder, leftHip, rightHip, leftElbow, rightElbow, leftKnee, rightKnee, baseSkeleton, baseBody;

    private float animIdleTimer = 0;
    private float animWalkTimer = 0;
    private float animRushTimer = 0;
    private float animDeathTimer = 0;

    private int punchCounter = 0;
    private bool addPunch = true;
    private bool doShake = true;

    private float walkTimer = 0f;
    private float timeBetweenMovements = 2f;
    private bool slowMovement = false;
    private Transform playerRef;
    private BossAI controller;
    private CameraFollow cam;

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
        cam = FindObjectOfType<CameraFollow>();

        SetNewLocation(controller.fireAttackPositions[2].position + new Vector3(2, 0), true, 0, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isDead)
        {
            if (currLocation != nextLocation)
            {
                if (walkTimer < 1) walkTimer += (slowMovement) ? Time.deltaTime / 2 : Time.deltaTime;
                else walkTimer = 1;
                currLocation = AnimMath.Lerp(prevLocation, nextLocation, walkTimer);
            }
            transform.position = currLocation;

            if (walkTimer == 1 || walkTimer == 0)
            {
                if (doingSlam) AnimSlam();
                else if (doingShockwave) AnimShockwave();
                else if (doingFlames) AnimFlamethrower();
                else AnimIdle();
            }
            if (doingRush) AnimRush();
            else if (walkTimer != 0 && walkTimer != 1) AnimWalk();
        }
        else AnimDeath();
    }

    public void SetNewLocation(Vector3 newPos, bool doSlow = false, int attackType = 0, bool resetWalk = true)
    {
        if(resetWalk) walkTimer = 0;
        slowMovement = doSlow;
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

                leftHipGoalPos = new Vector3(.82f, -3.17f);
                leftHipGoalRot = 0;

                leftKneeGoalRot = -22;

                rightHipGoalPos = new Vector3(-.83f, -1.29f);
                rightHipGoalRot = -73;

                rightKneeGoalRot = 25;

                leftShoulderGoalPos = new Vector3(3.32f, -1.33f);
                leftShoulderGoalRot = -75;

                leftElbowGoalRot = -30;

                rightShoulderGoalPos = new Vector3(2, -1.5f);
                rightShoulderGoalRot = 35;

                rightElbowGoalRot = -60;
                moveTimer = .000001f;

                if(doShake && Vector3.Distance(baseBody.transform.localPosition, baseBodyGoalPos) < .05f)
                {
                    cam.Shake(.2f, 2);
                    doShake = false;
                }
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

                doShake = true;
            }
            

            
            if (slamAttackDuration > 1.2f)
            {
                controller.SlamHitboxSwap(false);
            }
            else if (slamAttackDuration > .8f)
            {
                controller.SlamHitboxSwap(true);
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
        
       
        if(shockwaveAttackDuration > 5)
        {
            walkTimer = 0;
            doingShockwave = false;
            SetNewLocation(prevLocation);
            shockwaveAttackDuration = 0;
            controller.isAttackingShockwave = false;
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

            if (shockwaveAttackDuration < 1)
            {
                baseBodyGoalPos = new Vector3(0, -.73f);
                baseBodyGoalRot = -50;

                leftShoulderGoalPos = new Vector3(1.72f, -.41f);
                leftShoulderGoalRot = -145;
                leftElbowGoalRot = 45;

                rightShoulderGoalPos = new Vector3(.53f, .49f);
                rightShoulderGoalRot = -70;
                rightElbowGoalRot = 30;

                leftHipGoalPos = new Vector3(0, -2.17f);
                leftHipGoalRot = -13;
                leftKneeGoalRot = -32;

                rightHipGoalPos = new Vector3(-2.5f, -1.07f);
                rightHipGoalRot = -75;
                rightKneeGoalRot = 25;
                moveTimer = .001f;

            }
            else if (shockwaveAttackDuration < 4.5f)
            {
                
                float checkTime = Mathf.Floor(shockwaveAttackDuration * 10);
                if (checkTime % 5f == 0 && addPunch)
                {
                    punchCounter++;
                    addPunch = false;
                    cam.Shake(.1f, 1);
                    controller.SummonDebris();
                }
                if (checkTime % 5 != 0) addPunch = true;
                if (punchCounter % 2 == 0)
                {
                    rightShoulderGoalRot = -70;
                    rightElbowGoalRot = 30;

                    leftShoulderGoalRot = -60;
                    leftElbowGoalRot = -70;
                }
                else
                {
                    rightShoulderGoalRot = 25;
                    rightElbowGoalRot = -90;

                    leftShoulderGoalRot = -145;
                    leftElbowGoalRot = 45;
                }
                baseBodyGoalPos = new Vector3(0, -.73f);
                baseBodyGoalRot = -50;
                leftShoulderGoalPos = new Vector3(1.72f, -.41f);
                rightShoulderGoalPos = new Vector3(.53f, .49f);

                leftHipGoalPos = new Vector3(0, -2.17f);
                leftHipGoalRot = -13;
                leftKneeGoalRot = -32;

                rightHipGoalPos = new Vector3(-2.5f, -1.07f);
                rightHipGoalRot = -75;
                rightKneeGoalRot = 25;

                moveTimer = .0001f;

            }
            else
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
    private void AnimRush()
    {
        rushAttackDuration += Time.deltaTime;
        if(rushAttackDuration < 2)
        {
            Vector3 readyPos = new Vector3(transform.position.x, playerRef.position.y + 2f);
            SetNewLocation(readyPos, false, 0, false);
        }
        else if(rushAttackDuration > 3)
        {
            if(!setRush)
            {
                SetNewLocation(transform.position + new Vector3(30, 0, 0));
                setRush = true;
            }
        }
        if(rushAttackDuration > 4)
        {
            walkTimer = 0;
            doingRush = false;
            setRush = false;
            SetNewLocation(prevLocation);
            controller.isAttackingRush = false;
            rushAttackDuration = 0;
            slowMovement = true;
            controller.SlamHitboxSwap(false);
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
            float moveTimer = 0;

            if(rushAttackDuration < 2)
            {
                baseBodyGoalRot = -30;

                leftShoulderGoalPos = new Vector3(1.52f, .87f);
                leftShoulderGoalRot = -125;
                leftElbowGoalRot = 35;

                rightShoulderGoalPos = new Vector3(-.48f, 1.5f);
                rightShoulderGoalRot = -25;
                rightElbowGoalRot = 30;

                animRushTimer += Time.deltaTime;

                float wave = Mathf.Sin(animRushTimer * 7);
                float offsetWave = Mathf.Sin((animRushTimer + Mathf.PI) * 7);

                leftHipGoalRot = wave * 25 - 5;
                rightHipGoalRot = offsetWave * 25 - 15;

                leftKneeGoalRot = wave * 15 - 25;
                rightKneeGoalRot = offsetWave * 15 + 20;

                moveTimer = .1f;
            }
            else if(rushAttackDuration < 3)
            {
                baseBodyGoalRot = -30;

                leftShoulderGoalPos = new Vector3(1.52f, .87f);
                leftShoulderGoalRot = -125;
                leftElbowGoalRot = 35;

                rightShoulderGoalPos = new Vector3(-.48f, 1.5f);
                rightShoulderGoalRot = -25;
                rightElbowGoalRot = 30;

                moveTimer = .01f;
            }
            else
            {
                baseBodyGoalRot = -30;

                leftShoulderGoalPos = new Vector3(1.52f, .87f);
                leftShoulderGoalRot = -125;
                leftElbowGoalRot = 35;

                rightShoulderGoalPos = new Vector3(-.48f, 1.5f);
                rightShoulderGoalRot = -25;
                rightElbowGoalRot = 30;

                leftHipGoalPos = new Vector3(.33f, -1.58f);
                leftHipGoalRot = -50;
                leftKneeGoalRot = -35;

                rightHipGoalPos = new Vector3(-1.34f, -1.04f);
                rightHipGoalRot = -55;
                rightKneeGoalRot = -25;

                moveTimer = .0001f;
                controller.SlamHitboxSwap(true);
            }
            
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

        rightHip.EaseToNewPosition(new Vector3(-1.91f, hipWavePos - 1.47f), .0001f);
        rightHip.EaseToNewRotation(hipWaveRot - 80, .0001f);

        leftHip.EaseToNewPosition(new Vector3(1f, hipWavePos - 2.03f), .0001f);
        leftHip.EaseToNewRotation(hipWaveRot - 55f, .0001f);

        float kneeWave = wave * -15f;

        rightKnee.EaseToNewRotation(kneeWave + 75f, .0001f);
        leftKnee.EaseToNewRotation(kneeWave + 40, .0001f);

        float bodyWave = wave * -.2f;
        baseSkeleton.EaseToNewPosition(new Vector3(0, bodyWave, 0), .0001f);
        baseBody.EaseToNewPosition(new Vector3(0, bodyWave / 2, 0), .0001f);
        
        
    }
    private void AnimWalk()
    {
        animWalkTimer += Time.deltaTime;
        
        float wave = Mathf.Sin(animWalkTimer * 7);
        float offsetWave = Mathf.Sin((animWalkTimer + Mathf.PI) * 7);

        
        float leftHipWave = wave * 25 - 50;
        float rightHipWave = offsetWave * 25 - 55;

        leftHip.EaseToNewRotation(leftHipWave, .0001f);
        rightHip.EaseToNewRotation(rightHipWave, .0001f);

        float leftKneeWave = wave * 15 + 40;
        float rightKneeWave = offsetWave * 15 + 45;

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
    private void AnimDeath()
    {
        animDeathTimer += Time.deltaTime;
        if(animDeathTimer > 5)
        {
            GameManager.instance.StartEndCutscene();  
        }
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
