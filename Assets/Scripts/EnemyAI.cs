using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private Vector3 punchTarget;

    private float speed = 5f;
    private float nextWaypointDistance = 2f;
    private float pathWait = 1f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    int enemyType = 0; // 0 for Melee, 1 for Ranged
    float attackTimer = 1f;
    float attackCooldown = 1f;
    public bool isAttacking = false;

    Seeker seeker;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").GetComponent<Transform>();
        seeker = GetComponent<Seeker>();


        if (transform.position.x > target.position.x)
        {
            punchTarget = new Vector3(target.position.x + 2f, target.position.y, 0);
            transform.localScale = new Vector3(-.2f, .2f, 1);
        }
        else
        {
            punchTarget = new Vector3(target.position.x - 2f, target.position.y, 0);
            transform.localScale = new Vector3(.2f, .2f, 1);
        }

        UpdatePath();

        //InvokeRepeating("UpdatePath", 0, 2f);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null) return;

        if (attackCooldown > 0) attackCooldown -= Time.fixedDeltaTime;
        if (pathWait > 0) pathWait -= Time.fixedDeltaTime;

        if (transform.position.x > target.position.x)
        {
            punchTarget = new Vector3(target.position.x + 2f, target.position.y, 0);
            transform.localScale = new Vector3(-.2f, .2f, 1);
        }
        else
        {
            punchTarget = new Vector3(target.position.x - 2f, target.position.y, 0);
            transform.localScale = new Vector3(.2f, .2f, 1);
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            if (attackCooldown <= 0) StartCoroutine("MeleeAttack");
            else
            {
                if (pathWait <= 0)
                {
                    UpdatePath();
                    pathWait = .5f;
                }
            }
            //UpdatePath();

            // Set up the attack
        } else
        {
            reachedEndOfPath = false;
            
            // Still walking
        }

        Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - new Vector2(transform.position.x, transform.position.y)).normalized;

        transform.position += new Vector3(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime, 0);
        
        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance) currentWaypoint++;

        if (!isAttacking) StopCoroutine("MeleeAttack");
    }

    private IEnumerator MeleeAttack()
    { 
        isAttacking = true;
        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0)
        {
            isAttacking = false;
            UpdatePath();
            attackTimer = 1;
            attackCooldown = 3f;
        }
        yield return new WaitForSeconds(Time.deltaTime);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error) 
        {
            path = p;
            currentWaypoint = 0;
        } 
    }

    void UpdatePath()
    {
        if (seeker.IsDone()) seeker.StartPath(transform.position, punchTarget, OnPathComplete);
    }
}
