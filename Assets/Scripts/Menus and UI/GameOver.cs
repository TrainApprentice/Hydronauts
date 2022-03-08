using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject quitWarning;
    public GameObject levelSlots;
    public void RestartLevel()
    {
        SceneManager.LoadScene("Level1");
    }

    public void ShowLevelSlots()
    {
        levelSlots.SetActive(true);
    }
    public void HideLevelSlots()
    {
        levelSlots.SetActive(false);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadGame(int slotNum)
    {
        // Replace with load game
        SceneManager.LoadScene("Level1");
    }
    public void ShowQuitWarning()
    {
        // Pop up Are You Sure?
        // Unsaved progress will be lost
        quitWarning.SetActive(true);
    }

    public void HideQuitWarning()
    {
        quitWarning.SetActive(false);
    }

    // Also for Game Over
    public void QuitGame()
    {
        Application.Quit();
    }
}
