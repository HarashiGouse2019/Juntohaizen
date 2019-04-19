using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MasterSounds.AudioManager;

public class Player_Pawn : Pawn
{
    public static Player_Pawn playerpawn;

    public GameObject crossHair;
    public  GameObject closestObject;
    public GameObject enemyTarget;
    public float lockOnDistance;
    public bool targetNear = false;
    public float dist = 0f, targetDist, closest = 4f;
    CircleCollider2D player_collider; 

    public override void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void Start()
    {
        base.Start(); //Our parent start method
        player_collider = GetComponent<CircleCollider2D>();
    }

    public override void Update()
    {
        base.Update(); //Our parent update method

        //Set up all animator parameters
        animator.SetBool("isWalking", controller.isWalking);
        animator.SetBool("isDescending", controller.isDescending);
        animator.SetBool("isInGround", controller.isInGround);
        animator.SetBool("isAscending", controller.isAscending);

        if (GameManager.instance.currentHealth == 0)
        {
            GameManager.instance.ResetAllValues();
            gameObject.SetActive(false);
        }

        GetClosestEnemy();
        if (closestObject == null)
        {
            targetNear = false;
        }

        
        //Checking if hiding in the ground
        if (controller.isInGround == true)
        {

            player_collider.radius = 0.1f;
            player_collider.offset = new Vector2(-0.00999999f, -0.4f);

            manaUsageCoroutine = PassiveManaUsage(1f, 1f);;

            StartCoroutine(manaUsageCoroutine);
        }  else
        {
            player_collider.radius = 0.32f;
            player_collider.offset = new Vector2(-0.00999999f, 0.02180004f);


        }
    }

    public override void MoveFoward()
    {
        fowardDown = Input.GetKey(controller.up);

        //Going fowards
        if (fowardDown == true)
        {
            controller.isWalking = true;

            //Deciding if we press right or left as we move up. This is what helps us diagonally move
            if (Input.GetKey(controller.right) == true)
            {
                if (controller.toggleLock == false)
                {
                    Vector3 xscale = transform.localScale;
                    xscale.x = 1;
                    transform.localScale = xscale;
                }
                else
                {
                    if (transform.position.x > closestObject.transform.position.x)
                    {
                        Vector3 xscale = transform.localScale;
                        xscale.x = -1;
                        transform.localScale = xscale;
                    }
                }

                move = new Vector2((float)rateOfSpeed, (float)rateOfSpeed);
            }
            else if (Input.GetKey(controller.left) == true)
            {
                if (controller.toggleLock == false)
                {
                    Vector3 xscale = transform.localScale;
                    xscale.x = -1;
                    transform.localScale = xscale;
                }
                else
                {
                    if (transform.position.x < closestObject.transform.position.x)
                    {
                        Vector3 xscale = transform.localScale;
                        xscale.x = 1;
                        transform.localScale = xscale;
                    }
                }


                move = new Vector2((float)-rateOfSpeed, (float)rateOfSpeed);
            }
            else
                move = new Vector2(0, (float)rateOfSpeed);

            if (rb.velocity.magnitude < maxSpeed)
                rb.velocity += move * Time.fixedDeltaTime;


        }
    }

