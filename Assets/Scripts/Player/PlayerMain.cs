using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMain : MonoBehaviour
{
    public Animator masterAnim;
    private float animResetTimer = 0f;

    public PlayerUI ui;

    public int health = 10;
    private bool isInvincible = false;
    private float iFrames = 0f;
    public bool hasSpecial;
    public string currSpecial;

    public CameraFollow cam;

    private float moveSpeed = 5f;
    private float jumpForce = 5000f;
    public Rigidbody2D rb;
    
    public SpriteRenderer sprite;

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
    }

    // Update is called once per frame
    void Update()
    {
        //print(animResetTimer);
        movement.x = (Input.GetAxisRaw("Vertical") > 0) ? Input.GetAxisRaw("Horizontal") + .15f : (Input.GetAxisRaw("Vertical") < 0) ? Input.GetAxisRaw("Horizontal") - .15f : Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical") * .75f;

        //print(comboTimer);

        if(movement != Vector2.zero && !isWalking)
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

        if (Input.GetButtonDown("Jump"))
        {
            // Figure out Jump physics
            playerState = JUMP_STATE;
            
        }
        if(Input.GetKeyDown("f"))
        {
            cam.GetComponent<CameraFollow>().SwapFreeze(false);
        }
        if(Input.GetKeyDown("l"))
        {
            ApplyDamage(3);
            //print("Bing");
        }
        if (playerState == JUMP_STATE)
        {
            if (!setLanding)
            {
                landingY = rb.position.y;
                jumpForce = 5000f;
                gameObject.layer = 2;
                setLanding = true;
            }
            shadow.transform.position = new Vector3(transform.position.x, landingY - 1.8f, 0f);
            movement.y += jumpForce * Time.deltaTime;
            jumpForce += Physics2D.gravity.y;
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                landingY += Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
            }

            if (rb.position.y <= landingY && jumpForce < 0)
            {
                playerState = WALK_STATE;
                rb.position = new Vector2(rb.position.x, landingY);
                shadow.transform.position = (transform.localScale.x > 0) ? new Vector3(transform.position.x - .28f, transform.position.y - 1.8f, 0f) : new Vector3(transform.position.x + .28f, transform.position.y - 1.8f, 0f);
                gameObject.layer = 0;
                setLanding = false;
            }
        }

        AttackInputs();
        StateMachine();

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

    }
    private void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (Input.GetAxisRaw("Horizontal") < 0) transform.localScale = new Vector3(-1f, 1f, 1);
        if (Input.GetAxisRaw("Horizontal") > 0) transform.localScale = new Vector3(1f, 1f, 1);
        
    }

    public void ApplyDamage(int damage)
    {
        if(!isInvincible)
        {
            health -= damage;
            isInvincible = true;
            iFrames = 2;
        }
        ui.UpdateHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Encounter")
        {
            switch(collision.gameObject.name)
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
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
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
        if(comboTimer < 0)
        {
            comboTimer = 0;
            comboSequence.Clear();
        }
        if(!canAttack)
        {
            attackCooldown -= Time.deltaTime;
            if(attackCooldown <= 0) canAttack = true;
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
        }
    }

    private void CheckCombos(string type)
    {
        var tempArray = comboSequence.ToArray();
        // Run through all possible combos
        if (CompareIntArrays(tempArray, uppercutCombo))
        {
            AnimUpdate("upperCut");
            //Clear combos at end of sequence
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
