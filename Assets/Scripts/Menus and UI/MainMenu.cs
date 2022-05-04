using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{


    private void Update()
    {
        // If space is pressed, go to the next scene
        if (Input.GetKeyDown("space")) SceneManager.LoadScene("SaveLoad");
    }
}
