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

    public void CheckSlot(int slotNum)
    {
        currSaveSlot = slotNum;
        if (!SaveFiles.instance.CheckDataInSlot(currSaveSlot))
        {
            print("Instant");
            SaveGame();
        }
        else
        {
            print("Delay");
            ShowConfirmOverwrite();
        }
    }
    public void ShowSaveSlots()
    {
        saveSlots.SetActive(true);
        pauseMenu.SetActive(false);
        SaveFiles.instance.UpdateAllSlots();
    }
    public void HideSaveSlots()
    {
        saveSlots.SetActive(false);
        pauseMenu.SetActive(true);
        confirmOverwrite.SetActive(false);
    }
    public void SaveGame()
    {
        if(SaveFiles.instance.CheckDataInSlot(currSaveSlot))
        {
            SaveFiles.instance.DeleteData(currSaveSlot);
        }
        // Save the game
        print("Game saved in slot " + currSaveSlot);
        PlayerMain player = GameManager.instance.player;
        int kills = GameManager.instance.totalKills;
        int encounter = GameManager.instance.maxEncounter;
        int level = (GameManager.instance.hasWon) ? 1 : 0;
        Vector3 position = (encounter > 0) ? GameManager.instance.encounterPos[encounter - 1].transform.position - new Vector3(3, 0) : new Vector3(-5, -.5f);
        SaveFiles.instance.SaveGame(player, position, currSaveSlot, kills, level, encounter);
        confirmOverwrite.SetActive(false);
        SaveFiles.instance.UpdateAllSlots();
    }

    public void ShowMenuWarning()
    {
        // Pop up Are You Sure?
        // Unsaved progress will be lost
        menuWarning.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void HideMenuWarning()
    {
        menuWarning.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ShowConfirmOverwrite()
    {
        confirmOverwrite.SetActive(true);
        saveSlots.SetActive(false);
    }
    public void HideConfirmOverwrite()
    {
        confirmOverwrite.SetActive(false);
        saveSlots.SetActive(true);
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
        pauseMenu.SetActive(false);
    }

    public void HideQuitWarning()
    {
        quitWarning.SetActive(false);
        pauseMenu.SetActive(true);
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
                case "Slot 1":
                    UnityEngine.Events.UnityAction temp1 = () => { CheckSlot(1); };
                    b.onClick.AddListener(temp1);
                    break;
                case "Slot 2":
                    UnityEngine.Events.UnityAction temp2 = () => { CheckSlot(2); };
                    b.onClick.AddListener(temp2);
                    break;
                case "Slot 3":
                    UnityEngine.Events.UnityAction temp3 = () => { CheckSlot(3); };
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
