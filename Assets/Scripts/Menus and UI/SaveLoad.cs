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

    private int currSlotSelection = 0;

    public void ShowLoadDelete(int slotNum)
    {
        fileChoice.SetActive(true);
        fileChoice.transform.position = new Vector3(4.63f, 3 * (2 - slotNum), 1);
        currSlotSelection = slotNum;
    }

    public void HideLoadDelete()
    {
        fileChoice.SetActive(false);
    }
    
    public void LoadGame()
    {
        SaveFiles.instance.LoadGame(currSlotSelection);
        SceneManager.LoadScene("Level1");
    }

    public void ShowDeleteWarning()
    {
        deleteWarning.SetActive(true);
    }

    public void HideDeleteWarning()
    {
        deleteWarning.SetActive(false);
    }

    public void DeleteSave()
    {
        SaveFiles.instance.DeleteData(currSlotSelection);
        HideDeleteWarning();
        HideLoadDelete();
    }

    public void ShowQuitWarning()
    {
        quitWarning.SetActive(true);
    }

    public void HideQuitWarning()
    {
        quitWarning.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("EndScene");
    }
}
