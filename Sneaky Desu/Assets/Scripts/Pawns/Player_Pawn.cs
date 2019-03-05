using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Pawn : Pawn
{

    public override void Start()
    {
        base.Start(); //Our parent start method
    }

    public override void Update()
    {
        base.Update(); //Our parent update method
    }

    public override void MoveFoward()
    {
        fowardDown = Input.GetKey(KeyCode.UpArrow);

        //Going fowards
        if (fowardDown == true)
        {
            isWalking = true;

            //Deciding if we press right or left as we move up. This is what helps us diagonally move
            if (Input.GetKey(KeyCode.RightArrow) == true)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = 1;
                transform.localScale = xscale;
                move = new Vector2((float)rateOfSpeed, (float)rateOfSpeed);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) == true)
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

        leftDown = Input.GetKey(KeyCode.LeftArrow);

        if (leftDown == true)
        {
            isWalking = true;
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

        rightDown = Input.GetKey(KeyCode.RightArrow);

        if (rightDown == true)
        {
            isWalking = true;
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

        backwardsDown = Input.GetKey(KeyCode.DownArrow);

        //Going backwards
        if (backwardsDown == true)
        {
            isWalking = true;

            //Deciding if we press right or left as we move up. This is what helps us diagonally move
            if (Input.GetKey(KeyCode.RightArrow) == true)
            {
                Vector3 xscale = transform.localScale;
                xscale.x = 1;
                transform.localScale = xscale;
                move = new Vector2((float)rateOfSpeed, (float)-rateOfSpeed);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) == true)
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
            GameManager.instance.DecreaseHealth(0.1f);
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
