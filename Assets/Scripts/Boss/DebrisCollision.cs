using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisCollision : MonoBehaviour
{
    public GameObject owner;
    public int damage = 2;

    /// <summary>
    /// Destroys the owner and itself
    /// </summary>
    public void DestroyMe()
    {
        Destroy(owner);
        Destroy(gameObject);
    }
}
