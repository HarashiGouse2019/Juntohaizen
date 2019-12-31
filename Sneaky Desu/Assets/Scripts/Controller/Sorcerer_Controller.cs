using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer_Controller : Controller
{
    public Sorcerer_Pawn sorcerer;
    Player_Pawn player;

    public float aggroDistance = 5f; //The distance in which the socerer 
    public float stoppingDistance = 3f;
    public float retreatDistance = 2f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        sorcerer = GetComponent<Sorcerer_Pawn>();
        player = FindObjectOfType<Player_Pawn>();
    }

    public void Update()
    {
        pawn.animator.SetBool("isMoving", sorcerer.isMoving);

        pawn.rb.velocity = sorcerer.direction; sorcerer.isMoving = true;

        sorcerer.MoveAbout();

        //Flipping over the X-Axis if necessary
        Vector3 xscale = transform.localScale;
        xscale.x = Mathf.Sign(sorcerer.rb.velocity.x);
        transform.localScale = xscale;

        bool playerIsVisible = !player.GetComponent<Player_Controller>().isInGround;
        bool atAggroDistance = Vector2.Distance(transform.position, player.transform.position) < aggroDistance;

        //Calculating the distance between this object and the player
        if (atAggroDistance && playerIsVisible)
            sorcerer.aggroState = true;
        else
            sorcerer.aggroState = false;
    }
}
