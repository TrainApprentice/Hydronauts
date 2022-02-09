using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public int type;
    public Sprite[] objectImages = new Sprite[4];

    public float direction;
    private float speed = 8f;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<SpriteRenderer>().sprite = objectImages[type - 1];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(Mathf.Cos(direction) * speed * Time.deltaTime, Mathf.Sin(direction) * speed * Time.deltaTime, 0);
        transform.position += movement;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMain>().ApplyDamage(type);
            Destroy(gameObject);
        }
        if(collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
