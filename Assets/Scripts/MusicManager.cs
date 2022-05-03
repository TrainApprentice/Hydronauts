using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip menuTrack, gameplayTrack, bossTrack, winTrack, loseTrack;
    public AudioSource main;
    public string currTrack;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);

        }

        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SwitchTrack("Menu");
        }
    }
    /// <summary>
    /// This method switches the track currently being player by the Music Manager with various string options
    /// </summary>
    /// <param name="newTrack">The "name" of the track to be played</param>
    public void SwitchTrack(string newTrack)
    {
        main.Stop();
        switch(newTrack)
        {
            case "Menu":
                main.clip = menuTrack;
                break;
            case "Gameplay":
                main.clip = gameplayTrack;
                break;
            case "Boss":
                main.clip = bossTrack;
                break;
            case "Lose":
                main.clip = loseTrack;
                break;
            case "Win":
                main.clip = winTrack;
                break;
            default:
                break;
        }
        currTrack = newTrack;
        main.Play();
    }
}
