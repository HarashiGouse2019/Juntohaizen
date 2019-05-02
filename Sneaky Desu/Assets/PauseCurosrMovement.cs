using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCurosrMovement : MonoBehaviour
{
    //[Header("Pause Menu Curosr")]
    public List<Sprite> pauseMenuCursorImages;

    //CursorPositioning
    public static float[] position;
    public static float[] slot;
    float x = -295.75f, y = 268.26f;
    float slotRowNum, slotColNum;
    // Update is called once per frame
    void Update()
    {
        position = new float[2]; //Creat e new arry for positioning
        slot = new float[2]; //Creat e new arry for slot position

        position[0] = x; //GameObject Positioning
        position[1] = y;

        slot[0] = slotRowNum; //Inventory Positioning
        slot[1] = slotColNum;

        transform.localPosition = new Vector2(position[0], position[1]); //The actual scene positioning relative to the parent position ;)

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch (slot[0])
            {
                case 3:
                    y -= 50;
                    break;
                default:
                    y -= 85;
                    break;
            }
            ++slotColNum;
        } else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (slot[0])
            {
                case 3:
                    y += 50;
                    break;
                default:
                    y += 85;
                    break;
            }
            --slotColNum;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            x -= 120;
            //Check if the row hits 2; if it does, it'll take me to a certain location on the inventory screen
            if (slot[0] == 3)
            {
                Image inventoryCursor = GetComponent<Image>();

                RectTransform rect = GetComponent<RectTransform>();

                inventoryCursor.sprite = pauseMenuCursorImages[0];
                rect.sizeDelta = new Vector2(112.51f, 82.26f);

                switch (slot[1])
                {
                    case 0:
                        x -= 120; y -= 9f;
                        break;
                    case 1:
                        x -= 120; y -= 44f;
                        break;
                    case 2:
                        --slotColNum;
                        x -= 120; y += 6f;
                        break;
                }
                

            }
            --slotRowNum;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            x += 120;

            //Check if the row hits 3; if it does, it'll take me to a certain location on the inventory screen
            if (slot[0] == 2 || (slot[1] == 0 && slot[1] == 1))
            {
                Image inventoryCursor = GetComponent<Image>();

                RectTransform rect = GetComponent<RectTransform>();

                inventoryCursor.sprite = pauseMenuCursorImages[1];
                rect.sizeDelta = new Vector2(350f, 66.5f);

                switch (slot[1])
                {
                    case 0:
                        x += 120; y += 9f;
                        break;
                    case 1:
                        x += 120; y += 44f;
                        break;
                }

            }
            ++slotRowNum;
        }
        //When it comes to heading into half of the inventory, 2,0 and 2,1 will aim me toward HP; 2,2 to HR
       


        DebugInventoryPosition();
    }

    void DebugInventoryPosition()
    {
        Debug.Log(slot[0] + "," + slot[1]);
    }
}
