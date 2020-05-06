using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "New Treasure", menuName = "Treasure")]
public class Treasure : ScriptableObject
{
    public uint treasureID = 10000;

    public Sprite treasureSprite;

    [TextArea] public string description;

    Treasure()
    {
        TreasureSystem.treasures.Add(this);
        treasureID += (uint)TreasureSystem.treasures.Count - 1;
    }
}
