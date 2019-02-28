using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor_Image : MonoBehaviour
{
    int w = 32;
    int h = 32;

    Vector2 mouse;

    public Texture2D cursorImage;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(mouse.x - (w / 2), mouse.y - (h / 2), w, h), cursorImage);
    }
}
