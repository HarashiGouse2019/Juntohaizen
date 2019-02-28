using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : Controller
{
    const bool isChasing = false, isAtPlayer = false;

    [HideInInspector] public Transform playerPosition;

    [HideInInspector] public GameObject target;

    [Range(1, 10)] public int fieldOfSightValue;
    public static int fieldOfSight;

    public override void Start()
    {
        base.Start();

        pawn.animator.Play("Idle");
        target = GameObject.FindGameObjectWithTag("Player");
        playerPosition = target.transform;
        fieldOfSight = fieldOfSightValue;
    }

    void FixedUpdate()
    {
        pawn.animator.SetBool("isChasing", isChasing);
        pawn.animator.SetBool("isAtPlayer", isAtPlayer);
        //Game Controls Updated Every Frame

        //All Possible States
        Vector2 heading = pawn.animator.transform.position - playerPosition.position;

        float distance = heading.magnitude;

        if (playerPosition.position.x < pawn.transform.position.x)
        {
            Vector2 xscale = pawn.transform.localScale;
            xscale.x = -1;
            pawn.transform.localScale = xscale;
        }
        else if (playerPosition.position.x > pawn.transform.position.x)
        {
            Vector2 xscale = pawn.transform.localScale;
            xscale.x = 1;
            pawn.transform.localScale = xscale;
        }

        if (distance > fieldOfSight * 2)
        {
            pawn.StandIdle();
        }

        if (distance < fieldOfSight || (distance > 1 && isAtPlayer == true))
        {
            pawn.ChaseAfter();

        }
        if (distance < fieldOfSight / 2)
        {
            pawn.Attack();
        }
    }
}