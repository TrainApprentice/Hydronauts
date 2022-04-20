using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMove : MonoBehaviour
{

    public Transform finalPos;
    private Vector2 startPos;
    private Vector3 goalPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        goalPos = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (goalPos != transform.position)
        {
            transform.position = AnimMath.Ease(transform.position, goalPos, .001f);
        }
        if (Vector3.Distance(goalPos, transform.position) < .0001f) transform.position = goalPos;
    }

    /// <summary>
    /// This function sets the goal position of the UI element, which will then be eased toward
    /// </summary>
    /// <param name="fromOrToStart">Determines which direction the movement happens (true: to finalPos, false: to startPos)</param>
    public void SetNewPosition(bool fromOrToStart)
    {
        if (fromOrToStart) goalPos = finalPos.position;
        else goalPos = startPos;
    }
}
