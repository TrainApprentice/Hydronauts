using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMove : MonoBehaviour
{

    public string title;
    /// <summary>
    /// Displays or hides the tutorial from outside the class
    /// </summary>
    /// <param name="visibility"></param>
    public void SwitchOnOff(bool visibility)
    {
        gameObject.SetActive(visibility);
    }
}
