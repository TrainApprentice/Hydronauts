using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour
{
    private Transform owner;
    public bool flipDirection;
    private float lifespan = 2.5f;
    private float damageAmt = 5f;
    // Start is called before the first frame update
    void Start()
    {
        owner = FindObjectOfType<PlayerMain>().transform;
        transform.position = owner.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Countdown the lifespan
        if (lifespan > 0) lifespan -= Time.deltaTime;
        else Destroy(gameObject);

        // Scale the blast across the screen, and make sure it stays with the player
        float scale = (lifespan > 2.25) ? lifespan - 2.25f : 0f;
        transform.localScale = (flipDirection) ? new Vector3((.75f - scale) * -6f, .2f, 0f) : new Vector3((.75f - scale) * 6f, .2f, 0f);
        transform.position = owner.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyMain>().ApplyDamage(damageAmt);
        }
        if(collision.CompareTag("Boss"))
        {
            collision.GetComponentInParent<BossAI>().ApplyDamage(damageAmt * 3);
            
        }
        if (collision.CompareTag("Fire"))
        {
            if (collision.GetComponent<FireObstacle>()) collision.GetComponent<FireObstacle>().ApplyDamage(damageAmt);
            else if (collision.GetComponent<FireProjectile>()) Destroy(collision.gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyMain>().ApplyDamage(damageAmt);
        }
        if (collision.CompareTag("Boss"))
        {
            collision.GetComponentInParent<BossAI>().ApplyDamage(damageAmt * 3);
            
        }
        if (collision.CompareTag("Fire"))
        {
            if (collision.GetComponent<FireObstacle>()) collision.GetComponent<FireObstacle>().ApplyDamage(damageAmt);
            else if (collision.GetComponent<FireProjectile>()) Destroy(collision.gameObject);
        }
    }
}
