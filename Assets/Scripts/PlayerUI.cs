using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    public PlayerMain playerRef;

    public GameObject healthBar;

    private Image health;

    public Transform healthBarBase;

    private List<Image> bars = new List<Image>();

    private void Start()
    {
        
        health = healthBar.GetComponent<Image>();
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        print(playerRef.health);
        
        foreach(Image i in bars)
        {
            Destroy(i.gameObject);
        }
        bars.Clear();
        for (int i = 0; i < playerRef.health; i++)
        {
            Image thing = Instantiate(health, healthBarBase);
            thing.transform.position += new Vector3(0, i/4f);
            bars.Add(thing);
            
        }
        //healthBar.GetComponent<RectTransform>().transform. = playerRef.health * 40;
    }
}