    public override void MoveLeft()
    {

        leftDown = Input.GetKey(controller.left);

        if (leftDown == true)
        {
            controller.isWalking = true;
            if (controller.toggleLock == false)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = -1;
                transform.localScale = xscale;
            }
            else
            {
                if (transform.position.x < closestObject.transform.position.x)
                {
                    Vector3 xscale = transform.localScale;
                    xscale.x = 1;
                    transform.localScale = xscale;
                }
            }


            //Going fowards


            Vector2 move = new Vector2((float)-rateOfSpeed, 0);
            if (rb.velocity.magnitude < maxSpeed)
                rb.velocity += move * Time.fixedDeltaTime;

        }

    }

    public override void MoveRight()
    {

        rightDown = Input.GetKey(controller.right);

        if (rightDown == true)
        {
            controller.isWalking = true;
            if (controller.toggleLock == false)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = 1;
                transform.localScale = xscale;

            }
            else
            {
                if (transform.position.x > closestObject.transform.position.x)
                {
                    Vector3 xscale = transform.localScale;
                    xscale.x = -1;
                    transform.localScale = xscale;
                }
            }

            //Going fowards

            Vector2 move = new Vector2((float)rateOfSpeed, 0);
            if (rb.velocity.magnitude < maxSpeed)
                rb.velocity += move * Time.fixedDeltaTime;

        }
    }

    public override void MoveBackwards()
    {

        backwardsDown = Input.GetKey(controller.down);

        //Going backwards
        if (backwardsDown == true)
        {
            controller.isWalking = true;

            //Deciding if we press right or left as we move up. This is what helps us diagonally move
            if (Input.GetKey(controller.right) == true)
            {
                if (controller.toggleLock == false)
                {
                    Vector3 xscale = transform.localScale;
                    xscale.x = 1;
                    transform.localScale = xscale;
                } else
                {
                    if (transform.position.x > closestObject.transform.position.x)
                    {
                        Vector3 xscale = transform.localScale;
                        xscale.x = -1;
                        transform.localScale = xscale;
                    }
                }

                move = new Vector2((float)rateOfSpeed, (float)-rateOfSpeed);
            }
            else if (Input.GetKey(controller.left) == true)
            {
                if (controller.toggleLock == false)
                {
                    Vector3 xscale = transform.localScale;
                    xscale.x = -1;
                    transform.localScale = xscale;
                } else
                {
                    if (transform.position.x < closestObject.transform.position.x)
                    {
                        Vector3 xscale = transform.localScale;
                        xscale.x = 1;
                        transform.localScale = xscale;
                    }
                }

                move = new Vector2((float)-rateOfSpeed, (float)-rateOfSpeed);
            }
            else
                move = new Vector2(0, (float)-rateOfSpeed);

            if (rb.velocity.magnitude < maxSpeed)
                rb.velocity += move * Time.fixedDeltaTime;

        }
    }

    public override void LockTarget(bool enable)
    {
        switch (enable)
        {
            case true:
                    if (targetNear == true)
                    {
                        //closest = dist;
                        Instantiate(crossHair);
                        crossHair.transform.position = new Vector2(enemyTarget.transform.position.x, enemyTarget.transform.position.y);
                        audioManager.Play("LockOn");
                        Magic_Movement.radius = 0.5f;
                    }
                break;
            case false:
                audioManager.Play("LockOff");
                
                break;
        }

    }

    public override GameObject GetClosestEnemy()
    {
        //We'll iterate through a list of enemy GameObjects, and see which one is closest to us.
        //Once we get the closest one, we assign that object into the closestObject gameObject
        //We then create our crossHair on our new target.
        if (GameManager.instance.enemyInstances != null)
        {
            for (int i = 0; i < GameManager.instance.enemyInstances.Count; i++)
            {

                dist = Vector3.Distance(GameManager.instance.enemyInstances[i].transform.position, transform.position);
                if (dist <= closest)
                {
                    //closest = dist;
                    closestObject = GameManager.instance.enemyInstances[i];
                    enemyTarget = closestObject;
                    crossHair.transform.position = new Vector2(enemyTarget.transform.position.x, enemyTarget.transform.position.y);
                    targetNear = true;
                    return closestObject;
                }
            }
        }

        //The function will still try to look for the closest enemy in the scene. If the current target is far, closestObject is set to null;
        if (enemyTarget != null)
        {
            if (Vector3.Distance(enemyTarget.transform.position, transform.position) > closest)
            {
                targetNear = false;
                closestObject = null;
                enemyTarget = closestObject;
                crossHair.transform.position = new Vector2(transform.position.x, transform.position.y);
                Player_Controller.player_controller.toggleLock = targetNear;
                Magic_Movement.radius = 1f;
            }
        }
        Magic_Movement.radius = 1f;
        return null;
    }

    public override void Descend()
    {
        if (transitionOn == false && controller.isInGround == false) {
            controller.isWalking = false;
            controller.isDescending = true;
            transitionCoroutine = DescendTransition(transitionDuration);
            StartCoroutine(transitionCoroutine);
        } 
    }

    public override void Ascend()
    {
        if (transitionOn == false)
        {
            controller.isWalking = false;
            controller.isAscending = true;
            transitionCoroutine = AscendTransition(transitionDuration);
            StartCoroutine(transitionCoroutine);
        }
    }

    public override void SavePlayer()
    {
        SaveLoadSystem.SavePlayer(this);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameManager manager; //Get a reference of our Game Manager

        manager = GameManager.instance; //Have it equal to the static instance of Game Manager

        Collider2D collectibles = col;


        //If we come across some gems, we'll increase our level, our mana, decrease the existing amount of 
        //active gems in the scene and destory the gem game object that we collide with.
        if (collectibles.gameObject.tag == "Gem")
        {
            manager.IncreaseLevel(2f);
            manager.IncreaseMana(1f);
            manager.totalGems++;
            collectibles.gameObject.SetActive(false);
        } else if (collectibles.gameObject.tag == "Lives")
        {
            manager.IncreaseHealth(5f);
            collectibles.gameObject.SetActive(false);
        } else if (collectibles.gameObject.tag == "xLives")
        {
            manager.IncreaseHealth(20f);
            collectibles.gameObject.SetActive(false);
        } 
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        Collider2D savePoint = col;

        //This is for taking damage from the hit box
        if (col.gameObject.tag == "hitbox")
        {
            GameManager.instance.DecreaseHealth(3f);
        }
        else if (savePoint.gameObject.tag == "SavePoint")
        {
            Debug.Log(savePoint.tag);
            if (Input.GetKeyDown(controller.interact))
            {
                SavePlayer(); Debug.Log("You Just Saved!!!!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //We'll destory the hitbox so that it doesn't mysterious linger in the game invisble
        if (col.gameObject.tag == "hitbox")
        {
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "Enemy")
        {
            player_collider.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            player_collider.isTrigger = true;
        }
    }
}
