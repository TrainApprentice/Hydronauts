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

    // Start is called before the first frame update
    void Start()
    {
        float randX = Random.Range(17, 31);
        float randY = Random.Range(-19, -26);
        transform.position = new Vector3(randX, randY);

        currProjectile = Instantiate(projectile, new Vector3(transform.position.x, transform.position.y + 50), Quaternion.identity);
        projectileStartPos = currProjectile.transform.position;
        currProjectile.GetComponent<DebrisCollision>().owner = gameObject;
        currSprite = Random.Range(0, 4);
        currProjectile.GetComponent<SpriteRenderer>().sprite = debrisSprites[currSprite];
        debrisHitbox = currProjectile.GetComponent<CircleCollider2D>();

        shadowSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime/2;

        float p = 1 - lifetime;
        if (lifetime > .5) shadowSprite.color = new Color(.1f, .1f, .1f, p);
        else shadowSprite.color = Color.black;

        currProjectile.transform.position = AnimMath.Lerp(projectileStartPos, transform.position, p);
        if(lifetime < .2f)
        {
            debrisHitbox.enabled = true;
        }

        if(lifetime <= 0)
        {
            Destroy(currProjectile);
            Destroy(gameObject);
        }
    }
}
