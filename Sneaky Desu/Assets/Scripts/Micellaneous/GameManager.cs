using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemyInstances;

    public GameObject playerPrefab;

    public int gemInstances = 0; //How many gems are in game

    public static GameManager instance; //For singleton implementation

    public GameObject MagicSource; //This will be a prefab that we instantiate when our level value is greater than 0

    bool magicSourceMade = false; //Rather or not we have an instance of our defined prefab

    public Transform Target; //The target that the prefab will be using; It's the origin of our player

    public float maxHealth = 100, maxMana = 100; //We are given these default values for our health and our energy

    [HideInInspector] public float currentHealth, currentMana; //The current health and the current mana that the player has

    [HideInInspector] public float levelProgression = 0, level = 0; //How far we are to our next level, and our current level

    [Header("Player Status UI Reference")]
    public RawImage GUIParent; // This is just the background interface
    public Image healthUI; //Reference our Health 
    public Image manaUI; //Reference our Mana
    public Image levelProgressionUI; //Reference how close we are to our level
    public TextMeshProUGUI levelUI; //Reference our Level UI text
    public SpriteRenderer PlayerImage; //The image that the player takes on.
    public bool GUI_ACTIVE;

    [Header("Spawn Destination")]
    public string Scene_Name;

    [Header("Set Spawn Coordinates")]
    public float posx, posy;

    void Awake()
    {
        //If our Game Manager does not exists, create one, and do not destroy it.
        //If there's an extra one, we'll destroy the extra one.
        //This is our singleton design pattern in use.

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        enemyInstances = new List<GameObject>();
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

        gemInstances = GameObject.FindGameObjectsWithTag("Gem").Length; //Give use the value of existing gem game objects
    }

    void Update()
    {
        //Set our GUI value to our GUI_ACTIVE boolean
        GUIParent.gameObject.SetActive(GUI_ACTIVE);

        InitiateKeyInput(); //All input from the keyboard will be called in this method
        
        //If we hit level one, we want to create our Magic prefab so that we can shoot some enemies.
        
        if (level == 1 && magicSourceMade == false)
        {
                Instantiate(MagicSource);
            magicSourceMade = true;
        }
    }

    void InitiateKeyInput()
    {
        //Simply quits the game when on standalone
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
        
            
        
        //These inputs are here in order to test the UI 
        if (Input.GetKey(KeyCode.Return)) IncreaseLevel(1f);
        if (Input.GetKey(KeyCode.Backspace)) DecreaseLevel(1f);
        if (Input.GetKey(KeyCode.Equals)) IncreaseMana(1f);
        if (Input.GetKey(KeyCode.Minus)) DecreaseMana(1f);

        levelUI.text = level.ToString(); //Text only takes a string, however level is an integer.
        //By having ".ToString()", we are able to convert the integer to a string.
        //Meaning the numbers can dynamically be changed based on the value of our level variable.
    }

    public float IncreaseLevel(float value)
    {
        levelProgressionUI.fillAmount += value / 100f; //Level will always go to 100. If it didn't, we would have "value / maxLevel"
       // Debug.Log(levelProgressionUI.fillAmount); //Displays a log that gives use the level fill amount
        if (levelProgressionUI.fillAmount == 1f)
        {
            float pastLevel = level;
            level += 1;
            levelProgressionUI.fillAmount = 0f;
            FindObjectOfType<AudioManager>().Play("LevelUp");
            Debug.Log("You went from Level " + pastLevel + " to Level " + level + "!!!");
        }
        return value;

        //If our level progression bar is maxed out, we have reached the next level!
    }

    public float DecreaseLevel(float value)
    {
        levelProgressionUI.fillAmount -= value / 100f;//Level will always go to 100. If it didn't, we would have "value / maxLevel"
                                                      // Debug.Log(levelProgressionUI.fillAmount); //Displays a log that gives use the level fill amount
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

        SceneManager.LoadScene(3);
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
}