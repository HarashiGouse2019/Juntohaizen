using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

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
        manager = GameManager.Instance;
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        //Play OST
        MusicManager.Instance.Play("ChikaraOST", 100);
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

        //Give initial position to Game Manager
        position.x = manager.posx;
        position.y = manager.posy;

        if (Player_Pawn.playerpawn != null)
            Player_Pawn.playerpawn.player_collider.isTrigger = false;

        //Set up all Player Stats
        manager.currentHealth = manager.maxHealth;
        manager.healthUI.fillAmount = manager.currentHealth / manager.maxHealth;
        manager.currentMana = manager.maxMana;
        manager.manaUI.fillAmount = manager.currentMana / manager.maxMana;

        //Resetting All Values and activate player and game ui
        manager.ResetAllValues();
        player_spawn.gameObject.SetActive(true);
        manager.GUI_ACTIVE = true;

        manager.playerPrefab.SetActive(true);
        manager.playerPrefab.transform.position = position;

        //Stop music
        MusicManager.Instance.Stop("ChikaraOST");

        //Transition to the first room
        manager.Goto_Scene("S_FLOOR_1");

        //Run Dialogue
        Dialogue dialogue = GameManager.Instance.GetComponent<Dialogue>();
        dialogue.Run(1, 0.05f);
    }

    public void LoadGame()
    {
        player_spawn.gameObject.SetActive(true);
        manager.GUI_ACTIVE = true;

        manager.playerPrefab.SetActive(true);
        MusicManager.Instance.Stop("ChikaraOST");
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

            //Load all Player Prefs from start
            bool volumeAdjust = (
                GameManager.Instance.masterVolumeAdjust != null &&
                MusicManager.Instance.musicVolumeAdjust != null &&
                AudioManager.Instance.soundVolumeAdjust != null);

            if (volumeAdjust)
            {
                GameManager.Instance.masterVolumeAdjust.value = LoadSettings("Master Volume");
                MusicManager.Instance.musicVolumeAdjust.value = LoadSettings("Music Volume");
                AudioManager.Instance.soundVolumeAdjust.value = LoadSettings("Sound Volume");
            }

            fileSearchStart = true;
        }
        else
        {
            loadButton.interactable = false;
            fileSearchStart = true;
            return;
        }
    }

    public void SaveSettings(string _key, float _value)
    {
        //Setting all values from settings
        PlayerPrefs.SetFloat(_key, _value);
    }

    public float LoadSettings(string _key)
    {
        //Loading all values to settings
        if (PlayerPrefs.HasKey(_key))
            return PlayerPrefs.GetFloat(_key);
        Debug.Log("This might be your problem.");
        return 0;
    }

    public void ApplySettings()
    {
        SaveSettings("Master Volume", GameManager.Instance.masterVolumeAdjust.value);
        SaveSettings("Music Volume", MusicManager.Instance.musicVolumeAdjust.value);
        SaveSettings("Sound Volume", AudioManager.Instance.soundVolumeAdjust.value);
    }
}
