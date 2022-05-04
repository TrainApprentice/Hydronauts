using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class MeleeEnemyAI : MonoBehaviour
{
    public Animator AnimController;
    public EnemyMain master;

    public AudioClip punch;
    private AudioSource sound;

    public Transform target;
    private Vector3 punchTarget;

    private float speed = 5f;
    private float nextWaypointDistance = 2f;
    private float pathWait = 1f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    float attackTimer = 1f;
    float attackCooldown = 1f;
    public bool isAttacking = false;

    Seeker seeker;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").GetComponent<Transform>();
        seeker = GetComponent<Seeker>();
        sound = GetComponent<AudioSource>();

        // Set the move target to one side of the player, based on the enemy's relative position to them
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

        // Start the pathfinding
        UpdatePath();

        //InvokeRepeating("UpdatePath", 0, 2f);
        
    }

    void FixedUpdate()
    {
        // If the path is gone or the enemy is stunned, don't run anything else
        if (path == null || master.isStunned) return;

        // Decrease the cooldowns on attacking and pathing
        if (attackCooldown > 0) attackCooldown -= Time.fixedDeltaTime;
        if (pathWait > 0) pathWait -= Time.fixedDeltaTime;

        // Updates the move target based on relative position to the player
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

        // If the enemy reached the end of the path, see if it can attack
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

            // Set up the attack
        } else
        {
            reachedEndOfPath = false;
            // Still walking
        }

        // Figure out its movement vector to update the animator
        Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - new Vector2(transform.position.x, transform.position.y)).normalized;

        if (dir != Vector2.zero) AnimController.SetBool("isWalking", true);

        transform.position += new Vector3(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime, 0);
        
        // If the enemy is close enough to the next path point, work toward the next one
        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance) currentWaypoint++;

        if (!isAttacking) StopCoroutine("MeleeAttack");
    }

    /// <summary>
    /// Runs the melee attack over one second
    /// </summary>
    /// <returns></returns>
    private IEnumerator MeleeAttack()
    {
        AnimController.SetBool("isWalking", false);
        if (!AnimController.GetBool("doAttack"))
        {
            sound.clip = punch;
            sound.Play();
            AnimController.SetBool("doAttack", true);
        }
        isAttacking = true;
        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0)
        {
            isAttacking = false;
            UpdatePath();
            attackTimer = 1;
            attackCooldown = 3f;
            AnimController.SetBool("doAttack", false);
        }
        yield return new WaitForSeconds(Time.deltaTime);
    }

    /// <summary>
    /// Runs when the path is completed to prepare for the next one
    /// </summary>
    /// <param name="p"></param>
    void OnPathComplete(Path p)
    {
        if (!p.error) 
        {
            path = p;
            currentWaypoint = 0;
        } 
    }

    /// <summary>
    /// Updates the path to move toward the updated move target
    /// </summary>
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, punchTarget, OnPathComplete);
        }
    }
}
