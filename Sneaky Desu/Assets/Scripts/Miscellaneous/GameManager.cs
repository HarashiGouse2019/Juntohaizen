using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public static GameManager instance; //For singleton implementation

    public List<GameObject> enemyInstances;

    public GameObject playerPrefab;

    Player_Pawn player;

    public int totalGems = 0;

    public bool typeIn = false;

    public bool paused = false; //Check if the game has paused!!!

    bool magicSourceMade = false; //Rather or not we have an instance of our defined prefab

    public Transform Target; //The target that the prefab will be using; It's the origin of our player

    public float maxHealth = 100, maxMana = 100; //We are given these default values for our health and our energy

    [HideInInspector] public float currentHealth, currentMana; //The current health and the current mana that the player has

    [HideInInspector] public float levelProgression = 0, level = 1; //How far we are to our next level, and our current level

    [Header("Player Status UI Reference")]
    public RawImage GUIParent; // This is just the background interface
    public Image healthUI; //Reference our Health 
    public Image manaUI; //Reference our Mana
    public Image levelProgressionUI; //Reference how close we are to our level
    public RawImage gemCountUI;
    public TextMeshProUGUI gemAmount;
    public TextMeshProUGUI levelUI; //Reference our Level UI text
    public SpriteRenderer PlayerImage; //The image that the player takes on.
    public bool GUI_ACTIVE;

    [Header("Pause Menu UI")]
    public Canvas pauseMenuUI; //Get a reference of the Pause Menu UI

    [Header("Spawn Destination")]
    public string Scene_Name;
    [HideInInspector] public Scene currentScene;

    [Header("Set Spawn Coordinates")]
    public float posx;
    public float posy;

    [Header("Object Pooler")]
    public ObjectPooler ObjectPooler;

    [Header("Text Box")]
    public RawImage textBoxUI;
    public TextMeshProUGUI dialogue;



    void Awake()
    {
        ////If our Game Manager does not exists, create one, and do not destroy it.
        ////If there's an extra one, we'll destroy the extra one.
        ////This is our singleton design pattern in use.

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Instantiate(playerPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

    void Start()
    {
        currentHealth = maxHealth; //Current health is set to a defined max health
        currentMana = maxMana; //Current mana is set to a defined max mana

        //The fill amount will only take values between 0 and 1. By dividing the current and the max value 
        //of our Health, Mana, and Level Progression, we'll always stay between 0 and 1
        healthUI.fillAmount = currentHealth / maxHealth;
        manaUI.fillAmount = currentMana / maxMana;
        levelProgressionUI.fillAmount = levelProgression;

        playerPrefab = FindObjectOfType<Player_Pawn>().gameObject;
        player = FindObjectOfType<Player_Pawn>();

        playerPrefab.SetActive(false);
    }

    void Update()
    {
        //Get the current active scene
        currentScene = SceneManager.GetActiveScene();

        //Set our GUI value to our GUI_ACTIVE boolean
        GUIParent.gameObject.SetActive(GUI_ACTIVE);

        InitiateKeyInput(); //All input from the keyboard will be called in this method

        //If we hit level one, we want to create our Magic prefab so that we can shoot some enemies.
        if (level > 0 && magicSourceMade == false)
        {

            player.MagicSource.SetActive(true);
            magicSourceMade = true;
        }

        if (player.MagicSource.activeInHierarchy == false) magicSourceMade = false; //If the player dies, and is ability is still in scene, we disable it.

        ScanAllEnemies(); //Iterate through all enemy instances in the current scene. This will then create a list.

        //Player Pausing the game
        //If the pause button has been pressed
        if (Input.GetKeyDown(Player_Controller.player_controller.pause))
        {
            if (Player_Controller.player_controller.p == 0) //If the game doesn't know that the pause button is being pressed
            {
                switch (paused)
                {
                    case false:
                        PauseGame(true); //Pause the game
                        Time.timeScale = 0f; //It will stop time like DIO!!!!
                        break;
                    case true:
                        PauseGame(false); //Unpause the game
                        Time.timeScale = 1f;
                        break;
                }

                Player_Controller.player_controller.p = 1; //Pause Button is pressed down
            }
        }
        else
        {
            Player_Controller.player_controller.p = 0; //The pause button is released
        }
    }

    void InitiateKeyInput()
    {
        //Simply quits the game when on standalone
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        levelUI.text = level.ToString();
        gemAmount.text = totalGems.ToString("D4");//Text only takes a string, however level is an integer.
        //By having ".ToString()", we are able to convert the integer to a string.
        //Meaning the numbers can dynamically be changed based on the value of our level variable.
    }

    public float IncreaseLevel(float value)
    {
        levelProgressionUI.fillAmount += value / 100f; //Level will always go to 100. If it didn't, we would have "value / maxLevel"
        levelProgression = levelProgressionUI.fillAmount;// Debug.Log(levelProgressionUI.fillAmount); //Displays a log that gives use the level fill amount
        if (levelProgressionUI.fillAmount == 1f)
        {
            float pastLevel = level;
            level += 1;
            IncreaseHealth(maxHealth);
            levelProgressionUI.fillAmount = 0f;
            AudioManager.audioManager.Play("LevelUp");
        }
        return value;

        //If our level progression bar is maxed out, we have reached the next level!
    }

    public float DecreaseLevel(float value)
    {
        levelProgressionUI.fillAmount -= value / 100f;//Level will always go to 100. If it didn't, we would have "value / maxLevel"
        levelProgression = levelProgressionUI.fillAmount;                                     // Debug.Log(levelProgressionUI.fillAmount); //Displays a log that gives use the level fill amount
        Debug.Log(levelProgressionUI.fillAmount);

        if (levelProgressionUI.fillAmount < 1f / maxHealth && level != 0f)
        {
            level -= 1;
            levelProgressionUI.fillAmount = maxHealth - 1f;
        }

        return value;

        //If our level progression bar is maxed out, we have reached the next level!
    }

    //Increase our health based on a given value
    public float IncreaseHealth(float value)
    {
        //If the fillAmount is not maxed out, we'll continue to increase our health
        if (healthUI.fillAmount != maxHealth) healthUI.fillAmount += value / maxHealth;
        currentHealth = healthUI.fillAmount;

        return value;
    }

    public float DecreaseHealth(float value)
    {
        //If the fillAmount is not 0, continue to decrease our health
        if (healthUI.fillAmount != 0) healthUI.fillAmount -= value / maxHealth;
        currentHealth = healthUI.fillAmount;
        if (currentHealth == 0)
        {
            GUI_ACTIVE = false;
            Die();
        }
        return value;
    }

    public float IncreaseMana(float value)
    {
        //If mana isn't maxed out, continue to increase mana
        if (manaUI.fillAmount != maxMana) manaUI.fillAmount += value / maxMana;
        currentMana = manaUI.fillAmount;
        return value;
    }
    public float DecreaseMana(float value)
    {
        //If mana is not at 0, continue decreasing mana
        if (manaUI.fillAmount != 0) manaUI.fillAmount -= value / maxMana;
        currentMana = manaUI.fillAmount;
        return value;
    }

    //Game State (Moving to losing or winning screen)

    public void Die()
    {
        //This will bring out our "Game Over" scene
        SceneManager.LoadScene(3);
    }

    public void Win()
    {
        //This will take us to our "Victory" scene
        SceneManager.LoadScene(2);
    }

    //Scene Managing
    public void Goto_Scene(string scene_name)
    {
        scene_name = Scene_Name;
        if (scene_name != null) SceneManager.LoadScene(scene_name);
    }

    //Scan all possible enemies in the current scene. This will be needed for our lock-on system.
    public int ScanAllEnemies()
    {
        //We create an array of all instances of GameObjects with the tag "Enemy"
        //We iterate through the array's length, and add each one into our list.
        //This continues until our list is equal to our GameObject array.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyInstances.Count < enemies.Length)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemyInstances.Add(enemies[i]);
            }
        }
        return enemyInstances.Count; //We then return the value of all instances through our List<GameObject>
    }

    //Pauses the game
    public bool PauseGame(bool active)
    {
        pauseMenuUI.gameObject.SetActive(active);
        paused = active;
        return active;
    }

    public IEnumerator DisplayText(string text, float textspeed)
    {
        if (typeIn == false)
        {
            textBoxUI.gameObject.SetActive(true);
            if (dialogue.text.Length > 0)
            {
                dialogue.text = "";
            }

            //This give a typewritter effect. With a ton of trial and error, this one works the best!!!
            for (int i = 0; i < text.Length; i++)
            {
                StartCoroutine(DisableDelay());
                dialogue.text = text.Substring(0, i);
                AudioManager.audioManager.Play("Type000");
                yield return new WaitForSeconds(textspeed);

            }
            typeIn = true;
        }
    }

    public IEnumerator DisableDelay()
    {
        yield return new WaitForSeconds(5f);
        textBoxUI.gameObject.SetActive(false);
        typeIn = false;
        StopCoroutine(DisableDelay());
    }

    public void ResetAllValues()
    {
        enemyInstances = new List<GameObject>();
        healthUI.fillAmount = maxHealth; //Current health is set to a defined max health
        manaUI.fillAmount = maxMana; //Current mana is set to a defined max mana
        level = 1;
        levelProgressionUI.fillAmount = 0f;
        magicSourceMade = false;
    }
}