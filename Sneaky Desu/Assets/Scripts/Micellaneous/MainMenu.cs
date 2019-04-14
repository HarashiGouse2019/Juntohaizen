using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public Player_Pawn player = null;
    public Player_Spawn player_spawn;

    public void Play()
    {
        //Set starting position!!!
        GameManager.instance.Scene_Name = "Level1-1";
        GameManager.instance.posx = 0f;
        GameManager.instance.posy = -2.5f;

        player_spawn.gameObject.SetActive(true);
        GameManager.instance.GUI_ACTIVE = true;
        GameManager.instance.Goto_Scene("Level1-1");
        GameManager.instance.currentHealth = GameManager.instance.maxHealth;
        GameManager.instance.healthUI.fillAmount = GameManager.instance.currentHealth / GameManager.instance.maxHealth;
        Instantiate(GameManager.instance.playerPrefab);
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
