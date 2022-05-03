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
    public void PlayHover()
    {
        main.clip = buttonHover;
        main.Play();
    }

    public void PlayClick()
    {
        main.clip = buttonClick;
        main.Play();
    }

}
