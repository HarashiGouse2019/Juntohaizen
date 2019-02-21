using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject[] spawnPoint;

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
    }

    void KeyCommand()
    {
        //Simply quits the game when on standalone
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
}