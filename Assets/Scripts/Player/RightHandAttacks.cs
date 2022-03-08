//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandAttacks : MonoBehaviour
{
    public Collider2D hitbox;
    public GameObject master;

    public GameObject comicEffect;
    private GameObject currComic;

    private float damage = 5f;
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (currComic == null) SpawnComic(collision.gameObject.transform.position);
            
            collision.gameObject.GetComponent<EnemyMain>().ApplyDamage(damage);
        }
    }

    private void SpawnComic(Vector2 enemyPos)
    {
        var dir = (master.transform.localScale.x < 0) ? "right" : "left";

        float rand = 0;
        if (dir == "left") rand = Random.Range(0, Mathf.PI / 3);
        if (dir == "right") rand = Random.Range(2 * Mathf.PI / 3, Mathf.PI);

        currComic = Instantiate(comicEffect, new Vector3(enemyPos.x + Mathf.Cos(rand) * 4, enemyPos.y + Mathf.Sin(rand) * 4, 0), Quaternion.identity);
    }
}


