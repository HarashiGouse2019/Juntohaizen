using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor_Image : MonoBehaviour
{
    int w = 32; //The width of screen
    int h = 32; //The height of screen

    Vector2 mouse; //This will grab our mouse's x and y coordinates

    public Texture2D cursorImage; //The cursor image will be referenced

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; //Our system cursor will not be visible
    }

    // Update is called once per frame
    void Update()
    {
        mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

        //Despite the system cursor not being visible, the position of it is still given.
        //Our Vector2 move is assign the cursors current position in game
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(mouse.x - (w / 2), mouse.y - (h / 2), w, h), cursorImage);

        //Draws our graphically drawn cursor (not the system cursor)
    }
}
