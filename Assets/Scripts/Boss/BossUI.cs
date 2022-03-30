using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{

    private Transform healthBar;
    private float prevHealth = 0;
    private int currHealth = 100;
    private float introTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(!healthBar) healthBar = FindObjectOfType<HealthBar>().transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (prevHealth < currHealth && introTimer != 1)
        {
            if (introTimer < 1) introTimer += Time.deltaTime / 3;
            else
            {
                introTimer = 1;
                prevHealth = currHealth;
            }
            float barWidth = AnimMath.Lerp(prevHealth, currHealth, introTimer);
            healthBar.localScale = new Vector3(barWidth / 100, 1, 1);
        }
        if(currHealth < prevHealth)
        {
            prevHealth = AnimMath.Ease(prevHealth, currHealth, .01f);
            if (prevHealth - currHealth < .01f) prevHealth = currHealth;
            healthBar.localScale = new Vector3(prevHealth / 100, 1, 1);
        }
        if (prevHealth <= .1f && currHealth <= 0) Destroy(gameObject);
        
    }

    public void SetCurrentHealth(int num)
    {
        currHealth = num;
    }
}
