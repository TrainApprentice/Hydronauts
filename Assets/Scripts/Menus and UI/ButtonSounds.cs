using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    public AudioClip buttonHover, buttonClick;
    private AudioSource main;

    private void Start()
    {
        main = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Plays the stored hover sound effect
    /// </summary>
    public void PlayHover()
    {
        main.clip = buttonHover;
        main.Play();
    }
    /// <summary>
    /// Plays the stored click sound effect
    /// </summary>
    public void PlayClick()
    {
        main.clip = buttonClick;
        main.Play();
    }

}
