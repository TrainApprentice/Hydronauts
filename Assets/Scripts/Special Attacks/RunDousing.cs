using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunDousing : MonoBehaviour
{
    private FireObstacle currFire;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            currFire = other.GetComponent<FireObstacle>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Fire"))
        {
            collision.GetComponent<FireObstacle>().ApplyDamage();
        }
    }
}
