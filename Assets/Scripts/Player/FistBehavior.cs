using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistBehavior : MonoBehaviour
{
    public Collider2D hitbox;
    public GameObject master;

    public GameObject comicEffect;
    private GameObject currComic;

    public int damage = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (currComic == null) SpawnComic(collision.gameObject.transform.position);

            collision.GetComponent<EnemyMain>().ApplyDamage(damage);
        }
        if(collision.CompareTag("Boss"))
        {
            if (currComic == null) SpawnComic(collision.gameObject.transform.position);
            collision.GetComponentInParent<BossAI>().ApplyDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            collision.gameObject.GetComponent<DebrisObstacle>().ApplyDamage(Mathf.Floor(damage / 2));
        }
    }

    /// <summary>
    /// When the fist collides with an enemy, spawn a word sound effect randomly in the direction they were punched
    /// </summary>
    /// <param name="enemyPos"></param>
    private void SpawnComic(Vector2 enemyPos)
    {
        var dir = (master.transform.localScale.x < 0) ? "right" : "left";

        float rand = 0;
        if (dir == "left") rand = Random.Range(0, Mathf.PI / 3);
        if (dir == "right") rand = Random.Range(2 * Mathf.PI / 3, Mathf.PI);

        currComic = Instantiate(comicEffect, new Vector3(enemyPos.x + Mathf.Cos(rand) * 4, enemyPos.y + Mathf.Sin(rand) * 4, 0), Quaternion.identity);
    }
}
