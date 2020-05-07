using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer_Pawn : Pawn
{
    public static Sorcerer_Pawn Instance;
    public Sorcerer_Controller sController;

    public GameObject player;

    public Loot_Chances lootChances;

    public ShootPlasma shooPlasmaScript;

    public float walkSpeed;
    public float time = 1f;
    public float resetTime;

    public int[] angles = { 0, 90, 180, 270 };
    public int dir = 0;

    public bool isMoving = true;
    public bool timeToChangeDirections = true;
    public bool aggroState = false;
    public bool coolDown = false;

    private float manaReserve;

    public Vector2 direction;

    // Start is called before the first frame update
    protected override void Initialize()
    {
        base.Begin(); //Our parent start method;
        Instance = this;
        enemyHealth = 20f;
        manaReserve = enemyHealth * 2f;
        walkSpeed = 1f;
        sController = GetComponent<Sorcerer_Controller>();
        player = GameObject.FindGameObjectWithTag("Player");

        base.Initialize();
    }


    // Update is called once per frame
    protected override void Main()
    {
        

        //If the enemy's health reaches to 0
        if (enemyHealth < 1)
        {
            Player_Controller.player_controller.toggleLock = false;
            GameObject loot = Instantiate(lootChances.gameObject);
            loot.transform.position = transform.position;
            GameManager.Instance.IncreaseMana(manaReserve);
            Destroy(gameObject);
        }

        if (aggroState == true)
        {
            
            //Always face the player
            if (transform.position.x > player.transform.position.x)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = -1;
                transform.localScale = xscale;
            }
            else if (transform.position.x < player.transform.position.x)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = 1;
                transform.localScale = xscale;
            }
        }
        shooPlasmaScript.active = aggroState;

        base.Main(); //OUr parent update method
    }

    //These are all the states that the enemy will use
    public override void StandIdle()
    {
        animator.Play("Sorcerer_Idle");
    }

    public override void MoveAbout()
    {
        if (aggroState == false)
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
                isMoving = true;
            }
        } else 
        {
            Aggro();
        }
    }

    public override void Aggro()
    {
        float movementSpeed = 2f;
       
        rb.velocity = Vector2.zero;

        if (Vector2.Distance(transform.position, player.transform.position) > sController.stoppingDistance)
        {
            walkSpeed = movementSpeed;
            
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, walkSpeed * Time.deltaTime);
   
            isMoving = true;
        }
        else if (Vector2.Distance(transform.position, player.transform.position) < sController.stoppingDistance && Vector2.Distance(transform.position, player.transform.position) > sController.retreatDistance)
        {
            transform.position = this.transform.position;

            isMoving = false;
        }
        else if (Vector2.Distance(transform.position, player.transform.position) < sController.retreatDistance)
        {
            walkSpeed = movementSpeed;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -walkSpeed * Time.deltaTime);
            isMoving = true;
        }
    }

    public void OnDestroy()
    {
        GameManager.Instance.enemyInstances.Remove(this.gameObject);
    }

    IEnumerator ChangeDirection()
    {
        if (aggroState == false)
        {
            dir = Random.Range(0, 4);
            timeToChangeDirections = false;
            yield return new WaitForSeconds(2f);
            timeToChangeDirections = true;
        }
        
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Walls")
        {
            rb.velocity = -rb.velocity;
        }
    }
}
