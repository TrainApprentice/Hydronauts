using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public GameObject fireProj;
    public GameObject fallingDebris;
    public Transform fireSpawnLeft, fireSpawnRight;
    public Transform playerRef;
    public GameObject slamHitbox;
    public AudioClip gotHit;

    [HideInInspector]
    public Transform[] fireAttackPositions = new Transform[3];

    private BossUI healthController;
    private AudioSource sound;

    public bool isAttackingFire = false;
    public bool isAttackingSlam = false;
    public bool isAttackingRush = false;
    public bool isAttackingShockwave = false;
    public bool isDead = false;

    public float health = 200;

    private float iFrames = 0;
    private BossMovement mover;
    private Transform shockwavePos;
    private float timeBetweenAttacks = 5f;
    private int currPhase = 1;
    private int flamePattern = 1;
    private float fireAttackTimer = 2f;
    private float timeBetweenFlames = .05f;
    // Start is called before the first frame update
    void Start()
    {
        playerRef = FindObjectOfType<PlayerMain>().transform;
        mover = GetComponent<BossMovement>();
        shockwavePos = FindObjectOfType<ShockwaveID>().transform;
        fireAttackPositions[0] = FindObjectOfType<BottomFire>().transform;
        fireAttackPositions[1] = FindObjectOfType<TopFire>().transform;
        fireAttackPositions[2] = FindObjectOfType<MidFire>().transform;

        healthController = FindObjectOfType<BossUI>();
        //healthController.gameObject.SetActive(true);
        sound = GetComponent<AudioSource>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead)
        {
            if (iFrames > 0) iFrames -= Time.deltaTime;
            else iFrames = 0;
            
            // This runs the state machine, counting down between attacks and randomly determining which one to do based on the current phase
            if (timeBetweenAttacks > 0) timeBetweenAttacks -= Time.deltaTime;
            else
            {
                timeBetweenAttacks = 8f;
                if (currPhase == 1)
                {
                    float randAttack = Random.Range(0f, 1f);

                    if (randAttack < .4f)
                    {
                        FlamethrowerAttack();
                        fireAttackTimer = 2f;
                    }
                    else if (randAttack < .8f)
                    {
                        SlamAttack();
                        isAttackingSlam = true;
                    }
                    else
                    {
                        ShockwaveAttack();
                        isAttackingShockwave = true;
                        timeBetweenAttacks = 10;
                    }


                }
                else if (currPhase == 2)
                {
                    timeBetweenAttacks = 6f;
                    float randAttack = Random.Range(0f, 1f);
                    if (randAttack < .3f)
                    {
                        RushAttack();
                        isAttackingRush = true;
                    }
                    else if (randAttack < .6f)
                    {
                        ShockwaveAttack();
                        isAttackingShockwave = true;
                        timeBetweenAttacks = 8f;
                    }
                    else if (randAttack < .85f)
                    {
                        FlamethrowerAttack();
                        fireAttackTimer = 2f;
                    }
                    else
                    {
                        SlamAttack();
                        isAttackingSlam = true;
                    }

                }
            }
            

            if (isAttackingFire) FlamethrowerPattern(flamePattern);
        }
        
    }
    /// <summary>
    /// This runs the creation of flames for the flamethrower attack, taking in an int to determine which pattern is used
    /// </summary>
    /// <param name="choosePattern"></param>
    public void FlamethrowerPattern(int choosePattern)
    {
        if (timeBetweenFlames > 0) timeBetweenFlames -= Time.deltaTime;
        if(fireAttackTimer > 0) fireAttackTimer -= Time.deltaTime;

        if (fireAttackTimer > 0 && timeBetweenFlames <= 0)
        {
            if (choosePattern == 1)
            {
                GameObject newFire = Instantiate(fireProj, fireSpawnLeft.position, Quaternion.identity);
                newFire.GetComponent<FireProjectile>().angle = (fireAttackTimer < 1) ? 30f - ((1 - fireAttackTimer) * 30f) : 30f;
                
            }
            else if (choosePattern == 2)
            {
                GameObject newFire = Instantiate(fireProj, fireSpawnRight.position, Quaternion.identity);
                newFire.GetComponent<FireProjectile>().angle = (fireAttackTimer < 1) ? -30f + ((1 - fireAttackTimer) * 30f): -30f;
                
            }
            else
            {
                GameObject newFire1 = Instantiate(fireProj, fireSpawnLeft.position, Quaternion.identity);
                newFire1.GetComponent<FireProjectile>().angle = 30f;
                GameObject newFire2 = Instantiate(fireProj, fireSpawnRight.position, Quaternion.identity);
                newFire2.GetComponent<FireProjectile>().angle = -30f;
            }
            
            timeBetweenFlames = .05f;
        }
        if (fireAttackTimer <= 0 && isAttackingFire)
        {
            isAttackingFire = false;
            if (flamePattern < 3) flamePattern++;
            else flamePattern = 1;
            timeBetweenFlames = 0;
        }
        
    }
    /// <summary>
    /// This function is called outside of the class, and is used to decrease the health of the boss by a certain amount
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(float damage)
    {
        if(iFrames == 0)
        {
            health -= damage;
            healthController.SetCurrentHealth(health);

            iFrames = .5f;

            sound.clip = gotHit;
            sound.Play();

            if (health <= 100) currPhase = 2;
            if (health <= 0) isDead = true;
        }
        
    }

    /// <summary>
    ///  Indicates to the BossMovement script to start moving toward a flamethrower position, and ready the flamethrower animation
    /// </summary>
    public void FlamethrowerAttack()
    {
        mover.SetNewLocation(fireAttackPositions[flamePattern - 1].position, (currPhase < 2), flamePattern + 5);
    }
    /// <summary>
    /// Indicates to the BossMovement script to start moving for a slam attack
    /// </summary>
    public void SlamAttack()
    {
        mover.SetNewLocation(playerRef.position - new Vector3(3f, -2.5f, 0), false, 1);
    }
    /// <summary>
    /// Indicates to the BossMovement script to start moving for a shockwave attack
    /// </summary>
    public void ShockwaveAttack() 
    {
        mover.SetNewLocation(shockwavePos.position, false, 2);
    }

    /// <summary>
    /// Indicates to the BossMovement script to start moving for a rush attack
    /// </summary>
    public void RushAttack()
    {
        mover.BeginRush();
    }
    /// <summary>
    /// Used by the BossMovement script's AnimShockwave to summon falling debris around the boss
    /// </summary>
    public void SummonDebris()
    {
        float randChance = Random.Range(0, 1);
        int numDebris = 0;

        numDebris = (randChance < .7f) ? 1 : 2;

        for(int i = 0; i < numDebris; i++)
        {
            GameObject newDebris = Instantiate(fallingDebris);
        }
    }

    /// <summary>
    /// Called by the BossMovement script to switch the hitbox on or off for slam and shockwave attacks
    /// </summary>
    /// <param name="turnOn"></param>
    public void SlamHitboxSwap(bool turnOn)
    {
        slamHitbox.SetActive(turnOn);
    }


   
}
