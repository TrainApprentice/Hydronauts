using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    private int currScreen = 1;
    private bool swapOnce = true;
    private float swapTimer = 10f;

    public Sprite credits1, credits2;
    public Image image;

    private void Start()
    {
        if(MusicManager.instance)
        {
            if (MusicManager.instance.currTrack == "Win") SaveWinGame();
        }
    }

    private void Update()
    {
        // Countdown the swap timer
        if (swapTimer > 0) swapTimer -= Time.deltaTime;
        else
        {
            StartCoroutine("ImgFade");
            swapOnce = true;
            swapTimer = 10f;
        }
        
    }
    /// <summary>
    /// Sends the player back to the title screen
    /// </summary>
    public void BackToTitle()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Save the winning game data to the last known used save file
    /// </summary>
    public void SaveWinGame()
    {
        if (!SaveFiles.instance) return;

        SaveFiles.instance.SaveGame(GameManager.instance.player, new Vector3(30, -22), SaveFiles.instance.chosenSlot, GameManager.instance.totalKills + 1, 1, 4);
    }

    /// <summary>
    /// Fades the image in and out over time for the credits
    /// </summary>
    /// <returns></returns>
    private IEnumerator ImgFade()
    {
        for (float a = 1; a > -1f; a -= .02f)
        {
            Color fade = image.color;
            fade.a = (a >= 0) ? a : -a;
            image.color = fade;
            if (a <= 0)
            {
                if(swapOnce)
                {
                    if (currScreen == 1) currScreen = 2;
                    else if (currScreen == 2) currScreen = 1;
                    SwitchImage();
                    swapOnce = false;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Switch the images between credits screens
    /// </summary>
    private void SwitchImage()
    {
        switch(currScreen)
        {
            case 1:
                image.sprite = credits1;
                break;
            case 2:
                image.sprite = credits2;
                break;
        }
    }
}
