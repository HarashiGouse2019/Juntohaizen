using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCurosrMovement : MonoBehaviour
{
    //[Header("Pause Menu Curosr")]
    public Image pauseMenuCursorImage;

    //CursorPositioning
    public float[] position;
    float x = -295.75f, y = 268.26f;
    // Update is called once per frame
    void Update()
    {
        position = new float[2];
        position[0] = x;
        position[1] = y;
        transform.localPosition = new Vector2(position[0], position[1]);
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            y -= 85;
            Debug.Log(position[0] + "," + position[1]);
        } else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            y += 85;
            Debug.Log(position[0] + "," + position[1]);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            x -= 120;
            Debug.Log(position[0] + "," + position[1]);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            x += 120;
            Debug.Log(position[0] + "," + position[1]);
        }
        
    }
}
