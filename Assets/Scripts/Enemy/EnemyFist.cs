using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFist : MonoBehaviour
{
    // Checks if the fist hits the player, dealing damage if it does
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMain>().ApplyDamage(2);
        }
    }
}
