using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour
{
    private Transform owner;
    public bool flipDirection;
    private float lifespan = .75f;
    private float damageAmt = 20f;
    // Start is called before the first frame update
    void Start()
    {
        owner = FindObjectOfType<PlayerMain>().transform;
        transform.position = owner.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifespan > 0) lifespan -= Time.deltaTime;
        else Destroy(gameObject);

        transform.localScale = (flipDirection) ? new Vector3((.75f - lifespan) * -6f, 1.5f, 0f) : new Vector3((.75f - lifespan) * 6f, 1.5f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Bing");
        if(collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyMain>().ApplyDamage(damageAmt);
        }
    }
}
