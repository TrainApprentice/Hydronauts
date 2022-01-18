using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject target;
    public bool isFrozen = false;
    public bool isThawing = false;

    float alpha = 0;

    private Vector3 setPosition;

    private bool setLerpOnce = true;
    private float baseX, baseY;
    

    // Update is called once per frame
    void Update()
    {
        if (isThawing && isFrozen)
        {

            if(setLerpOnce)
            {
                baseX = transform.position.x;
                baseY = transform.position.y;
                setLerpOnce = false;
            }
            
            if(alpha < .3333f)
            {
                alpha += Time.deltaTime;
                setPosition.x = Mathf.Lerp(baseX, target.transform.position.x, alpha*3);
                setPosition.y = Mathf.Lerp(baseY, target.transform.position.y, alpha*3);
                setPosition.z = -10;
                transform.position = setPosition;
            }
           
            else
            {
                isFrozen = false;
                isThawing = false;
                setLerpOnce = true;
                alpha = 0;
            }

        }
        else if(!isThawing && !isFrozen)
        {
            setPosition.x = (target.transform.position.x - transform.position.x > 4) ? target.transform.position.x - 4 : (target.transform.position.x - transform.position.x < -4) ? target.transform.position.x + 4 : transform.position.x;
            setPosition.y = (target.transform.position.y - transform.position.y > 2) ? target.transform.position.y - 2 : (target.transform.position.y - transform.position.y < -2) ? target.transform.position.y + 2 : transform.position.y;
            setPosition.z = -10;
            transform.position = setPosition;
        }
    }

    public void SwapFreeze(bool setCenter)
    {

        if (!isFrozen)
        {
            if (setCenter)
            {
                setPosition.y = -3f;
                transform.position = setPosition;
            }
            isFrozen = true;
        }
        else isThawing = true;
        
    }

    
}
