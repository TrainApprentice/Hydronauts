using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisObstacle : MonoBehaviour
{
    private float currHealth, maxHealth;
    private float size;

    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        size = Random.Range(2, 5);
        maxHealth = size;
        currHealth = maxHealth;

        sprite = GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(size, size, 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the opacity based on current health
        float alpha = currHealth / maxHealth;
        sprite.color = new Color(1, 1, 1, alpha);

        if (currHealth <= 0)
        {
            PlayerMain player = FindObjectOfType<PlayerMain>();
            player.ApplyDamage(-1);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called outside the class to damage the obstacle
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(float damage)
    {
        currHealth -= damage;
        
    }
}
