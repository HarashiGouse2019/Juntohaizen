using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman_Guard_Pawn : Pawn
{
    Transform target;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void StandIdle()
    {
        animator.Play("Idle");
        animator.SetBool("isChasing", false);
    }

    public override void ChaseAfter()
    {
        animator.Play("Chase");
        animator.SetBool("isChasing", true);
    }

    public override void Attack()
    {
        animator.SetBool("isAtPlayer", true);
        animator.Play("Attack");
    }

    public override void ReturnToLastPosition()
    {
        
    }
}
