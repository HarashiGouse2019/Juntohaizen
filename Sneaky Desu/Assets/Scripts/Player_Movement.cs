using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public static Vector3 originPosition;

    public Animator animator;

    //Initializing speed and the speed of rotation
    public float rateOfSpeed, maxSpeed;

    public float speed;

    private bool fowardDown, backwardsDown, rightDown, leftDown;

    bool isWalking = false;

    private Rigidbody2D rb; //Giving an identifier (or name) that'll reference our RigidBody!!

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //Grab the component of the local RigidBody
        originPosition = gameObject.transform.position;
    }

    void FixedUpdate()
    {
        animator.SetBool("isWalking", isWalking);
        //Game Controls Updated Every Frame
        if (Input.GetKey(KeyCode.UpArrow))
            MoveFoward();

        else if (Input.GetKey(KeyCode.DownArrow))
            MoveBackwards();

        else if (Input.GetKey(KeyCode.LeftArrow))
            MoveLeft();

        else if (Input.GetKey(KeyCode.RightArrow))
            MoveRight();

        else isWalking = false;

    }

    void MoveFoward()
    {
        
        fowardDown = Input.GetKey(KeyCode.UpArrow);

        //Going fowards
        if (fowardDown == true)
        {
            isWalking = true;
            Vector2 move = new Vector2(0, (float)rateOfSpeed);
            if (rb.velocity.magnitude < maxSpeed)
                rb.velocity += move;
            
        }
    } //The player moves up

    void MoveRight()
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
                rb.velocity += move;

        }
    } //The player moves right

    void MoveBackwards()
    {

        
        backwardsDown = Input.GetKey(KeyCode.DownArrow);


        //Going backwards
        if (backwardsDown == true)
        {
            isWalking = true;
            Vector2 move = new Vector2(0, (float)-rateOfSpeed);
            if (rb.velocity.magnitude < maxSpeed)
                rb.velocity += move;
            
        }
       
        
    } //The player moves down

    void MoveLeft()
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
                rb.velocity += move;
            

        } 

    } //The player moves left
}
