using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    private float lifetime = 1f;
    private float moveSpeed = 25f;
    private float size = 1f;
    private float maxSize = 2f;
    private SpriteRenderer sprite;
    public float angle;
    public int damage = 2;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        angle *= Mathf.PI / 180;
    }


    void Update()
    {
        // Countdown lifetime
        lifetime -= Time.deltaTime;
        
        // Change the scale and opacity based on lifetime
        float p = 1 - lifetime;
        size = AnimMath.Lerp(size, maxSize, p);
        transform.localScale = new Vector3(size, size, 1);
        sprite.color = new Color(1, 1, 1, lifetime);

        // Move the flame in the correct direction, with some friction
        Vector3 direction = new Vector3(moveSpeed * Mathf.Cos(angle) * Time.deltaTime, moveSpeed * Mathf.Sin(angle) * Time.deltaTime, 0);
        moveSpeed *= .999f;
        transform.position += direction;

        // When lifetime runs out, destroy the fire
        if (lifetime <= 0) Destroy(gameObject);
    }
}
