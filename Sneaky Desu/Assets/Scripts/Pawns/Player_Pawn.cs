using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Pawn : Pawn
{
    public float lockOnDistance;

    public override void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void Start()
    {
        base.Start(); //Our parent start method
    }

    public override void Update()
    {
        base.Update(); //Our parent update method

        foreach (GameObject obj in GameManager.instance.enemyInstances)
        {
            lockOnDistance = Vector3.Distance(obj.transform.position, transform.position);
        }

        //Set up all animator parameters
        animator.SetBool("isWalking", controller.isWalking);
        animator.SetBool("isDescending", controller.isDescending);
        animator.SetBool("isInGround", controller.isInGround);
        animator.SetBool("isAscending", controller.isAscending);

        if (GameManager.instance.currentHealth == 0)
        {
            Destroy(gameObject);
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
                Vector3 xscale = transform.localScale;
                xscale.x = 1;
                transform.localScale = xscale;
                move = new Vector2((float)rateOfSpeed, (float)rateOfSpeed);
            }
            else if (Input.GetKey(controller.left) == true)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = -1;
                transform.localScale = xscale;
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
            Vector3 xscale = transform.localScale;
            xscale.x = -1;
            transform.localScale = xscale;

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
            Vector3 xscale = transform.localScale;
            xscale.x = 1;
            transform.localScale = xscale;

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
                Vector3 xscale = transform.localScale;
                xscale.x = 1;
                transform.localScale = xscale;
                move = new Vector2((float)rateOfSpeed, (float)-rateOfSpeed);
            }
            else if (Input.GetKey(controller.left) == true)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = -1;
                transform.localScale = xscale;
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

        float lockDistance = 8f;

        if (lockOnDistance <= lockDistance) {
            switch (enable)
            {
                case true:
                    FindObjectOfType<AudioManager>().Play("LockOn");
                    //Camera_Follow.camera.InitiateLockOn(obj.transform);
                    Debug.Log("Lock On");
                    break;
                case false:
                    FindObjectOfType<AudioManager>().Play("LockOff");
                    Debug.Log("Lock Off");
                    break;
            }
        }

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

    private void OnTriggerEnter2D(Collider2D gem)
    {
        //If we come across some gems, we'll increase our level, our mana, decrease the existing amount of 
        //active gems in the scene and destory the gem game object that we collide with.
        if (gem.gameObject.tag == "Gem")
        {
            GameManager.instance.IncreaseLevel(2f);
            GameManager.instance.IncreaseMana(1f);
            --GameManager.instance.gemInstances;
            Destroy(gem.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        //This is for taking damage from the hit box
        if (col.gameObject.tag == "hitbox")
        {
            GameManager.instance.DecreaseHealth(10f);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //We'll destory the hitbox so that it doesn't mysterious linger in the game invisble
        if (col.gameObject.tag == "hitbox")
        {
            Destroy(col.gameObject);
        }
    }
}
