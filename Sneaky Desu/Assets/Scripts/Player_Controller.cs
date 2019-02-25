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

        pawn.coroutine = Walk();

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
                pawn.coroutine = RecoveryWhileIdle(1f);
                if (pawn.isWaiting == false) StartCoroutine(pawn.coroutine); else StopCoroutine(pawn.coroutine);
                pawn.isWalking = false;
            }
        }

        if (pawn.isWalking == true)
        {
            StartCoroutine(pawn.coroutine);
        }

    }

   

    IEnumerator Walk()
    {
       
        if (pawn.step == false)
        {
            FindObjectOfType<AudioManager>().Play("Walk");
            pawn.Status.DecreaseHealth(1f);
            pawn.step = true;
            yield return new WaitForSeconds((float)0.15);
            pawn.step = false;
        }
    }

    IEnumerator RecoveryWhileIdle(float value)
    {
        if (pawn.Status.healthUI.fillAmount != pawn.Status.maxHealth)
        {
            pawn.Status.IncreaseHealth(1f);
            pawn.isWaiting = true;
        }
        yield return new WaitForSeconds(value);
        pawn.isWaiting = false;
    }
}
