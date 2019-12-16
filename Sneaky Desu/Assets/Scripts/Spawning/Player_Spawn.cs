using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Spawn : MonoBehaviour
{
    public static Player_Spawn Instance;

    public Vector3 coordinates;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        GameManager.Instance.playerPrefab.transform.position = new Vector3(GameManager.Instance.posx, GameManager.Instance.posy, 0);
    }
}
