using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    private void Start()
    {
        if (!SaveFiles.instance) return;

        SaveFiles.instance.SaveGame(GameManager.instance.player, new Vector3(30, -22), SaveFiles.instance.chosenSlot, GameManager.instance.totalKills, 1, 4);
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
