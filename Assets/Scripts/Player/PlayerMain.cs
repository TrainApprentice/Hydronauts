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

    public int health = 10;
    public int maxHealth = 10;
    public float specialMeter = 10f;
    public float maxSpecialMeter = 10f;
    public bool isDead = false;

    private bool isInvincible = false;
    private float iFrames = 0f;
    private bool hasSpecial = true;
    private string currSpecial = "sprinkler";
    private float specialRechargeRate = .25f;

    private float animResetTimer = 0f;
    private float perfectBlockTimer = 0;

    private float moveSpeed = 5f;
    private float jumpForce = 5000f;
    
    private float specialDuration = 0f;
    private bool canMove = true;
    private bool canBlock = true;
    private bool isRunning = false;
    private bool isDousing = false;
    private bool isBlocking = false;

    Vector2 movement;

    private int playerState, animState;
    private bool canAttack = true;
    private bool isWalking = false;

    private float landingY;
    private bool setLanding = false;

    private float comboTimer = .3f;
    private List<int> comboSequence = new List<int>();
    private int[] uppercutCombo = new int[4] { 1, 1, 2, 1 };
    private int[] slamCombo = new int[5] { 1, 1, 2, 1, 2 };
    private int[] longHitCombo = new int[5] { 1, 2, 1, 2, 2 };
    private float attackCooldown = .1f;

    private GameObject shadow;
    private GameObject currDouse;
    private const int IDLE_STATE = 0;
    private const int WALK_STATE = 1;
    private const int ATTACK_STATE = 2;
    private const int JUMP_STATE = 3;
    private const int HIT_STATE = 4;
    private const int SPECIAL_STATE = 5;

    // Start is called before the first frame update
    void Start()
    {
        shadow = transform.Find("Shadow").gameObject;

        cam = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        // If there's a save, load those stats
        // Otherwise, reset to default values

        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        RunDousing();
        

        if (specialDuration > 0)
        {
            canAttack = false;
            canBlock = false;
            if (currSpecial == "blast") canMove = false;
            if (specialDuration - Time.deltaTime == 0) specialDuration -= Time.deltaTime * 1.1f;
            else specialDuration -= Time.deltaTime;

        }
        else if(specialDuration < 0)
        { 
            specialDuration = 0;
            canAttack = true;
            canBlock = true;
            if (currSpecial == "blast") canMove = true;
            if (currSpecial == "sprinkler") moveSpeed = 5f;
        }
        

        isRunning = Input.GetKey("left shift");
        if (isRunning) moveSpeed = 10f;
        else if (specialDuration == 0) moveSpeed = 5f;

        if (hasSpecial)
        {
            if (specialMeter < maxSpecialMeter) specialMeter += Time.deltaTime * specialRechargeRate;
            else specialMeter = maxSpecialMeter;
        }
        else specialMeter = 0;

        if (canMove)
        {
            movement.x = (Input.GetAxisRaw("Vertical") > 0) ? Input.GetAxisRaw("Horizontal") + .15f : (Input.GetAxisRaw("Vertical") < 0) ? Input.GetAxisRaw("Horizontal") - .15f : Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical") * .75f;
        }
        else movement = Vector2.zero;


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
        if(Input.GetKeyDown("q"))
        {
            ActivateSpecial();
        }
        isDousing = Input.GetKey("r");
        isBlocking = Input.GetKey("left ctrl") && canBlock;
        if (isBlocking)
        {
            canMove = false;
            canAttack = false;
        }
        else
        {
            canAttack = true;
            canMove = true;
        }
        if (Input.GetKeyDown("left ctrl"))
        {
            perfectBlockTimer = .2f;
        }
        if (isBlocking && perfectBlockTimer > 0)
        {
            perfectBlockTimer -= Time.deltaTime;
        }
        else perfectBlockTimer = 0;

        AttackInputs();
        //StateMachine();
        

        if (iFrames > 0)
        {
            sprite.color = new Color(1, 1, 1, .6f);
            iFrames -= Time.deltaTime;
        }
        else
        {
            sprite.color = new Color(1, 1, 1);
            iFrames = 0;
            isInvincible = false;
        }

        if(isBlocking)
        {
            sprite.color = new Color(.8f, .8f, .8f);
        }
        else sprite.color = new Color(1, 1, 1);

        // DEBUG ONLY
        
        if (Input.GetKeyDown("l"))
        {
            cam.Shake(.1f, 1);
        }
        if (Input.GetKeyDown("b"))
        {
            currSpecial = "blast";
            specialMeter = maxSpecialMeter;
        }
        if (Input.GetKeyDown("r"))
        {
            currSpecial = "sprinkler";
            specialMeter = maxSpecialMeter;
        }

        if (health <= 0) isDead = true;

    }

    

    private void FixedUpdate()
    {
        transform.position += (Vector3)movement * moveSpeed * Time.fixedDeltaTime;
      
        if (Input.GetAxisRaw("Horizontal") < 0) transform.localScale = new Vector3(-1f, 1f, 1);
        if (Input.GetAxisRaw("Horizontal") > 0) transform.localScale = new Vector3(1f, 1f, 1);
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Encounter")
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

        if(collision.CompareTag("Fire"))
        {
            if(collision.GetComponent<FireObstacle>()) ApplyDamage(collision.GetComponent<FireObstacle>().damage);
            if (collision.GetComponent<FireProjectile>()) ApplyDamage(collision.GetComponent<FireProjectile>().damage);
        }

    }

    private void RunDousing()
    {
        if (isDousing)
        {
            if (!currDouse)
            {
                float offset = transform.localScale.x;
                currDouse = Instantiate(dousingBox, new Vector3(transform.position.x + offset, transform.position.y - .5f), Quaternion.identity);
                currDouse.transform.localScale = transform.localScale;
            }
            canMove = false;
            canAttack = false;
        }
        else
        {
            canMove = !isBlocking;
            canAttack = !isBlocking;
            Destroy(currDouse);
        }
        
    }

    public void ApplyDamage(int damage)
    {
        if(damage > 0)
        {
            if (!isInvincible && perfectBlockTimer == 0)
            {
                health -= (isBlocking) ? damage / 2 : damage;
                isInvincible = true;
                iFrames = 2;
            }
        }
        else
        {
            health -= damage;
            if (health >= maxHealth) health = maxHealth;
        }
        
    }

    
    private void StateMachine()
    {
        switch(playerState)
        {
            case IDLE_STATE:
                canAttack = true;
                break;
            case WALK_STATE:
                canAttack = true;
                break;
            case ATTACK_STATE:
                canAttack = true;
                break;
            case JUMP_STATE:
                canAttack = true;
                break;
            case HIT_STATE:
                canAttack = false;
                break;
            case SPECIAL_STATE:
                canAttack = false;
                break;
            default:
                break;
        }
    }
    private void AttackInputs()
    {
        if (comboTimer > 0) comboTimer -= Time.deltaTime;
        if(comboTimer < 0 || comboSequence.Count > 6)
        {
            comboTimer = 0;
            comboSequence.Clear();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        if(!canAttack)
        {
            attackCooldown -= Time.deltaTime;
            if(attackCooldown <= 0) canAttack = !isBlocking;
        }
        if(Input.GetButtonDown("Fire1") && canAttack)
        {
            comboTimer = .5f;
            if(playerState == JUMP_STATE)
            {
                // Jump Attack (Light)
                comboSequence.Add(3);
                canAttack = false;
                attackCooldown = .2f;
            }
            else
            {
                //Left Punch (Light Attack)
                comboSequence.Add(1);
                canAttack = false;
                attackCooldown = .2f;
            }
            CheckCombos("light");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        }

        if(Input.GetButtonDown("Fire2") && canAttack)
        {
            comboTimer = .5f;
            if (playerState == JUMP_STATE)
            {
                // Jump Attack (Heavy)
                comboSequence.Add(4);
                canAttack = false;
                attackCooldown = .2f;
            }
            else
            {
                //Right Punch (Heavy Attack)
                comboSequence.Add(2);
                canAttack = false;
                attackCooldown = .2f;
            }
            CheckCombos("heavy");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

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
    public void Reset()
    {
        health = maxHealth;
        isDead = false;
        //hasSpecial = false;
        //specialDuration = 0;
        //specialMeter = 0;
    }

    private void ActivateSpecial()
    {
        if(hasSpecial && specialMeter == maxSpecialMeter)
        {
            switch(currSpecial)
            {
                case "blast":
                    specialMeter -= 8f;
                    specialDuration = 1f;
                    GameObject newBlast = Instantiate(blastAttack, transform.position, Quaternion.identity);
                    if (transform.localScale.x < 0) newBlast.GetComponent<Blast>().flipDirection = true;
                    canMove = false;
                    break;
                case "sprinkler":
                    specialMeter -= 6f;
                    specialDuration = 2f;
                    Instantiate(sprinklerAttack, transform.position, Quaternion.identity);
                    moveSpeed = 2f;
                    break;
                default:
                    print("Can't use that!");
                    break;

            }
        }
    }

    private void AnimUpdate(string varUpdate, bool setVal = true)
    {
        masterAnim.SetBool(varUpdate, setVal);
        if(varUpdate != "walking") animResetTimer = .1f;

        
    }

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
