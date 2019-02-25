using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStats;

public abstract class Pawn : MonoBehaviour
{
    public static Vector3 originPosition;
    Vector2 move;

    public Animator animator;
    public PlayerStatusScript Status;

    public bool col = false, step = false;

    public bool isWaiting = false;

    public bool isWalking = false;

    //Initializing speed and the speed of rotation
    public float rateOfSpeed, maxSpeed;

    public float speed;

    private bool fowardDown, backwardsDown, rightDown, leftDown;

    public Rigidbody2D rb; //Giving an identifier (or name) that'll reference our RigidBody!!

    public IEnumerator coroutine;

    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //Grab the component of the local RigidBody
        originPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void MoveFoward()
    {

        fowardDown = Input.GetKey(KeyCode.UpArrow);

        //Going fowards
        if (fowardDown == true)
        {
            isWalking = true;

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
    } //The player moves up

    public virtual void MoveRight()
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
    } //The player moves right

    public virtual void MoveBackwards()
    {


        backwardsDown = Input.GetKey(KeyCode.DownArrow);


        //Going backwards
        if (backwardsDown == true)
        {
            isWalking = true;

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


    } //The player moves down

    public virtual void MoveLeft()
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

    public IEnumerator Walk()
    {

        if (step == false)
        {
            FindObjectOfType<AudioManager>().Play("Walk");
            Status.DecreaseHealth(1f);
            step = true;
            yield return new WaitForSeconds((float)0.15);
            step = false;
        }
    }

    public IEnumerator RecoveryWhileIdle(float value)
    {
        if (Status.healthUI.fillAmount != Status.maxHealth)
        {
            Status.IncreaseHealth(1f);
            isWaiting = true;
        }
        yield return new WaitForSeconds(value);
        isWaiting = false;
    }
}
