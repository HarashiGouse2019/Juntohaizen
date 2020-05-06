using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEntry : MonoBehaviour
{
    public static DoorEntry Instance;

    public float value_x;
    public float value_y;
    public string scene_name;
    public bool allowSpawn = true;

    public Player_Pawn player;

    private void Awake()
    {
        player = FindObjectOfType<Player_Pawn>();
        Instance = this;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.posx = value_x;
            GameManager.Instance.posy = value_y;
            Player_Spawn.Instance.coordinates = new Vector3(GameManager.Instance.posx, GameManager.Instance.posy, 0);
            collision.gameObject.transform.position = Player_Spawn.Instance.coordinates;

            if (scene_name != null)
            {
                GameManager.Instance.Scene_Name = scene_name;
                GameManager.Instance.Goto_Scene(scene_name);
            }
            else
                Debug.LogWarning("scene_name is currently null. Scene transition will be ignored.");
        }
    }
}
