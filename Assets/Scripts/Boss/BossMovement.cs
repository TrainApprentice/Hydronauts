using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public JointStorage leftShoulder, rightShoulder, leftHip, rightHip, leftElbow, rightElbow, leftKnee, rightKnee, baseSkeleton, baseBody;

    public AudioClip flames, groundPunch, slam;
    private AudioSource sound;
    private bool doSoundOnce = true;

    private float animIdleTimer = 0;
    private float animWalkTimer = 0;
    private float animRushTimer = 0;
    private float animDeathTimer = 0;

    private int punchCounter = 0;
    private bool addPunch = true;
    private bool doShake = true;

    private float walkTimer = 0f;
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

        sound = GetComponent<AudioSource>();

        // Move the boss into starting position for the fight
        SetNewLocation(controller.fireAttackPositions[2].position + new Vector3(2, 0), true, 0, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isDead)
        {
            // Moves the boss from point to point, based on what's set in SetNewLocation
            if (currLocation != nextLocation)
            {
                if (walkTimer < 1) walkTimer += (slowMovement) ? Time.deltaTime / 2 : Time.deltaTime;
                else walkTimer = 1;
                currLocation = AnimMath.Lerp(prevLocation, nextLocation, walkTimer);

            }
            else walkTimer = 1;
            transform.position = currLocation;

            // Determines which animation to play, based on which attack is being done
            if (walkTimer == 1 || walkTimer == 0)
            {
                if (doingSlam) AnimSlam();
                else if (doingShockwave) AnimShockwave();
                else if (doingFlames) AnimFlamethrower();
                else AnimIdle();
            }
            if (doingRush) AnimRush();
            else if (walkTimer != 0 && walkTimer != 1) AnimWalk();
            bool test = (currLocation == nextLocation);
            Debug.LogError(test + ", Walk: " + walkTimer);
            //Debug.LogError("Walk: " + walkTimer + ",\n Slam: " + doingSlam + ",\n Shock: " + doingShockwave + ",\n Flame: " + doingFlames + ",\n Rush: " + doingRush);
        }
        else AnimDeath();
    }

    /// <summary>
    /// This sets a new location for the boss to move to, setting the newLocation and prevLocation for the movement
    /// Allows for various inputs to determine if this is slower movement, movement for an attack, or it should reset the walk timer
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="doSlow"></param>
    /// <param name="attackType"></param>
    /// <param name="resetWalk"></param>
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

    /// <summary>
    /// Sets doingRush to true, which will trigger a rush attack
    /// </summary>
    public void BeginRush()
    {
        doingRush = true;
    }
    /// <summary>
    /// Runs the animation for the slam attack, as well as the hitbox
    /// </summary>
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
                if(doSoundOnce)
                {
                    sound.clip = slam;
                    sound.Play();
                    doSoundOnce = false;
                }
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
                    sound.clip = groundPunch;
                    sound.Play();
                    doSoundOnce = false;
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
    /// <summary>
    /// Runs the animation for the shockwave attack, as well as indicating when to summon debris
    /// </summary>
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
                    sound.clip = groundPunch;
                    sound.volume = .4f;
                    sound.Play();
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
    /// <summary>
    /// Runs the animation for the rush attack, as well as the hitbox
    /// </summary>
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

    /// <summary>
    /// Runs the animation for the flamethrower attack, determining what to move based on which pattern is being used
    /// </summary>
    private void AnimFlamethrower()
    {
        if(flameAttackDuration == 0) controller.isAttackingFire = true;
        flameAttackDuration += Time.deltaTime;
        if(flameAttackDuration > 2 || !controller.isAttackingFire)
        {
            walkTimer = 0;
            doingFlames = false;
            SetNewLocation(prevLocation);
            flameAttackDuration = 0;
        }
        else
        {
            float rightElbowGoalRot = rightElbow.startRot.eulerAngles.z;
            float leftElbowGoalRot = leftElbow.startRot.eulerAngles.z;
            float rightShoulderGoalRot = rightShoulder.startRot.eulerAngles.z;
            if(doSoundOnce)
            {
                sound.clip = flames;
                sound.volume = .6f;
                sound.Play();
                doSoundOnce = false;
            }
            if (currFlamePattern == 1)
            {
                leftElbowGoalRot = 10;
            }
            else if (currFlamePattern == 2)
            {
                rightShoulderGoalRot = 0;
                rightElbowGoalRot = -15;
            }
            else if (currFlamePattern == 3)
            {
                rightShoulderGoalRot = 0;
                rightElbowGoalRot = -15;
                leftElbowGoalRot = 10;
            }

            rightShoulder.EaseToNewRotation(rightShoulderGoalRot, .001f);
            rightElbow.EaseToNewRotation(rightElbowGoalRot, .001f);
            leftElbow.EaseToNewRotation(leftElbowGoalRot, .001f);
        }
    }
    /// <summary>
    /// Runs the idle animation and resets various animation timers
    /// </summary>
    private void AnimIdle()
    {
        doSoundOnce = true;
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
    /// <summary>
    /// Runs the walk animation
    /// </summary>
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
    /// <summary>
    /// Runs the death animation
    /// </summary>
    private void AnimDeath()
    {
        animDeathTimer += Time.deltaTime;
        
        if(animDeathTimer > 1)
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
            float baseSkeletonGoalRot = baseSkeleton.startRot.eulerAngles.z;

            Vector3 leftShoulderGoalPos = leftShoulder.startPos;
            Vector3 rightShoulderGoalPos = rightShoulder.startPos;
            Vector3 leftHipGoalPos = leftHip.startPos;
            Vector3 rightHipGoalPos = rightHip.startPos;
            Vector3 baseBodyGoalPos = baseBody.startPos;
            Vector3 baseSkeletonGoalPos = baseSkeleton.startPos;

            float moveTimer = 0;

            if (animDeathTimer < 2.5f)
            {
                baseBodyGoalPos = new Vector3(1.6f, -2.12f, 0);
                baseSkeletonGoalPos = new Vector3(1.9f, -1.71f);

                leftShoulderGoalRot = -80;
                leftElbowGoalRot = -65;
                rightShoulderGoalRot = -10;
                rightElbowGoalRot = -50;

                leftHipGoalRot = -55;
                leftKneeGoalRot = -30;
                rightHipGoalRot = -83;
                rightKneeGoalRot = 2;

                moveTimer = .01f;
            }
            else
            {
                GameManager.instance.StartEndCutscene();
                baseSkeletonGoalPos = new Vector3(4, -4);
                baseSkeletonGoalRot = -90;

                baseBodyGoalPos = new Vector3(3.85f, -4.3f);
                baseBodyGoalRot = -90;

                rightShoulderGoalPos = new Vector3(2.6f, 3.7f);
                rightShoulderGoalRot = 20;
                rightElbowGoalRot = -20;

                leftShoulderGoalRot = -60;
                leftElbowGoalRot = -110;

                leftKneeGoalRot = 50;

                rightHipGoalPos = new Vector3(.15f, -3.77f);
                rightKneeGoalRot = 35;
                moveTimer = .01f;
            }

            baseSkeleton.EaseToNewPosition(baseSkeletonGoalPos, moveTimer);
            baseSkeleton.EaseToNewRotation(baseSkeletonGoalRot, moveTimer);

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
    /// <summary>
    /// A function to reset any or all animation timers used by the various animation methods
    /// </summary>
    /// <param name="currAnim"></param>
    void ResetTimers(string currAnim = "none")
    {
        animIdleTimer = (currAnim == "IDLE") ? animIdleTimer : 0;
        animWalkTimer = (currAnim == "WALK") ? animWalkTimer : 0;
    }

    
}
