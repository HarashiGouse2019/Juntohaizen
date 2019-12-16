using System;
using UnityEngine;
using UnityEngine.Events;


public class Treasure_Chest : MonoBehaviour
{
    public Treasure content;
    
    //Chest open or closed
    public Sprite[] sprites = new Sprite[2];

    public static uint opened = 0;

    public ChestActionClass chestAction;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChestStatus();
    }

    private void Update()
    {
        spriteRenderer.sprite = sprites[opened];
    }

    public void OpenChest()
    {
        opened = 1;
        chestAction.InvokeAction();
    }

    uint ChestStatus()
    {
        return opened;
    }

}