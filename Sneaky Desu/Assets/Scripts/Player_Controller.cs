using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStats;

public class Player_Controller : Controller
{
    public override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        pawn.animator.SetBool("isWalking", pawn.isWalking);
        //Game Controls Updated Every Frame

        pawn.coroutine = pawn.Walk();

        if (pawn.col == false)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                pawn.MoveFoward();

            else if (Input.GetKey(KeyCode.DownArrow))
                pawn.MoveBackwards();

            else if (Input.GetKey(KeyCode.LeftArrow))
                pawn.MoveLeft();

            else if (Input.GetKey(KeyCode.RightArrow))
                pawn.MoveRight();

            else
            {
                pawn.coroutine = pawn.RecoveryWhileIdle(1f);
                if (pawn.isWaiting == false) StartCoroutine(pawn.coroutine); else StopCoroutine(pawn.coroutine);
                pawn.isWalking = false;
            }
        }

        if (pawn.isWalking == true)
        {
            StartCoroutine(pawn.coroutine);
        }

    }
}
