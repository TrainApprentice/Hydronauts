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

    // Start is called before the first frame update
    void Start()
    {
        shadow = transform.Find("Shadow").gameObject;

        cam = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        
    }

    // Update is called once per frame
    void Update()
    {
        RunDousing();
        

        if (specialDuration > 0)
        {
            canAttack = false;
            canBlock = false;
            //if (currSpecial == "blast") canMove = false;
            if (specialDuration - Time.deltaTime == 0) specialDuration -= Time.deltaTime * 1.1f;
            else specialDuration -= Time.deltaTime;

        }
        else if(specialDuration < 0)
        { 
            specialDuration = 0;
            canAttack = true;
            canBlock = true;
            moveSpeed = 5f;
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
            GameManager.instance.HideSpecialTutorial();
        }
        isDousing = Input.GetKey("r");
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
        if (Input.GetKeyDown("left ctrl"))
        {
            perfectBlockTimer = .2f;
        }
        if (isBlocking && perfectBlockTimer > 0)
        {
            perfectBlockTimer -= Time.deltaTime;
        }
        else perfectBlockTimer = 0;

        if (Time.timeScale == 0) canAttack = false;

        AttackInputs();
        //StateMachine();
        

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
        transform.position += (Vector3)movement * moveSpeed * Time.fixedDeltaTime;
      
        if(!isInCutscene && !(specialDuration > 0 && currSpecial == "blast"))
        {
            if (Input.GetAxisRaw("Horizontal") < 0) transform.localScale = new Vector3(-1f, 1f, 1);
            if (Input.GetAxisRaw("Horizontal") > 0) transform.localScale = new Vector3(1f, 1f, 1);
        }
        
        
    }

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
            }
            else if(perfectBlockTimer > 0)
            {
                isInvincible = true;
                iFrames = 1;
            }
        }
        else
        {
            health -= damage;
            if (health >= maxHealth) health = maxHealth;
        }

    }

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

        }

        if(Input.GetButtonDown("Fire2") && canAttack)
        {
            comboTimer = .5f;
            comboSequence.Add(2);
            canAttack = false;
            attackCooldown = .2f;
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
        //if(varUpdate != "walking") animResetTimer = .1f;

        
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
