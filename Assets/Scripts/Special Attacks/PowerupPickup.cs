using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public int powerupType = 0; // 1 for blast, 2 for sprinkler
    public Sprite blast, sprinkler;

    // Start is called before the first frame update
    void Start()
    {
        switch(powerupType)
        {
            case 1:
                GetComponent<SpriteRenderer>().sprite = blast;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = sprinkler;
                break;
        }
        
    }

    private void OnDestroy()
    {
        GameManager.instance.ShowSpecialTutorial();
    }
}
