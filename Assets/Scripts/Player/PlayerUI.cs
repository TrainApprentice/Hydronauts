using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    public PlayerMain playerRef;
    public GameObject healthBar;
    public GameObject specialMeterNeedle;

    private List<Image> bars = new List<Image>();

    private void Start()
    {
        
    }

    private void Update()
    {
        UpdateSpecialMeter();
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        float p = (float)playerRef.health / playerRef.maxHealth;
        Vector3 scaleGoal = new Vector3(60 * p, 20, 1);

        healthBar.transform.localScale = (p != 1) ? AnimMath.Ease(healthBar.transform.localScale, scaleGoal, .001f) : scaleGoal;
        
    }

    void UpdateSpecialMeter()
    {
        float p = playerRef.specialMeter / playerRef.maxSpecialMeter;

        float angleZ = AnimMath.Lerp(120, -120, p);

        Quaternion goalRot = Quaternion.Euler(0, 0, angleZ);

        specialMeterNeedle.transform.localRotation = (p!= 0) ? AnimMath.Ease(specialMeterNeedle.transform.localRotation, goalRot, .001f, false) : goalRot;
    }
}
