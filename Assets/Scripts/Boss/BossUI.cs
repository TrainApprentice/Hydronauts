using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{

    private Transform healthBar;
    /// <summary>
    /// The previous health value of the boss before taking damage, used to scale the health bar
    /// </summary>
    private float prevHealth = 0;
    /// <summary>
    /// The current value of the boss' health
    /// </summary>
    private float currHealth = 200;

    /// <summary>
    /// A counter for the boss' intro sequence where the health bar fills up
    /// </summary>
    private float introTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        if(!healthBar) healthBar = FindObjectOfType<HealthBar>().transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Increases the healthbar for the boss' intro sequence
        if (prevHealth < currHealth && introTimer != 1)
        {
            if (introTimer < 1) introTimer += Time.deltaTime / 3;
            else
            {
                introTimer = 1;
                prevHealth = currHealth;
            }
            float barWidth = AnimMath.Lerp(prevHealth, currHealth, introTimer);
            healthBar.localScale = new Vector3(barWidth / 200, 1, 1);
        }
        // Runs the easing of the healthbar from the previous health to the current health
        if(currHealth < prevHealth)
        {
            prevHealth = AnimMath.Ease(prevHealth, currHealth, .01f);
            if (prevHealth - currHealth < .01f) prevHealth = currHealth;
            healthBar.localScale = new Vector3(prevHealth / 200, 1, 1);
        }
        // If health is too low, destroy the healthbar
        if (prevHealth <= .1f && currHealth <= 0)
        {
            Destroy(gameObject);
        }
        
    }
    /// <summary>
    /// This sets the current health of the boss from the BossAI, setting off the easing for the healthbar
    /// </summary>
    /// <param name="num"></param>
    public void SetCurrentHealth(float num)
    {
        currHealth = num;
    }
}
