using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObstacle : MonoBehaviour
{
    float currHealth;
    float maxHealth;
    float size;
    float soundTimer = 0;
    float maxDist = 10;
    float minDist = 2;
    public int damage;

    public AudioClip ambient, doused;
    private AudioSource sfx;
    private PlayerMain player;
    void Start()
    {
        // Randomize the size of the flame
        size = Random.Range(2, 4);
        damage = Mathf.FloorToInt(size);

        maxHealth = size;
        currHealth = maxHealth;
        player = FindObjectOfType<PlayerMain>();
        sfx = GetComponent<AudioSource>();

        // Start the ambient sound loop
        sfx.loop = true;
        sfx.clip = ambient;
        sfx.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // If health is below zero, switch the sound playing, hide the dousing tutorial, and heal the player
        if (currHealth < 0)
        {
            GameManager.instance.HideDousingTutorial();
            player.ApplyDamage(-1);
            sfx.clip = doused;
            sfx.volume = .7f;
            sfx.loop = false;
            sfx.Play();
            soundTimer = .8f;
            currHealth = 0;
        }
        // Once the flame is dead, let the doused sound play out before destroying the obstacle
        if (currHealth == 0)
        {
            soundTimer -= Time.deltaTime;
            if (soundTimer <= 0) Destroy(gameObject);
        }
        
        CheckPlayerDistance();

        // Update the flame's size based on its current health
        float p = currHealth / maxHealth;

        Vector3 smallestSize = (currHealth == 0) ? Vector3.zero : new Vector3(size / 8, size / 8, 1);
        Vector3 largestSize = new Vector3(size, size, 1);

        transform.localScale = AnimMath.Lerp(smallestSize, largestSize, p);
    }

    /// <summary>
    /// Called outside the class to deal damage to the fire obstacle
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(float damage = .05f)
    {
        if(currHealth > 0) currHealth -= damage;
    }

    /// <summary>
    /// Check the distance between this obstacle and the player, and change the volume of the ambient fire accordingly
    /// </summary>
    private void CheckPlayerDistance()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist > maxDist) sfx.volume = 0;
        else if (dist <= minDist) sfx.volume = .4f;
        else
        {
            float mapVal = AnimMath.Map(dist, maxDist, minDist, 0, .5f);
            sfx.volume = mapVal;
        }
    }
}
