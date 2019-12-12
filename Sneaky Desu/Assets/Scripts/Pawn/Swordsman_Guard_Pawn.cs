using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman_Guard_Pawn : Pawn
{
    public static Swordsman_Guard_Pawn instance;

    public Loot_Chances lootChances;

    private float manaReserve;

    public GameObject hitbox; public float offset;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); //Our parent start method;
        instance = this;
        enemyHealth = 3f;
        manaReserve = enemyHealth * 2f;

        if (hitbox != null)
        {
            hitbox = Instantiate(hitbox, gameObject.transform);
            hitbox.SetActive(false);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); //OUr parent update method

        //If the enemy's health reaches to 0
        if (this.enemyHealth < 1)
        {
            Player_Controller.player_controller.toggleLock = false;
            GameObject loot = Instantiate(lootChances.gameObject);
            loot.transform.position = transform.position;
            GameManager.instance.IncreaseMana(manaReserve);
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

    public override void HitBoxEnablement(int _enabled)
    {
        if (hitbox != null)
        {
            hitbox.SetActive(_enabled == 1 ? true : false);
            if (_enabled == 1)
                hitbox.transform.position = target.transform.position;
        }
    }

    //The animations are controlled by the animator, so by setting a boolean, we can get a certain animation
    //to play.

    public void OnDestroy()
    {
        GameManager.instance.enemyInstances.Remove(this.gameObject);
    }

}
