using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayOnEnemy : MonoBehaviour
{
    static StayOnEnemy aim;
    GameObject target;
    private void Awake()
    {
        if (aim == null )
        {
            aim = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Exception Handing try...catch statement
        //Try to find a player pawn. If it can't
        //it will destroy itself;
        try {
            target = FindObjectOfType<Player_Pawn>().closestObject;
        }
        catch
        {
            Destroy(gameObject);
        }
            
        if (target != null)
        {
            gameObject.transform.position = target.transform.position;
            if (Player_Controller.player_controller.toggleLock == false)
            {
                Destroy(gameObject);
            }
        } else if (target == null || Player_Pawn.playerpawn.targetNear == false)
        {
            Destroy(gameObject);
        }
    }
}
