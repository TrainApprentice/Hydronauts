using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public GameObject fireProj;
    public Transform fireSpawnLeft, fireSpawnRight;
    public Transform playerRef;

    private BossMovement mover;
    private bool isAttackingFire = false;
    private bool isAttackingSlam = false;
    private bool isAttackingRush = false;
    private bool isAttackingShockwave = false;
    private int currPhase = 1;
    private int flamePattern = 1;
    private float fireAttackTimer = 2f;
    private float timeBetweenFlames = .05f;
    // Start is called before the first frame update
    void Start()
    {
        playerRef = FindObjectOfType<PlayerMain>().transform;
        mover = GetComponent<BossMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("k") && !isAttackingFire)
        {
            isAttackingFire = true;
            fireAttackTimer = 2f;
        }
        if(Input.GetKeyDown("o") && !isAttackingSlam)
        {
             SlamAttack();
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
                //print("1 " + newFire.GetComponent<FireProjectile>().angle);
            }
            else if (choosePattern == 2)
            {
                GameObject newFire = Instantiate(fireProj, fireSpawnRight.position, Quaternion.identity);
                newFire.GetComponent<FireProjectile>().angle = (fireAttackTimer < 1) ? -30f + ((1 - fireAttackTimer) * 30f): -30f;
                //print("2 " + newFire.GetComponent<FireProjectile>().angle);
            }
            else
            {
                GameObject newFire1 = Instantiate(fireProj, fireSpawnLeft.position, Quaternion.identity);
                newFire1.GetComponent<FireProjectile>().angle = 30f;
                GameObject newFire2 = Instantiate(fireProj, fireSpawnRight.position, Quaternion.identity);
                newFire2.GetComponent<FireProjectile>().angle = -30f;
            }
            print(choosePattern);
            timeBetweenFlames = .05f;
        }
        if (fireAttackTimer <= 0)
        {
            isAttackingFire = false;
            if (flamePattern < 3) flamePattern++;
            else flamePattern = 1;

            timeBetweenFlames = 0;
        }
        
    }

    public void SlamAttack()
    {
        mover.SetNewLocation(playerRef.position - new Vector3(3f, -1f, 0), 1);
    }
}
