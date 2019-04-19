using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer_Pawn : Pawn
{
    public static Sorcerer_Pawn instance;

    public Loot_Chances lootChances;

    public GameObject plamsaPrefab;

    public float walkSpeed;

    public int[] angles = { 0, 90, 180, 270 };
    public int dir = 0;
    public bool isMoving = true;

    public bool timeToChangeDirections = true;

    public Vector2 direction;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); //Our parent start method;
        
        instance = this;
        enemyHealth = 20f;
        walkSpeed = 1f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); //OUr parent update method

        

        //If the enemy's health reaches to 0
        if (enemyHealth == 0)
        {
            Player_Controller.player_controller.toggleLock = false;
            Instantiate(lootChances);
            lootChances.transform.position = transform.position;
            Destroy(gameObject);
        }

            
    }

    //These are all the states that the enemy will use
    public override void StandIdle()
    {
        animator.Play("Sorcerer_Idle");
    }

    public override void MoveAbout()
    {
        if (timeToChangeDirections == true)
        {
            walkSpeed = 1f;
            switch (angles[dir])
            {
                
                case 0:
                    direction = new Vector2(walkSpeed, 0);
                    StartCoroutine(ChangeDirection());
                    break;
                case 90:
                    direction = new Vector2(0, walkSpeed);
                       StartCoroutine(ChangeDirection());
                    break;
                case 180:
                    direction = new Vector2(-walkSpeed, 0);
                       StartCoroutine(ChangeDirection());
                    break;
                case 270:
                    direction = new Vector2(0, -walkSpeed);
                    StartCoroutine(ChangeDirection());
                    break;
            }
        }
        isMoving = true;
    }

    public void OnDestroy()
    {
        GameManager.instance.enemyInstances.Remove(this.gameObject);
    }

    IEnumerator ChangeDirection()
    {
        dir = Random.Range(0, 4);
        timeToChangeDirections = false;
        yield return new WaitForSeconds(2f);
        timeToChangeDirections = true; 
        
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Walls")
        {
            rb.velocity = -rb.velocity;
        }
    }
}
