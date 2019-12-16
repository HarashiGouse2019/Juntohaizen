[System.Serializable]
public class PlayerData
{
    public float health, mana, level, levelProgression, maxHealth, maxMana; //All values
    public float healthFill, manaFill, levelProFill; //All GUI bars
    public string location; //Floor Level
    public float[] position = new float[3];

    public PlayerData (Player_Pawn player)
    {
        health = GameManager.Instance.currentHealth;
        maxHealth = GameManager.Instance.maxHealth;
        healthFill = health / maxHealth;

        mana = GameManager.Instance.currentMana;
        maxMana = GameManager.Instance.maxMana;
        manaFill = mana / maxMana;

        level = GameManager.Instance.level;
        levelProgression = GameManager.Instance.levelProgression;
        levelProFill = levelProgression / 100f;

        location = GameManager.Instance.Scene_Name;

        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
