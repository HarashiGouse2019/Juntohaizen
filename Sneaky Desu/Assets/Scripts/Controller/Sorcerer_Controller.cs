using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer_Controller : Controller
{
    public Sorcerer_Pawn sorcerer;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        sorcerer = GetComponent<Sorcerer_Pawn>();
    }

    public void Update()
    {
        pawn.animator.SetBool("isMoving", sorcerer.isMoving);

        pawn.rb.velocity = sorcerer.direction; sorcerer.isMoving = true;

        pawn.MoveAbout();

        //Flipping over the X-Axis if necessary
        Vector3 xscale = transform.localScale;
        xscale.x = Mathf.Sign(sorcerer.rb.velocity.x);
        transform.localScale = xscale;
    }
}
