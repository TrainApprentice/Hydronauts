//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    public EnemyAI AIController;

    public Animator masterAnim;
    private float animResetTimer = 0f;

    public GameObject healthBar, barBack;
    
    public float health = 15f;
    public float maxHealth = 15f;

    public bool isDead = false;
    public bool isStunned = false;

    private float iFrames = 0;
    // Start is called before the first frame update
    void Start()
    {
        float rand = Random.Range(0, 2);
        if (rand < 1) transform.localScale = new Vector3(-.2f, .2f, 1);
        else transform.localScale = new Vector3(.2f, .2f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (AIController.isAttacking)
        {
            //GetComponent<SpriteRenderer>().color = new Color(1, .5f, .5f);
            //AnimUpdate("doAttack");
        }
        else
        {
            //GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
        */
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
            //GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        if (animResetTimer > 0) animResetTimer -= Time.deltaTime;
        else
        {
            animResetTimer = 0;
            masterAnim.SetBool("onHit", false);
            //masterAnim.SetBool("doAttack", false);

        }


    }

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
        }
        
        if (health <= 0) isDead = true;
    }

    private void UpdateHealthBar()
    {
        if(health > 0)
        {
            healthBar.transform.localPosition = (transform.localScale.x < 0) ? new Vector3((1 - (health / maxHealth)) * 6f, 18f, 0) : new Vector3((1 - (health / maxHealth)) * -6f, 18f, 0);
            healthBar.transform.localScale = new Vector3((health / maxHealth) * 12f, 1f, 1);   
        }
        
    }

    private void AnimUpdate(string varUpdate)
    {
        masterAnim.SetBool(varUpdate, true);
        animResetTimer = .1f;
    }


}
