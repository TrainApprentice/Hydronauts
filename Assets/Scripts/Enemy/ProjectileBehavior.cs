using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    /// <summary>
    /// Determines the image and damage of the projectile
    /// </summary>
    public int type;
    public Sprite[] objectImages = new Sprite[3];

    public float direction;
    private float speed = 10f;

    private float rotSpeed = 500f;
    private float currRot = 0f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = objectImages[type - 1];
    }

    // Update is called once per frame
    void Update()
    {
        // Move the projectile in a direction
        Vector3 movement = new Vector3(Mathf.Cos(direction) * speed * Time.deltaTime, Mathf.Sin(direction) * speed * Time.deltaTime, 0);
        transform.position += movement;

        // Spin!
        currRot += rotSpeed * Time.deltaTime;
        if (currRot >= 360) currRot -= 360;

        transform.eulerAngles = new Vector3(0, 0, currRot);
    }

    // Checks for collision between the projectile and either the player and the wall
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
