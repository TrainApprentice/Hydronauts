using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    public GameObject fileChoice;
    public GameObject deleteWarning;

    private int currSlotSelection = 0;

    public void ShowLoadDelete(int slotNum)
    {
        fileChoice.SetActive(true);
        print(fileChoice.transform.position.y);
        fileChoice.transform.position = new Vector3(4.63f, 3 * (2 - slotNum), 1);
        currSlotSelection = slotNum;
    }

    public void HideLoadDelete()
    {
        fileChoice.SetActive(false);
    }
    
    public void LoadGame()
    {
        // Replace with load game
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
        // Pop up Are You Sure?
        print("Save Deleted from slot " + currSlotSelection);
        HideDeleteWarning();
        HideLoadDelete();

    }
}
