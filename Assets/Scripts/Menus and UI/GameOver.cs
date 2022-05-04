using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject quitWarning;
    public GameObject levelSlots;
    public GameObject mainText;

    private void Start()
    {
        MusicManager.instance.SwitchTrack("Lose");
    }
    /// <summary>
    /// Restarts the level, reseting the game stats and loading an empty save file
    /// </summary>
    public void RestartLevel()
    {
        
        SceneManager.LoadScene("Level1");
        SaveFiles.instance.LoadGame(0);
        GameManager.instance.ResetGameStats();
    }

    /// <summary>
    /// Display the level slots menu and update all their information
    /// </summary>
    public void ShowLevelSlots()
    {
        mainText.SetActive(false);
        levelSlots.SetActive(true);
        SaveFiles.instance.UpdateAllSlots();
    }
    /// <summary>
    /// Hide the level slots menu
    /// </summary>
    public void HideLevelSlots()
    {
        levelSlots.SetActive(false);
        mainText.SetActive(true);
    }
    /// <summary>
    /// Sends the player back to the main menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MusicManager.instance.SwitchTrack("Menu");
    }
    /// <summary>
    /// Load the game from the selected slot and send the player to the game screen
    /// </summary>
    /// <param name="slotNum"></param>
    public void LoadGame(int slotNum)
    {
        // Replace with load game
        SaveFiles.instance.LoadGame(slotNum);
        SceneManager.LoadScene("Level1");
        
    }
    /// <summary>
    /// Display the quit warning
    /// </summary>
    public void ShowQuitWarning()
    {
        quitWarning.SetActive(true);
        mainText.SetActive(false);
    }
    /// <summary>
    /// Hide the quit warning
    /// </summary>
    public void HideQuitWarning()
    {
        quitWarning.SetActive(false);
        mainText.SetActive(true);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
