using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject MagicSource;

    public Transform Target;

    public List<GameObject> spawnPoint;

    public float maxHealth = 100, maxMana = 100;

    [HideInInspector] public float currentHealth, currentMana;

    [HideInInspector] public float levelProgression = 0, level = 0;

    [Header("Player Status UI Reference")]
    public Image healthUI;
    public Image manaUI;
    public Image levelProgressionUI;

    public TextMeshProUGUI levelUI;

    public SpriteRenderer PlayerImage;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        healthUI.fillAmount = currentHealth / maxHealth;
        manaUI.fillAmount = currentMana / maxMana;
        levelProgressionUI.fillAmount = levelProgression;
    }

    void Update()
    {
        InitiateKeyInput();
    }

    void InitiateKeyInput()
    {
        //Simply quits the game when on standalone
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(MagicSource);
        }

        if (Input.GetKey(KeyCode.Return)) IncreaseLevel(1f);
        if (Input.GetKey(KeyCode.Backspace)) DecreaseLevel(1f);
        if (Input.GetKey(KeyCode.Equals)) IncreaseMana(1f);
        if (Input.GetKey(KeyCode.Minus)) DecreaseMana(1f);

        levelUI.text = level.ToString();
    }

    public float IncreaseLevel(float value)
    {
        levelProgressionUI.fillAmount += value / 100f;
        Debug.Log(levelProgressionUI.fillAmount);
        if (levelProgressionUI.fillAmount == 1f)
        {
            float pastLevel = level;
            level += 1;
            levelProgressionUI.fillAmount = 0f;
            FindObjectOfType<AudioManager>().Play("LevelUp");
            Debug.Log("You went from Level " + pastLevel + " to Level " + level + "!!!");
        }
        return value;
    }

    public float DecreaseLevel(float value)
    {
        levelProgressionUI.fillAmount -= value / 100f;
        Debug.Log(levelProgressionUI.fillAmount);

        if (levelProgressionUI.fillAmount < 1f / maxHealth && level != 0f)
        {
            level -= 1;
            levelProgressionUI.fillAmount = maxHealth - 1f;
        }

        return value;
    }

    public float IncreaseHealth(float value)
    {
        if (healthUI.fillAmount != maxHealth) healthUI.fillAmount += value / maxHealth;
        currentHealth = healthUI.fillAmount;
        return value;
    }

    public float DecreaseHealth(float value)
    {
        if (healthUI.fillAmount != 0) healthUI.fillAmount -= value / maxHealth;
        currentHealth = healthUI.fillAmount;
        return value;
    }

    public float IncreaseMana(float value)
    {
        if (manaUI.fillAmount != maxMana) manaUI.fillAmount += value / maxMana;
        currentMana = manaUI.fillAmount;
        return value;
    }
    public float DecreaseMana(float value)
    {
        if (manaUI.fillAmount != 0) manaUI.fillAmount -= value / maxMana;
        currentMana = manaUI.fillAmount;
        return value;
    }
}