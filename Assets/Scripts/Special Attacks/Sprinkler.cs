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
        if (lifespan > 0) lifespan -= Time.deltaTime;
        else Destroy(gameObject);

        transform.position = owner.position;

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
        //FadeColor();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyMain>().ApplyDamage(damageAmt);
        }
        if(collision.CompareTag("Fire"))
        {
            collision.GetComponent<FireObstacle>().ApplyDamage(1);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyMain>().ApplyDamage(damageAmt);
        }
    }
    void FadeColor()
    {
        Color thing = new Color(image.color.r, image.color.g, image.color.b, damageInterval * 3);
        image.color = thing;
    }

}
