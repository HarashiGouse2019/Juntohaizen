using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayOnEnemy : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = FindObjectOfType<Player_Pawn>().closestObject.transform.position;
        if (Player_Controller.player_controller.toggleLock == false)
        {
            Destroy(gameObject);
        }
    }
}
