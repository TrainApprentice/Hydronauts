using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{

    private float walkTimer = 0f;
    private float timeBetweenMovements = 2f;

    private Vector3 currLocation;
    private Vector3 nextLocation;
    private Vector3 prevLocation;


    // Start is called before the first frame update
    void Start()
    {
        currLocation = transform.position;
        prevLocation = currLocation;
        nextLocation = currLocation;
    }

    // Update is called once per frame
    void Update()
    {
        if (currLocation != nextLocation)
        {
            walkTimer += Time.deltaTime;
            currLocation = AnimMath.Lerp(prevLocation, nextLocation, walkTimer);
            print(currLocation);
        }
        transform.position = currLocation;
    }

    public void SetNewLocation(Vector3 newPos, int attackType = 0)
    {
        prevLocation = currLocation;
        nextLocation = newPos;

        if(attackType > 0)
        {

        }
    }
}
