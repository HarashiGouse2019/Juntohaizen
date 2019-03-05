using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : Controller
{
    public override void Start()
    {
        base.Start(); //Calls our parent start function
    }

    void FixedUpdate()
    {
        pawn.animator.SetBool("isWalking", pawn.isWalking);
        //Game Controls Updated Every Frame

        pawn.coroutine = pawn.Walk(); //Coroutine to make our walking sound

        if (pawn.col == false)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                pawn.MoveFoward(); //Player will move up

            else if (Input.GetKey(KeyCode.DownArrow))
                pawn.MoveBackwards(); //Player will move down

            else if (Input.GetKey(KeyCode.LeftArrow))
                pawn.MoveLeft(); //Player will move left

            else if (Input.GetKey(KeyCode.RightArrow))
                pawn.MoveRight(); //Player will move right

            else
            {
                pawn.coroutine = pawn.RecoveryWhileIdle(1f); //Increases our health when we are not moving; Our player is resting
                if (pawn.isWaiting == false) StartCoroutine(pawn.coroutine); else StopCoroutine(pawn.coroutine);
                pawn.isWalking = false;
            }
        }

        if (pawn.isWalking == true)
        {
            StartCoroutine(pawn.coroutine); //Start our coroutine walking steps
        }
    }
}
