using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Enemy_Controller : Controller
{
    bool isChasing = false; //Rather the enemy is chasing the player
    bool isAtPlayer = false; //Rather the enemy is at target reach

    public Animator animator; //Referencing our animator

    [Range(1, 10)] public int fieldOfSightValue; //How far our enemy can see or sense the player
    public static int fieldOfSight; //This is used for other scripts to reference our field of view value

    public override void Start()
    {
        base.Start(); //The start of the parent class

        pawn.animator.Play("Idle"); //Plays the idle animation
        pawn.target = GameObject.FindGameObjectWithTag("Player"); //References the player gameObject
        pawn.playerPosition = pawn.target.transform; //Grabs the player's position
        fieldOfSight = fieldOfSightValue; //Assign our non-static variable to the static one, so that other scripts can use it.
    }

    void Update()
    {
        pawn.animator.SetBool("isChasing", isChasing); //Makes a transition from idle to chase or from chase to idle depending on our boolean
        pawn.animator.SetBool("isAtPlayer", isAtPlayer);
        if (Player_Controller.player_controller.isInGround != true)
        {
            //Game Controls Updated Every Frame
            //The enemy faces the direction of the player depending on rather the x value is positive or negative from target's position
            if (pawn.playerPosition.position.x < pawn.transform.position.x)
            {
                Vector2 xscale = pawn.transform.localScale; //Grab our local scale
                xscale.x = -1; //Have it reflect over the y-axis
                pawn.transform.localScale = xscale; //Give the modified value to our scale, resulting in the enemy looking the left
            }
            else if (pawn.playerPosition.position.x > pawn.transform.position.x)
            {
                Vector2 xscale = pawn.transform.localScale; //Grab our local scale
                xscale.x = 1;  //Have it reflect over the y-axis
                pawn.transform.localScale = xscale;  //Give the modified value to our scale, resulting in the enemy looking the right
            }

            if (pawn.distance > fieldOfSight) //If enemy is out of reach
            {
                pawn.StandIdle();
                isAtPlayer = false;

            }
            if (pawn.distance < fieldOfSight && pawn.distance > 1) //If enemy sees player
            {
                pawn.ChaseAfter();
                isAtPlayer = false;
                isChasing = true;

            }
            if (pawn.distance < 1) //If enemy makes contact with the player
            {
                isAtPlayer = true;

                pawn.Attack();
            }
        } else
        {
            pawn.StandIdle();
            isAtPlayer = false;
            isChasing = false;
        }
    }
}