using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayerStats;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerStatusScript PlayerStatus;

    public GameObject MagicSource;

    public Transform Target;

    public List<GameObject> spawnPoint;

    public int lives;

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

    void Update()
    {
        KeyCommand();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(MagicSource);
        }
    }

    void KeyCommand()
    {
        //Simply quits the game when on standalone
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
}