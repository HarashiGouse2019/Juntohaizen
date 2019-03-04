using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Enemy_Controller : Controller
{
    bool isChasing = false;
    bool isAtPlayer = false;

    public Animator animator;

    [Range(1, 10)] public int fieldOfSightValue;
    public static int fieldOfSight;

    

    public override void Start()
    {
        base.Start();

        pawn.animator.Play("Idle");
        pawn.target = GameObject.FindGameObjectWithTag("Player");
        pawn.playerPosition = pawn.target.transform;
        fieldOfSight = fieldOfSightValue;
    }

    void FixedUpdate()
    {
        pawn.animator.SetBool("isChasing", isChasing);
        pawn.animator.SetBool("isAtPlayer", isAtPlayer);
        //Game Controls Updated Every Frame

        

        if (pawn.playerPosition.position.x < pawn.transform.position.x)
        {
            Vector2 xscale = pawn.transform.localScale;
            xscale.x = -1;
            pawn.transform.localScale = xscale;
        }
        else if (pawn.playerPosition.position.x > pawn.transform.position.x)
        {
            Vector2 xscale = pawn.transform.localScale;
            xscale.x = 1;
            pawn.transform.localScale = xscale;
        }

        if (pawn.distance > fieldOfSight)
        {
            pawn.StandIdle();
            isAtPlayer = false;
        }
        if (pawn.distance < fieldOfSight && pawn.distance > 1)
        {
            pawn.ChaseAfter();
            isAtPlayer = false;

        }
        if (pawn.distance < 1)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Chase")) animator.Play("Attack");
            pawn.Attack();
        }
    }
}