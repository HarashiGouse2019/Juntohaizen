using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float health, mana, level, levelProgression, maxHealth, maxMana;
    public float[] position;

    public PlayerData (Player_Pawn player)
    {
        health = GameManager.instance.currentHealth;
        maxHealth = GameManager.instance.maxHealth;
        mana = GameManager.instance.currentMana;
        maxMana = GameManager.instance.maxMana;
        level = GameManager.instance.level;
        levelProgression = GameManager.instance.levelProgression;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
