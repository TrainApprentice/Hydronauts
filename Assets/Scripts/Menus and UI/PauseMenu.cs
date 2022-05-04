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

    public ButtonSounds sound;

    private int currSaveSlot = 0;

    /// <summary>
    /// When a slot is clicked, see if it has save data in there already. If not, save the game. If so, ask to confirm overwrite
    /// </summary>
    /// <param name="slotNum"></param>
    public void CheckSlot(int slotNum)
    {
        currSaveSlot = slotNum;
        if (!SaveFiles.instance.CheckDataInSlot(currSaveSlot))
        {
            SaveGame();
        }
        else
        {
            ShowConfirmOverwrite();
        }
    }
    /// <summary>
    /// Display the save slots menu, and update all slots with their information
    /// </summary>
    public void ShowSaveSlots()
    {
        saveSlots.SetActive(true);
        pauseMenu.SetActive(false);
        SaveFiles.instance.UpdateAllSlots();
    }
    /// <summary>
    /// Hide save slots display
    /// </summary>
    public void HideSaveSlots()
    {
        saveSlots.SetActive(false);
        pauseMenu.SetActive(true);
        confirmOverwrite.SetActive(false);
    }
    /// <summary>
    /// Saves the game, taking all relevant information from the GameManager and calling on the SaveFiles
    /// If there was data in the slot being saved to, delete it
    /// </summary>
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

    /// <summary>
    /// Displays the menu warning for returning to the main menu
    /// </summary>
    public void ShowMenuWarning()
    {
        menuWarning.SetActive(true);
        pauseMenu.SetActive(false);
    }
    /// <summary>
    /// Hides the menu warning
    /// </summary>
    public void HideMenuWarning()
    {
        menuWarning.SetActive(false);
        pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Displays the warning for overwriting a save file
    /// </summary>
    public void ShowConfirmOverwrite()
    {
        confirmOverwrite.SetActive(true);
    }
    /// <summary>
    /// Hides the warning for overwriting a save file
    /// </summary>
    public void HideConfirmOverwrite()
    {
        confirmOverwrite.SetActive(false);
    }

    /// <summary>
    /// Returns the player to the main menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Display the quit warning, all unsaved progress will be lost
    /// </summary>
    public void ShowQuitWarning()
    {
        quitWarning.SetActive(true);
        pauseMenu.SetActive(false);
    }
    /// <summary>
    /// Hide the quit warning
    /// </summary>
    public void HideQuitWarning()
    {
        quitWarning.SetActive(false);
        pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
    /// <summary>
    /// Re-finds all menus and buttons upon returning to the game scene after loading
    /// </summary>
    public void FindMenuElements()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        saveSlots = GameObject.FindGameObjectWithTag("SaveSlots");
        quitWarning = GameObject.FindGameObjectWithTag("QuitWarning");
        confirmOverwrite = GameObject.FindGameObjectWithTag("ConfirmOverwrite");
        menuWarning = GameObject.FindGameObjectWithTag("MenuWarning");
        sound = FindObjectOfType<ButtonSounds>();
        

        SetButtonFunctions();
        pauseMenu.SetActive(false);
        saveSlots.SetActive(false);
        quitWarning.SetActive(false);
        confirmOverwrite.SetActive(false);
        menuWarning.SetActive(false);
    }

    /// <summary>
    /// Sets all pause menu buttons with their appropriate onClick functions
    /// </summary>
    private void SetButtonFunctions()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach(Button b in allButtons)
        {
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(sound.PlayClick);
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
