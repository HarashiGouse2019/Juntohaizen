using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using static MasterSounds.AudioManager;

public class MainMenu : MonoBehaviour
{
    public GameManager manager;

    public Player_Pawn player = null;
    public Player_Spawn player_spawn;

    public void Start()
    {
        manager = GameManager.instance;
        audioManager.Play("JuntohaizenOST");
    }

    public void Play()
    {
        audioManager.Stop("JuntohaizenOST");

        //Resetting All Values
        manager.ResetAllValues();

        //Set starting position!!!
        manager.Scene_Name = "Level1-1";
        manager.posx = 0f;
        manager.posy = -2.5f;

        player_spawn.gameObject.SetActive(true);
        manager.GUI_ACTIVE = true;
        manager.Goto_Scene("Level1-1");
        manager.currentHealth = manager.maxHealth;
        manager.healthUI.fillAmount = manager.currentHealth / manager.maxHealth;
        Instantiate(manager.playerPrefab);
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
