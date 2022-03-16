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

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        angle *= Mathf.PI / 180;
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        

        float p = 1 - lifetime;
        size = AnimMath.Lerp(size, maxSize, p);
        transform.localScale = new Vector3(size, size, 1);

        sprite.color = new Color(1, 1, 1, lifetime);

        Vector3 direction = new Vector3(moveSpeed * Mathf.Cos(angle) * Time.deltaTime, moveSpeed * Mathf.Sin(angle) * Time.deltaTime, 0);
        moveSpeed *= .999f;
        transform.position += direction;

        if (lifetime <= 0) Destroy(gameObject);
    }
}
