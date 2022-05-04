using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComicFade : MonoBehaviour
{

    private float lifespan = .5f;
    private TMP_Text displayText;
    // Start is called before the first frame update
    void Start()
    {
        displayText = GetComponent<TMP_Text>();
        RandomText();
    }

    // Update is called once per frame
    void Update()
    {
        // Countdown the lifespan
        lifespan -= Time.deltaTime;

        // Increase the comic's size over time for most of the duration
        if (lifespan > .25f) displayText.fontSize += Time.deltaTime * 8;

        if (lifespan <= 0) Destroy(gameObject);
    }

    /// <summary>
    /// Randomizes the text displayed by the comic efect
    /// </summary>
    void RandomText()
    {
        int rand = Mathf.FloorToInt(Random.Range(0, 5));
        string newText = "";

        switch(rand)
        {
            case 0:
                newText = "Bam!";
                break;
            case 1:
                newText = "Whack!";
                break;
            case 2:
                newText = "Smack!";
                break;
            case 3:
                newText = "Clang!";
                break;
            case 4:
                newText = "Oof!";
                break;
            default:
                newText = "Bam!";
                break;
        }
        displayText.text = newText;
    }
}
