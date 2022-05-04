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
    
    /// <summary>
    /// Updates the visuals of the save slot with appropriate information if there's save data, or clearing it if there isn't
    /// </summary>
    /// <param name="isCleared"></param>
    public void UpdateVisuals(bool isCleared)
    {
        // If there's no save data, clear the display
        if (isCleared)
        {
            slotNumber.gameObject.SetActive(true);
            killDisplay.text = "";
            levelDisplay.text = "";
            specialImageDisplay.gameObject.SetActive(false);
            specialText.gameObject.SetActive(false);
            GetComponent<Image>().sprite = emptySave;
        }
        // Otherwise, show relevant information
        else
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
        
    }
}
