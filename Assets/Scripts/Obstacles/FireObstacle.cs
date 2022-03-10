using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObstacle : MonoBehaviour
{
    float currHealth;
    float maxHealth;
    public float size;
    void Start()
    {
        size = Random.Range(2, 4);

        maxHealth = size;
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currHealth < 0) Destroy(gameObject);
        float p = currHealth / maxHealth;

        Vector3 smallestSize = new Vector3(size / 8, size / 8, 1);
        Vector3 largestSize = new Vector3(size, size, 1);

        transform.localScale = AnimMath.Lerp(smallestSize, largestSize, p);
    }

    public void ApplyDamage(float damage = .05f)
    {
        currHealth -= damage;
    }
}