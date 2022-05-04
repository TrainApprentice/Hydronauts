//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain : MonoBehaviour
{   

    public Animator masterAnim;
    private float animResetTimer = 0f;

    public GameObject healthBar, barBack;
    public int enemyType = 0; // 1 for Melee, 2 for Ranged
    
    public float health = 15f;
    public float maxHealth = 15f;

    public bool isDead = false;
    public bool isStunned = false;

    public AudioClip gotHit;
    private AudioSource sound;

    private float iFrames = 0;
    // Start is called before the first frame update
    void Start()
    {
        float rand = Random.Range(0, 2);
        if (rand < 1) transform.localScale = new Vector3(-.2f, .2f, 1);
        else transform.localScale = new Vector3(.2f, .2f, 1);

        if (GetComponent<MeleeEnemyAI>()) enemyType = 1;
        else if (GetComponent<RangedEnemyAI>()) enemyType = 2;

        sound = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        // Runs invincibility frames, showing the health bar when invincible
        if (iFrames > 0)
        {
            iFrames -= Time.deltaTime;
            healthBar.SetActive(true);
            barBack.SetActive(true);
        }
        else
        {
            iFrames = 0;
            isStunned = false;
            healthBar.SetActive(false);
            barBack.SetActive(false);
            
        }

        // Make sure the animator's values are reset for the next animation
        if (animResetTimer > 0) animResetTimer -= Time.deltaTime;
        else
        {
            animResetTimer = 0;
            masterAnim.SetBool("onHit", false);

        }


    }
    /// <summary>
    /// Used by other classes to deal damage to the enemy
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(float damage)
    {
        if(iFrames == 0)
        {
            health -= damage;
            isStunned = true;
            iFrames = .3f;
            UpdateHealthBar();
            AnimUpdate("onHit");
            masterAnim.SetBool("doAttack", false);
            masterAnim.SetBool("isWalking", false);

            sound.clip = gotHit;
            sound.Play();
        }
        
        if (health <= 0) isDead = true;
    }
    /// <summary>
    /// Updates the health bar for each enemy type, as they work on different scales
    /// </summary>
    private void UpdateHealthBar()
    {
        switch(enemyType)
        {
            case 1:
                if (health > 0)
                {
                    healthBar.transform.localPosition = (transform.localScale.x < 0) ? new Vector3((1 - (health / maxHealth)) * 6f, 18f, 0) : new Vector3((1 - (health / maxHealth)) * -6f, 18f, 0);
                    healthBar.transform.localScale = new Vector3((health / maxHealth) * 12f, 1f, 1);
                }
                break;
            case 2:
                if (health > 0)
                {
                    healthBar.transform.localPosition = (transform.localScale.x < 0) ? new Vector3((1 - (health / maxHealth)) * .875f, 2.5f, 0) : new Vector3((1 - (health / maxHealth)) * -.875f, 2.5f, 0);
                    healthBar.transform.localScale = new Vector3((health / maxHealth) * 1.75f, .15f, 1);
                }
                break;

        }
        
        
    }
    /// <summary>
    /// Updates a given bool in the animator to true, and sets the reset timer to prepare for the next one
    /// </summary>
    /// <param name="varUpdate"></param>
    private void AnimUpdate(string varUpdate)
    {
        masterAnim.SetBool(varUpdate, true);
        animResetTimer = .1f;
    }


}
