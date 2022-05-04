using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMain : MonoBehaviour
{
    public Animator masterAnim;

    public PlayerUI ui;
    public CameraFollow cam;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public GameObject sprinklerAttack, blastAttack, dousingBox;
    public AudioClip gotHit, lightPunch, heavyPunch, sprinkler, hose, dousing;

    [HideInInspector]
    public int health = 15;
    [HideInInspector]
    public int maxHealth = 15;
    [HideInInspector]
    public float specialMeter = 0f;
    [HideInInspector]
    public float maxSpecialMeter = 10f;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public string currSpecial = "";
    [HideInInspector]
    public bool hasSpecial = true;
    [HideInInspector]
    public bool isInCutscene = false;

    private bool isInvincible = false;
    private float iFrames = 0f;
    private float specialRechargeRate = .25f;

    private float animResetTimer = 0f;
    private float perfectBlockTimer = 0;

    private float moveSpeed = 5f;
    
    private float specialDuration = 0f;
    private bool canMove = true;
    private bool canBlock = true;
    private bool isRunning = false;
    private bool isDousing = false;
    private bool isBlocking = false;

    Vector2 movement;

    private bool canAttack = true;
    private bool isWalking = false;

    private float comboTimer = .3f;
    private List<int> comboSequence = new List<int>();
    private int[] uppercutCombo = new int[4] { 1, 1, 2, 1 };
    private int[] slamCombo = new int[5] { 1, 1, 2, 1, 2 };
    private int[] longHitCombo = new int[5] { 1, 2, 1, 2, 2 };
    private float attackCooldown = .1f;

    private GameObject shadow;
    private GameObject currDouse;
    private AudioSource sfx;

    // Start is called before the first frame update
    void Start()
    {
        shadow = transform.Find("Shadow").gameObject;

        cam = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        RunDousing();
        
        // If the player is using a special attack, count down the duration and limit the player abilities
        if (specialDuration > 0)
        {
            canAttack = false;
            canBlock = false;
            
            // This makes sure that the specialDuration is never directly equal to 0
            if (specialDuration - Time.deltaTime == 0) specialDuration -= Time.deltaTime * 1.1f;
            else specialDuration -= Time.deltaTime;

        }
        // Resets values to normal
        else if(specialDuration < 0)
        { 
            specialDuration = 0;
            canAttack = true;
            canBlock = true;
            moveSpeed = 5f;
            sfx.Stop();
            sfx.loop = false;
        }
        
        // Check if the player is running and update speed accordingly
        isRunning = Input.GetKey("left shift") && canAttack;
        if (isRunning) moveSpeed = 10f;
        else if (specialDuration == 0) moveSpeed = 5f;

        // If the player has a special attack, slowly increase the special meter
        if (hasSpecial)
        {
            if (specialMeter < maxSpecialMeter) specialMeter += Time.deltaTime * specialRechargeRate;
            else specialMeter = maxSpecialMeter;
        }
        else specialMeter = 0;

        // If the player isn't in a cutscene, run the player movement
        if (!isInCutscene)
        {
            if (canMove)
            {
                movement.x = (Input.GetAxisRaw("Vertical") > 0) ? Input.GetAxisRaw("Horizontal") + .15f : (Input.GetAxisRaw("Vertical") < 0) ? Input.GetAxisRaw("Horizontal") - .15f : Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical") * .75f;
            }
            else movement = Vector2.zero;
        }
        else rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Update the animator based on whether the player is moving
        if (movement != Vector2.zero && !isWalking && specialDuration <= 0)
        {
            AnimUpdate("walking");
            isWalking = true;
        }
        if(movement == Vector2.zero)
        {
            isWalking = false;
            AnimUpdate("walking", false);
        }

        // Reset all animator values to false to prepare for other inputs
        if (animResetTimer > 0) animResetTimer -= Time.deltaTime;
        else
        {
            animResetTimer = 0;
            AnimUpdate("basicLight", false);
            AnimUpdate("basicHeavy", false);
            AnimUpdate("upperCut", false);
            AnimUpdate("groundSlam", false);
            AnimUpdate("longHit", false);

        }

        // If the player presses Q, activate their special
        if(Input.GetKeyDown("q"))
        {
            ActivateSpecial();
            GameManager.instance.HideSpecialTutorial();
        }

        // Check for dousing input
        isDousing = Input.GetKey("r");

        // Run blocking ability
        isBlocking = Input.GetKey("left ctrl") && canBlock;
        if (isBlocking)
        {
            canMove = false;
            canAttack = false;
        }
        else if(specialDuration == 0)
        {
            canAttack = true;
            canMove = true;
        }
        else
        {
            canMove = true;
        }
        // Run perfect block timer
        if (Input.GetKeyDown("left ctrl"))
        {
            perfectBlockTimer = .2f;
        }
        if (isBlocking && perfectBlockTimer > 0)
        {
            perfectBlockTimer -= Time.deltaTime;
        }
        else perfectBlockTimer = 0;

        // If the game is paused, the player can't attack
        if (Time.timeScale == 0) canAttack = false;

        // Check for attack inputs from the mouse
        AttackInputs();

        // If the player is invincible, fade them a little and update the animator
        if (iFrames > 0)
        {
            sprite.color = new Color(1, 1, 1, .6f);
            iFrames -= Time.deltaTime;
            if (iFrames < 1.5f) AnimUpdate("damaged", false);
        }
        else
        {
            sprite.color = new Color(1, 1, 1);
            iFrames = 0;
            isInvincible = false;
        }
        AnimUpdate("blocking", isBlocking);
        
        
        if (health <= 0) isDead = true;

    }

    

    private void FixedUpdate()
    {
        // This is what actually moves the player
        transform.position += (Vector3)movement * moveSpeed * Time.fixedDeltaTime;
      
        if(!isInCutscene && !(specialDuration > 0 && currSpecial == "blast"))
        {
            if (Input.GetAxisRaw("Horizontal") < 0) transform.localScale = new Vector3(-1f, 1f, 1);
            if (Input.GetAxisRaw("Horizontal") > 0) transform.localScale = new Vector3(1f, 1f, 1);
        }
        
        
    }

    /// <summary>
    /// Resets the player information after loading into the game scene
    /// </summary>
    /// <param name="hasData"></param>
    public void Reset(bool hasData = true)
    {

        isDead = false;
        if (!hasData)
        {
            currSpecial = "";
            health = maxHealth;
            hasSpecial = false;
            specialDuration = 0;
            specialMeter = 0;
        }

    }
    /// <summary>
    /// Deals damage to the player 
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(int damage)
    {
        if (damage > 0)
        {
            if (!isInvincible && perfectBlockTimer == 0)
            {
                health -= (isBlocking) ? damage / 2 : damage;
                isInvincible = true;
                iFrames = 2;
                AnimUpdate("damaged");

                sfx.clip = gotHit;
                sfx.volume = 1f;
                sfx.loop = false;
                sfx.Play();
                
            }
            else if(perfectBlockTimer > 0)
            {
                isInvincible = true;
                iFrames = 1;

                sfx.clip = gotHit;
                sfx.volume = .6f;
                sfx.loop = false;
                sfx.Play();
                
            }
        }
        else
        {
            health -= damage;
            if (health >= maxHealth) health = maxHealth;
        }

    }

    /// <summary>
    /// Dictates the player's movement in cutscenes
    /// </summary>
    /// <param name="direction"></param>
    public void SetMovementInCutscene(string direction)
    {
        if (!isInCutscene) return;

        switch(direction)
        {
            case "left":
                movement.x = -1;
                movement.y = 0;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case "right":
                movement.x = 1;
                movement.y = 0;
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case "up":
                movement.x = .15f;
                movement.y = 1;
                break;
            case "down":
                movement.x = -.15f;
                movement.y = -1;
                break;
            default:
                movement.x = 0;
                movement.y = 0;
                break;

        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player runs into an encounter hitbox, begin that encounter
        if (collision.CompareTag("Encounter"))
        {
            switch (collision.gameObject.name)
            {
                case "Encounter1":
                    GameManager.instance.StartEncounter(1);
                    break;
                case "Encounter2":
                    GameManager.instance.StartEncounter(2);
                    break;
                case "Encounter3":
                    GameManager.instance.StartEncounter(3);
                    break;
                case "BossEncounter":
                    GameManager.instance.StartEncounter(4);
                    break;
            }
            collision.gameObject.SetActive(false);
        }

        // If the player collides with damaging objects, deal damage
        if(collision.CompareTag("Fire"))
        {
            if(collision.GetComponent<FireObstacle>()) ApplyDamage(collision.GetComponent<FireObstacle>().damage);
            if (collision.GetComponent<FireProjectile>()) ApplyDamage(collision.GetComponent<FireProjectile>().damage);
        }

        if(collision.CompareTag("Falling"))
        {
            ApplyDamage(collision.GetComponent<DebrisCollision>().damage);
            collision.GetComponent<DebrisCollision>().DestroyMe();
        }
        if(collision.CompareTag("Slam"))
        {
            ApplyDamage(4);
        }

        // If the player collides with a powerup, grant them the correct powerup
        if(collision.CompareTag("Powerup"))
        {
            currSpecial = (collision.GetComponent<PowerupPickup>().powerupType == 1) ? "blast" : "sprinkler";
            PowerupPickup[] temp = FindObjectsOfType<PowerupPickup>();
            foreach(PowerupPickup p in temp)
            {
                Destroy(p.gameObject);
            }
            specialMeter = maxSpecialMeter;
            hasSpecial = true;
            GameManager.instance.ShowSpecialTutorial();
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fire"))
        {
            if (collision.GetComponent<FireObstacle>()) ApplyDamage(collision.GetComponent<FireObstacle>().damage);
            if (collision.GetComponent<FireProjectile>()) ApplyDamage(collision.GetComponent<FireProjectile>().damage);
        }
    }

    /// <summary>
    /// Run the dousing function to make sure the player can't douse in multiple places at once, or while moving
    /// </summary>
    private void RunDousing()
    {
        if(specialDuration == 0)
        {
            if (isDousing)
            {
                if (!currDouse)
                {
                    float offset = transform.localScale.x;
                    currDouse = Instantiate(dousingBox, new Vector3(transform.position.x + offset, transform.position.y - .5f), Quaternion.identity);
                    currDouse.transform.localScale = transform.localScale;

                    sfx.clip = dousing;
                    sfx.volume = .6f;
                    sfx.loop = true;
                    sfx.Play();

                }
                canMove = false;
                canAttack = false;
            }
            else
            {
                canMove = !isBlocking;
                canAttack = !isBlocking;
                if (currDouse)
                {
                    sfx.Stop();
                    sfx.loop = false;
                    Destroy(currDouse);
                }
            }

        }

    }

    /// <summary>
    /// Takes in all inputs and keeps track of the current combos
    /// </summary>
    private void AttackInputs()
    {
        if (comboTimer > 0) comboTimer -= Time.deltaTime;
        if(comboTimer < 0 || comboSequence.Count > 6)
        {
            comboSequence.Clear();
        }
        if(!canAttack)
        {
            if(attackCooldown > 0) attackCooldown -= Time.deltaTime;
            if(attackCooldown <= 0) canAttack = !isBlocking;
        }
        if(Input.GetButtonDown("Fire1") && canAttack)
        {
            comboTimer = .5f;
            comboSequence.Add(1);
            canAttack = false;
            attackCooldown = .2f;
            CheckCombos("light");
            sfx.clip = lightPunch;
            sfx.volume = .6f;
            sfx.loop = false;
            sfx.Play();
        }

        if(Input.GetButtonDown("Fire2") && canAttack)
        {
            comboTimer = .5f;
            comboSequence.Add(2);
            canAttack = false;
            attackCooldown = .2f;
            CheckCombos("heavy");

            sfx.clip = heavyPunch;
            sfx.volume = .6f;
            sfx.loop = false;
            sfx.Play();
        }
    }
    /// <summary>
    /// Checks the current combo array against the set combos stored in the class, and updates the animator accordingly
    /// </summary>
    /// <param name="type"></param>
    private void CheckCombos(string type)
    {
        var tempArray = comboSequence.ToArray();
        // Run through all possible combos
        if (CompareIntArrays(tempArray, uppercutCombo))
        {
            AnimUpdate("upperCut");
        }
        else if(CompareIntArrays(tempArray, slamCombo))
        {
            AnimUpdate("groundSlam");
            comboSequence.Clear();
        }
        else if(CompareIntArrays(tempArray, longHitCombo))
        {
            AnimUpdate("longHit");
            comboSequence.Clear();
        }
        else
        {
            if (type == "light")
            {
                AnimUpdate("basicLight");
            }
            if (type == "heavy")
            {
                AnimUpdate("basicHeavy");
            }
        }
        
        
        
        
    }
    /// <summary>
    /// Activates the special attack based on the player's current special
    /// </summary>
    private void ActivateSpecial()
    {
        if(hasSpecial && specialMeter == maxSpecialMeter)
        {
            switch(currSpecial)
            {
                case "blast":
                    specialMeter -= 8f;
                    specialDuration = 2.5f;
                    GameObject newBlast = Instantiate(blastAttack, transform.position, Quaternion.identity);
                    if (transform.localScale.x < 0) newBlast.GetComponent<Blast>().flipDirection = true;
                    moveSpeed = 2f;

                    sfx.clip = hose;
                    sfx.volume = .6f;
                    sfx.loop = true;
                    sfx.Play();
                    break;
                case "sprinkler":
                    specialMeter -= 6f;
                    specialDuration = 2f;
                    Instantiate(sprinklerAttack, transform.position, Quaternion.identity);
                    moveSpeed = 2f;

                    sfx.clip = sprinkler;
                    sfx.volume = .6f;
                    sfx.loop = true;
                    sfx.Play();
                    break;
                default:
                    print("Can't use that!");
                    break;

            }
        }
    }
    /// <summary>
    /// Updates the animator's value with the given string
    /// </summary>
    /// <param name="varUpdate"></param>
    /// <param name="setVal"></param>
    private void AnimUpdate(string varUpdate, bool setVal = true)
    {
        masterAnim.SetBool(varUpdate, setVal);

        
    }
    /// <summary>
    /// Compares two int arrays to see if they're equal
    /// </summary>
    /// <param name="arr1"></param>
    /// <param name="arr2"></param>
    /// <returns>Whether the two int arrays are equal</returns>
    private bool CompareIntArrays(int[] arr1, int[] arr2)
    {
        if (arr1.Length != arr2.Length) return false;

        for(int i = 0; i < arr1.Length; i++)
        {
            if(arr1[i] != arr2[i])
            {
                return false;
            }
        }
        return true;
    }
}
