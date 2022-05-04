using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Sprinkler : MonoBehaviour
{
    private Transform owner;
    private SpriteRenderer image;

    private float lifespan = 2f;
    private float damageInterval = .33333f;
    private float damageAmt = 3f;
    private CapsuleCollider2D hitbox;
    // Start is called before the first frame update
    void Start()
    {
        owner = FindObjectOfType<PlayerMain>().transform;
        hitbox = GetComponent<CapsuleCollider2D>();
        image = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Countdown the sprinkler's lifetime
        if (lifespan > 0) lifespan -= Time.deltaTime;
        else Destroy(gameObject);

        transform.position = owner.position;

        // Countdown the damage interval, and turn on or off the hitbox accordingly
        if (damageInterval > 0)
        {
            damageInterval -= Time.deltaTime;
            if(damageInterval < .2f) hitbox.enabled = false;
        }
        else
        {
            hitbox.enabled = true;
            damageInterval = .33333f;
        }
        

    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyMain>().ApplyDamage(damageAmt);
        }
        if (collision.CompareTag("Boss"))
        {
            collision.GetComponentInParent<BossAI>().ApplyDamage(damageAmt * 4);
        }
        if (collision.CompareTag("Fire"))
        {
            if (collision.GetComponent<FireObstacle>()) collision.GetComponent<FireObstacle>().ApplyDamage(damageAmt);
            else if (collision.GetComponent<FireProjectile>()) Destroy(collision.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyMain>().ApplyDamage(damageAmt);
        }
    }
    

}
