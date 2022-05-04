using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class RangedEnemyAI : MonoBehaviour
{
    public Animator AnimController;
    public EnemyMain master;

    public GameObject projectile;
    public AudioClip throwing;
    private AudioSource sound;

    private int projectileType;
    public Transform target;
    private float walkDistance = 10f;
    private Vector3 walkTarget;
    

    private float speed = 5f;
    private float nextWaypointDistance = 2f;
    private float pathWait = 1f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    float attackTimer = .75f;
    float attackCooldown = 1f;
    public bool isAttacking = false;
    private bool hasThrown = false;
    private bool doSoundOnce = true;

    Seeker seeker;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").GetComponent<Transform>();
        seeker = GetComponent<Seeker>();
        
        // Pick the move target in relation to the player a certain distance away
        if (transform.position.x < target.position.x)
        {
            var randAngle = Random.Range(Mathf.PI / 2, Mathf.PI * 1.5f);
            walkTarget = new Vector3(target.position.x + (Mathf.Cos(randAngle) * walkDistance), target.position.y + (Mathf.Sin(randAngle) * walkDistance), 0);
            transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
        else
        {
            var randAngle = Random.Range(Mathf.PI * 1.5f, Mathf.PI * 2.5f);
            walkTarget = new Vector3(target.position.x + Mathf.Cos(randAngle) * walkDistance, target.position.y + Mathf.Sin(randAngle) * walkDistance, 0);
            transform.localScale = new Vector3(-1.5f, 1.5f, 1);
        }

        sound = GetComponent<AudioSource>();

        // Start the pathing
        UpdatePath();
        projectileType = (int)Random.Range(1, 4);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If there isn't a path or the enemy is stunned, don't run this
        if (path == null || master.isStunned) return;

        // Decrease the cooldowns on attacking and pathing
        if (attackCooldown > 0) attackCooldown -= Time.fixedDeltaTime;
        if (pathWait > 0) pathWait -= Time.fixedDeltaTime;

        // Update the move target based on relative position to the player
        if (transform.position.x < target.position.x)
        {
            var randAngle = Random.Range(Mathf.PI / 2, Mathf.PI * 1.5f);
            walkTarget = new Vector3(target.position.x + (Mathf.Cos(randAngle) * walkDistance), target.position.y + (Mathf.Sin(randAngle) * walkDistance), 0);
            transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
        else
        {
            var randAngle = Random.Range(Mathf.PI * 1.5f, Mathf.PI * 2.5f);
            walkTarget = new Vector3(target.position.x + Mathf.Cos(randAngle) * walkDistance, target.position.y + Mathf.Sin(randAngle) * walkDistance, 0);
            transform.localScale = new Vector3(-1.5f, 1.5f, 1);
        }

        // If the enemy reached the end of its path, see if it can attack, otherwise update the path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            if (attackCooldown < 0) 
            {
                //attackCooldown = 0;
                StartCoroutine("RangedAttack"); 
            }
            else
            {
                if (pathWait <= 0)
                {
                    UpdatePath();
                    pathWait = 2f;
                }
            }

           
        } else
        {
            reachedEndOfPath = false;
            // Still walking
        }

        // Determine the enemy's movement vector and use it to update the position and the animator
        Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - new Vector2(transform.position.x, transform.position.y)).normalized;

        if (dir != Vector2.zero) AnimController.SetBool("isWalking", true);

        transform.position += new Vector3(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime, 0);

        // If the enemy is close enough to the next path point, work toward the next one
        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance) currentWaypoint++;

        if (!isAttacking) StopCoroutine("RangedAttack");
    }
    /// <summary>
    /// Runs the ranged attack over .75 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator RangedAttack()
    {
        // Play the sound once
        if(doSoundOnce)
        {
            sound.clip = throwing;
            sound.Play();
            doSoundOnce = false;
        }
      
        
        AnimController.SetBool("isWalking", false);
        if (!AnimController.GetBool("doAttack")) AnimController.SetBool("doAttack", true);
        attackTimer -= Time.deltaTime;
        isAttacking = true;

        // Halfway through the animation, create the projectile
        if (attackTimer <= .375f && !hasThrown)
        {
            hasThrown = true;
            GameObject newThrow = Instantiate(projectile, transform.position, Quaternion.identity);
            newThrow.GetComponent<ProjectileBehavior>().direction = AngleToPlayer();
            newThrow.GetComponent<ProjectileBehavior>().type = projectileType;
        }
        if (attackTimer <= 0)
        {
            UpdatePath();
            attackTimer = .75f;
            attackCooldown = 3f;
            hasThrown = false;
            AnimController.SetBool("doAttack", false);
            isAttacking = false;
            doSoundOnce = true;
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
            seeker.StartPath(transform.position, walkTarget, OnPathComplete);
        }
    }
    /// <summary>
    /// Determines the angle between the enemy and the player
    /// </summary>
    /// <returns></returns>
    float AngleToPlayer()
    {
        var dx = target.position.x - transform.position.x;
        var dy = target.position.y - transform.position.y;
        return Mathf.Atan2(dy, dx);
    }
}
