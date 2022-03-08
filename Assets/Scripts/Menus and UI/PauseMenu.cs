using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void FindMenuElements()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        saveSlots = GameObject.FindGameObjectWithTag("SaveSlots");
        quitWarning = GameObject.FindGameObjectWithTag("QuitWarning");
        confirmOverwrite = GameObject.FindGameObjectWithTag("ConfirmOverwrite");
        menuWarning = GameObject.FindGameObjectWithTag("MenuWarning");

        

        SetButtonFunctions();

        pauseMenu.SetActive(false);
        saveSlots.SetActive(false);
        quitWarning.SetActive(false);
        confirmOverwrite.SetActive(false);
        menuWarning.SetActive(false);
    }

    private void SetButtonFunctions()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach(Button b in allButtons)
        {
            b.onClick.RemoveAllListeners();
            switch(b.gameObject.name)
            {
                case "Resume":
                    b.onClick.AddListener(GameManager.instance.ResumeGame);
                    break;
                case "Save":
                    b.onClick.AddListener(ShowSaveSlots);
                    break;
                case "Main Menu":
                    b.onClick.AddListener(ShowMenuWarning);
                    break;
                case "Quit":
                    b.onClick.AddListener(ShowQuitWarning);
                    break;
                case "ConfirmMenu":
                    b.onClick.AddListener(BackToMenu);
                    break;
                case "BackFromMenu":
                    b.onClick.AddListener(HideMenuWarning);
                    break;
                case "ConfirmQuit":
                    b.onClick.AddListener(QuitGame);
                    break;
                case "BackFromQuit":
                    b.onClick.AddListener(HideQuitWarning);
                    break;
                case "Slot1":
                    UnityEngine.Events.UnityAction temp1 = () => { ShowConfirmOverwrite(1); };
                    b.onClick.AddListener(temp1);
                    break;
                case "Slot2":
                    UnityEngine.Events.UnityAction temp2 = () => { ShowConfirmOverwrite(2); };
                    b.onClick.AddListener(temp2);
                    break;
                case "Slot3":
                    UnityEngine.Events.UnityAction temp3 = () => { ShowConfirmOverwrite(3); };
                    b.onClick.AddListener(temp3);
                    break;
                case "ConfirmOverwrite":
                    b.onClick.AddListener(SaveGame);
                    break;
                case "BackFromOverwrite":
                    b.onClick.AddListener(HideConfirmOverwrite);
                    break;

                case "BackFromSlots":
                    b.onClick.AddListener(HideSaveSlots);
                    break;
            }
        }
    }
}
