using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Public Members
    public GameManager manager;
    public Player_Pawn player;
    public Player_Spawn player_spawn;
    public Button loadButton;

    #endregion

    #region Private Members
    PlayerData data;

    bool fileSearchStart = false;

    Scene sceneBeforeSave;
    Vector3 position;

    #endregion

    void Awake()
    {
        manager = GameManager.instance;
    }

    void Start()
    {
        MusicManager.manager.Play("ChikaraOST", 100);
    }

    void Update()
    {
        if (data == null)
        {
            data = SaveLoadSystem.LoadPlayer();
            ReadSaveFile();
        }
    }

    public void Play()
    {
        //Set starting position!!!
        manager.Scene_Name = "S_FLOOR_1";
        manager.posx = 0f;
        manager.posy = -2.5f;

        position.x = manager.posx;
        position.y = manager.posy;

        if (Player_Pawn.playerpawn != null)
            Player_Pawn.playerpawn.player_collider.isTrigger = false;

        manager.currentHealth = manager.maxHealth;
        manager.healthUI.fillAmount = manager.currentHealth / manager.maxHealth;
        manager.currentMana = manager.maxMana;
        manager.manaUI.fillAmount = manager.currentMana / manager.maxMana;

        //Resetting All Values
        manager.ResetAllValues();
        player_spawn.gameObject.SetActive(true);
        manager.GUI_ACTIVE = true;

        manager.playerPrefab.SetActive(true);
        manager.playerPrefab.transform.position = position;

        MusicManager.manager.Stop("ChikaraOST");

        manager.Goto_Scene("S_FLOOR_1");

        Dialogue dialogue = GameManager.instance.GetComponent<Dialogue>();
        dialogue.Run(1, 0.05f);
    }

    public void Load()
    {
        player_spawn.gameObject.SetActive(true);
        manager.GUI_ACTIVE = true;

        manager.playerPrefab.SetActive(true);
        MusicManager.manager.Stop("ChikaraOST");
        manager.Goto_Scene(sceneBeforeSave.ToString());
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0); //This is the index build of the Main Menu
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();

        //In Unity, nothing will happen except having a debug log saying "Quit". 
        //However once you build the game and this method is called, the application will close;
    }

    public void ReadSaveFile()
    {
        string path = Application.persistentDataPath + "/hai.zen";
        if (File.Exists(path) && fileSearchStart == false)
        {

            loadButton.interactable = true;

            sceneBeforeSave = manager.currentScene;

            //Gather the players last know position
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];

            //Update the players position through the game manager
            manager.playerPrefab.transform.position = position;

            //Doing a little extra
            manager.posx = position.x;
            manager.posy = position.y;
            manager.Scene_Name = data.location;

            //Load player health data
            manager.currentHealth = data.health;
            manager.maxHealth = data.maxHealth;
            manager.healthUI.fillAmount = data.health;

            //Load mana data
            manager.currentMana = data.mana;
            manager.maxMana = data.maxMana;
            manager.manaUI.fillAmount = data.mana;

            //Get the current level
            manager.level = data.level;

            //And next level progression
            manager.levelProgression = data.levelProgression;
            manager.levelProgressionUI.fillAmount = data.levelProgression;
            fileSearchStart = true;
        }
        else
        {
            loadButton.interactable = false;
            fileSearchStart = true;
            return;
        }
    }
}
