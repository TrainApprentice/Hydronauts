using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject quitWarning;
    public GameObject levelSlots;
    public GameObject mainText;
    public void RestartLevel()
    {
        
        SceneManager.LoadScene("Level1");
        GameManager.instance.ResetGameStats();
    }

    public void ShowLevelSlots()
    {
        mainText.SetActive(false);
        levelSlots.SetActive(true);
        SaveFiles.instance.UpdateAllSlots();
    }
    public void HideLevelSlots()
    {
        levelSlots.SetActive(false);
        mainText.SetActive(true);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadGame(int slotNum)
    {
        // Replace with load game
        SaveFiles.instance.LoadGame(slotNum);
        SceneManager.LoadScene("Level1");
        
    }
    public void ShowQuitWarning()
    {
        // Pop up Are You Sure?
        // Unsaved progress will be lost
        quitWarning.SetActive(true);
        mainText.SetActive(false);
    }

    public void HideQuitWarning()
    {
        quitWarning.SetActive(false);
        mainText.SetActive(true);
    }

    
    public void QuitGame()
    {
        Application.Quit();
    }
}
