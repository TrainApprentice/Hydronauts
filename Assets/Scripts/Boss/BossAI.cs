using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public GameObject fireProj;
    public Transform fireSpawnLeft, fireSpawnRight;
    public Transform playerRef;
    
    private Transform[] fireAttackPositions = new Transform[3];
    private BossUI healthController;

    public bool isAttackingFire = false;
    public bool isAttackingSlam = false;
    public bool isAttackingRush = false;
    public bool isAttackingShockwave = false;

    public int health = 100;

    private BossMovement mover;
    private Transform shockwavePos;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("k") && !isAttackingFire)
        {
            FlamethrowerAttack();
            
            fireAttackTimer = 2f;
        }
        if(Input.GetKeyDown("o") && !isAttackingSlam)
        {
            SlamAttack();
            isAttackingSlam = true;
        }
        if(Input.GetKeyDown("u") && !isAttackingShockwave)
        {
            ShockwaveAttack();
            isAttackingShockwave = true;
        }
        if(Input.GetKeyDown("y") && !isAttackingRush)
        {
            RushAttack();
            isAttackingRush = true;
        }

        if (isAttackingFire) FlamethrowerAttack(flamePattern);
    }

    public void FlamethrowerAttack(int choosePattern)
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
            print(flamePattern);
            timeBetweenFlames = 0;
        }
        
    }
    public void ApplyDamage(int damage)
    {
        health -= damage;
        healthController.SetCurrentHealth(health);

        if (health <= 0) Destroy(gameObject);
    }

    public void FlamethrowerAttack()
    {
        mover.SetNewLocation(fireAttackPositions[flamePattern - 1].position, flamePattern + 5);
    }
    public void SlamAttack()
    {
        mover.SetNewLocation(playerRef.position - new Vector3(3f, -2.5f, 0), 1);
    }

    public void ShockwaveAttack() 
    {
        mover.SetNewLocation(shockwavePos.position, 2);
    }
    public void RushAttack()
    {
        mover.BeginRush();
    }
}
