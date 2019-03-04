using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman_Guard_Pawn : Pawn
{
    
    public static GameObject HitBox;

    

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
    }

    public override void StandIdle()
    {
        animator.Play("Idle");
        Debug.Log("Doing nothing Desu!!!");
    }

    public override void ChaseAfter()
    {
        animator.SetBool("isChasing", true);

        Debug.Log("Going after Player Desu!!!");

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        
    }

    public override void Attack()
    {

        animator.SetBool("isAtPlayer", true);
        Debug.Log("Attacking Desu!!!");
        
        

    }

    public override void ReturnToLastPosition()
    {
        
    }
}
