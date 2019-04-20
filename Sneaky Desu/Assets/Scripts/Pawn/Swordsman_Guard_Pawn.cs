using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman_Guard_Pawn : Pawn
{
    public static Swordsman_Guard_Pawn instance;

    public Loot_Chances lootChances;
    public static GameObject HitBox; //The hitbox prefab that we'll instantiate

   
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); //Our parent start method;
        instance = this;
        enemyHealth = 3f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); //OUr parent update method

        //If the enemy's health reaches to 0
        if (this.enemyHealth == 0)
        {
            Player_Controller.player_controller.toggleLock = false;
            GameObject loot = Instantiate(lootChances.gameObject);
            loot.transform.position = transform.position;
            Destroy(gameObject);
        }
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

    public void OnDestroy()
    {
        GameManager.instance.enemyInstances.Remove(this.gameObject);
    }

}
