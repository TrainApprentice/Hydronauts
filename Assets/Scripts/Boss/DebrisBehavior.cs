using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBehavior : MonoBehaviour
{
    public GameObject projectile;
    public Sprite[] debrisSprites = new Sprite[4];
    private GameObject currProjectile;
    private Vector3 projectileStartPos;
    private float lifetime = 1f;
    private int currSprite;
    private SpriteRenderer shadowSprite;
    private CircleCollider2D debrisHitbox;
    private float damage = 2;

    void Start()
    {
        // Randomize the spawn point somewhere in the boss arena
        float randX = Random.Range(17, 31);
        float randY = Random.Range(-19, -26);
        transform.position = new Vector3(randX, randY);

        // Create the actual projectile and set everything up about it
        currProjectile = Instantiate(projectile, new Vector3(transform.position.x, transform.position.y + 50), Quaternion.identity);
        projectileStartPos = currProjectile.transform.position;
        currProjectile.GetComponent<DebrisCollision>().owner = gameObject;
        currSprite = Random.Range(0, 4);
        currProjectile.GetComponent<SpriteRenderer>().sprite = debrisSprites[currSprite];
        debrisHitbox = currProjectile.GetComponent<CircleCollider2D>();

        // Grab the shadow sprite for fading
        shadowSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Countdown the shadow's lifetime
        lifetime -= Time.deltaTime/2;

        // Increase opacity over time
        float p = 1 - lifetime;
        if (lifetime > .5) shadowSprite.color = new Color(.1f, .1f, .1f, p);
        else shadowSprite.color = Color.black;

        // Move the actual projectile down toward the shadow
        currProjectile.transform.position = AnimMath.Lerp(projectileStartPos, transform.position, p);

        // Activate the hitbox at the correct time
        if(lifetime < .2f)
        {
            debrisHitbox.enabled = true;
        }

        // If lifetime's expired, kill both the debris and the shadow
        if(lifetime <= 0)
        {
            Destroy(currProjectile);
            Destroy(gameObject);
        }
    }
}
