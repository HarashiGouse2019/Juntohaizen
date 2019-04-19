using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using static MasterSounds.AudioManager;

public class MainMenu : MonoBehaviour
{
    public GameManager manager;

    public Player_Pawn player;
    public Player_Spawn player_spawn;

    Scene sceneBeforeSave;

    Vector3 position;
    PlayerData data;

    public void Start()
    {
        manager = GameManager.instance;
        audioManager.Play("JuntohaizenOST");
    }

    public void Update()
    {
        string path = Application.persistentDataPath + "/player.sav";
        try
        {
            if (File.Exists(path))
            {

                sceneBeforeSave = manager.currentScene;

                //Gather the players last know position
                position.x = data.position[0];
                position.y = data.position[1];
                position.z = data.position[2];

                //Update the players position through the game manager
                manager.playerPrefab.transform.position = position;

                //Load player health data
                manager.currentHealth = data.health;
                //manager.maxHealth = data.maxHealth;
                //manager.healthUI.fillAmount = data.health / data.maxHealth;

                //Load mana data
                manager.currentMana = data.mana;
                //manager.maxMana = data.maxMana;
                //manager.manaUI.fillAmount = data.mana / data.maxMana;

                //Get the current level
                manager.level = data.level;

                //And next level progression
                manager.levelProgression = data.levelProgression;
                //manager.levelProgressionUI.fillAmount = data.levelProgression / 100f; 

            }
        } catch
        {
            return;
        }
    }

    public void Play()
    {
        //Set starting position!!!
        manager.Scene_Name = "Level1-1";
        manager.posx = 0f;
        manager.posy = -2.5f;

        position.x = manager.posx;
        position.y = manager.posy;

        

        manager.currentHealth = manager.maxHealth;
        manager.healthUI.fillAmount = manager.currentHealth / manager.maxHealth;

        audioManager.Stop("JuntohaizenOST");

        //Resetting All Values
        manager.ResetAllValues();
        player_spawn.gameObject.SetActive(true);
        manager.GUI_ACTIVE = true;

  
 
       manager.playerPrefab.SetActive(true);
        manager.playerPrefab.transform.position = position;

        manager.Goto_Scene("Level1-1");
    }

    public void Load()
    {
        
        player_spawn.gameObject.SetActive(true);
        manager.GUI_ACTIVE = true;

        data = SaveLoadSystem.LoadPlayer();

        manager.playerPrefab.SetActive(true);

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
}
