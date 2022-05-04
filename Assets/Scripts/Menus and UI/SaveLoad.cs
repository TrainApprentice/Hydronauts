using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    public GameObject fileChoice;
    public GameObject deleteWarning;
    public GameObject quitWarning;

    /// <summary>
    /// Stores the last save slot that was clicked
    /// </summary>
    private int currSlotSelection = 0;

    /// <summary>
    /// Show the load/delete menu based on which slot was clicked
    /// </summary>
    /// <param name="slotNum"></param>
    public void ShowLoadDelete(int slotNum)
    {
        fileChoice.SetActive(true);
        fileChoice.transform.position = new Vector3(4.63f, 3 * (2 - slotNum), 1);
        currSlotSelection = slotNum;
    }

    /// <summary>
    /// Hide the load/delete menu
    /// </summary>
    public void HideLoadDelete()
    {
        fileChoice.SetActive(false);
    }
    
    /// <summary>
    /// Load the game from the chosen slot and send the palyer to the game scene
    /// </summary>
    public void LoadGame()
    {
        SaveFiles.instance.LoadGame(currSlotSelection);
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Displays the warning for deleting save data
    /// </summary>
    public void ShowDeleteWarning()
    {
        deleteWarning.SetActive(true);
    }
    /// <summary>
    /// Hides the warning for deleting save data
    /// </summary>
    public void HideDeleteWarning()
    {
        deleteWarning.SetActive(false);
    }

    /// <summary>
    /// Calls to SaveFiles to delete the data in the chosen slot
    /// </summary>
    public void DeleteSave()
    {
        SaveFiles.instance.DeleteData(currSlotSelection);
        HideDeleteWarning();
        HideLoadDelete();
    }

    /// <summary>
    /// Displays the warning for quitting the game
    /// </summary>
    public void ShowQuitWarning()
    {
        quitWarning.SetActive(true);
    }
    /// <summary>
    /// Hides the warning for quitting the game
    /// </summary>
    public void HideQuitWarning()
    {
        quitWarning.SetActive(false);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Send the player to the credits screen
    /// </summary>
    public void GoToCredits()
    {
        SceneManager.LoadScene("EndScene");
    }
}
