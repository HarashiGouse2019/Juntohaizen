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
                case 0:

                    if (slot[1] == 2)
                    {
                        y -= 85 * 2; ++slotColNum;
                    } else
                    {
                        y -= 85;
                    }
                    break;
                case 1:
                    if (slot[1] == 3)
                    {
                        y -= 85 * 2; ++slotColNum;
                    } else
                    {
                        y -= 85;
                    }
                    break;
                case 2:
                    if (slot[1] == 2)
                    {
                        y -= 85 * 2; ++slotColNum;
                    } else
                    {
                        y -= 85;
                    }
                    break;
                case 3:
                    y -= 50;
                    break;
                default:
                    y -= 85;
                    break;
            }
            ++slotColNum;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (slot[0])
            {
                case 0:
                    if (slot[1] == 4)
                    {
                        y += 85;
                        x += 120; ++slotRowNum;
                    }
                    else
                    {
                        y += 85;
                    }
                    break;
                case 1:
                    if (slot[1] == 5)
                    {
                        y += 85 * 2; --slotColNum;
                    } else
                    {
                        y += 85;
                    }
                    break;
                case 2:
                    if (slot[1] == 4)
                    {
                        y += 85;
                        x -= 120; --slotRowNum;
                    }
                    else
                    {
                        y += 85;
                    }
                    break;
                case 3:
                    y += 50;
                    break;
                
                default:
                    y += 85;
                    break;
            }
            --slotColNum;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            x -= 120;
            //Check if the row hits 2; if it does, it'll take me to a certain location on the inventory screen
            switch (slot[0])
            {
                case 1:
                    if (slot[1] == 3)
                    {
                        y -= 85; ++slotColNum;
                    }
                    break;
                case 2:
                    if(slot[1] == 4)
                    {
                        x -= 120; --slotRowNum;
                    }
                    break;
                case 3:
                    //Change our cursor image!!!
                    Image inventoryCursor = GetComponent<Image>();

                    RectTransform rect = GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(112.51f, 82.26f);
                    inventoryCursor.sprite = pauseMenuCursorImages[0];
                    

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
                    break;
            }     
            --slotRowNum;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            x += 120;

            //Check if the row hits 3; if it does, it'll take me to a certain location on the inventory screen
            switch (slot[0])
            {
                case 0:
                    if (slot[1] == 4)
                    {
                        x += 120; ++slotRowNum;
                    }
                    break;
                case 1:
                    if (slot[1] == 3)
                    {
                        y -= 85; ++slotColNum;
                    }
                    break;
                case 2:


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

                    break;
                
            }
            ++slotRowNum;
        }
        //When it comes to heading into half of the inventory, 2,0 and 2,1 will aim me toward HP; 2,2 to HR
        //When I hit 0,2 and 2,2, I want to aim at the circle thingy majig!!!!


        DebugInventoryPosition();
    }

    void DebugInventoryPosition()
    {
        Debug.Log(slot[0] + "," + slot[1]);
    }
}
