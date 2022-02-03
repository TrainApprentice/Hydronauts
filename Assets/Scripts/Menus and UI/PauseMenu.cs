using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject saveSlots;
    public GameObject menuWarning, quitWarning, confirmOverwrite;

    private int currSaveSlot = 0;

    public void ShowSaveSlots()
    {
        saveSlots.SetActive(true);
    }
    public void HideSaveSlots()
    {
        saveSlots.SetActive(false);
        confirmOverwrite.SetActive(false);
    }
    public void SaveGame()
    {

        // Save the game
        print("Game saved in slot " + currSaveSlot);
        confirmOverwrite.SetActive(false);
    }

    public void ShowMenuWarning()
    {
        // Pop up Are You Sure?
        // Unsaved progress will be lost
        menuWarning.SetActive(true);
        
    }
    public void HideMenuWarning()
    {
        menuWarning.SetActive(false);
    }

    public void ShowConfirmOverwrite(int slotNum)
    {
        confirmOverwrite.SetActive(true);
        currSaveSlot = slotNum;
    }
    public void HideConfirmOverwrite()
    {
        confirmOverwrite.SetActive(false);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Also for Game Over
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
