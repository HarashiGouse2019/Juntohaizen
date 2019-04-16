using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman_Guard_Pawn : Pawn
{
    
    public static GameObject HitBox; //The hitbox prefab that we'll instantiate

    public void OnDestroy()
    {
        GameManager.instance.enemyInstances.Remove(this.gameObject);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); //Our parent start method;
        //GameManager.instance.enemyInstances.Add(gameObject);
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); //OUr parent update method
        
    }

    //These are all the states that the enemy will use
    public override void StandIdle()
    {
        animator.Play("Idle");
    }

    public override void ChaseAfter()
    {
        animator.SetBool("isChasing", true);
        //A nice way for our enemy to chase after the player!!!
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    public override void Attack()
    {
        animator.SetBool("isAtPlayer", true);
    }

    //The animations are controlled by the animator, so by setting a boolean, we can get a certain animation
    //to play.
}
