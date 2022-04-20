using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotDisplay : MonoBehaviour
{
    public int slot;

    public int enemiesKilled = 0;
    public int currLevel = 0;
    public string currSpecial = "";

    public TMP_Text slotNumber, specialText;
    public TMP_Text levelDisplay, killDisplay;
    public Image specialImageDisplay;
   

    public Sprite blastSprite, sprinklerSprite;
    public Sprite emptySave, filledSave;
    
    public void UpdateVisuals()
    {
        if(enemiesKilled > 0 && currSpecial != "")
        {
            slotNumber.gameObject.SetActive(false);
            specialImageDisplay.gameObject.SetActive(true);
            specialText.gameObject.SetActive(true);
            killDisplay.text = "Total Enemies Defeated: " + enemiesKilled;
            levelDisplay.text = "Levels Completed: " + currLevel;
            GetComponent<Image>().sprite = filledSave;
            switch (currSpecial)
            {
                case "blast":
                    specialImageDisplay.color = new Color(1, 1, 1);
                    specialImageDisplay.sprite = blastSprite;
                    break;
                case "sprinkler":
                    specialImageDisplay.color = new Color(1, 1, 1);
                    specialImageDisplay.sprite = sprinklerSprite;
                    break;
                default:
                    specialImageDisplay.color = new Color(1, 1, 1, 0);
                    break;
            }
        }
        else
        {
            slotNumber.gameObject.SetActive(true);
            killDisplay.text = "";
            levelDisplay.text = "";
            specialImageDisplay.gameObject.SetActive(false);
            specialText.gameObject.SetActive(false);
            GetComponent<Image>().sprite = emptySave;
        }
    }
}
