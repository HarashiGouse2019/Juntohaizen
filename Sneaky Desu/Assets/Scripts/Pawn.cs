using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStats;

public abstract class Pawn : MonoBehaviour
{
    public PlayerStatusScript Status;

    public static Vector3 originPosition;
    public Vector2 move;

    public Animator animator;
    

    public bool col = false, step = false;

    public bool isWaiting = false;

    public bool isWalking = false;

    //Initializing speed and the speed of rotation
    public float rateOfSpeed, maxSpeed;

    public float speed;

    public bool fowardDown, backwardsDown, rightDown, leftDown;

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


    } //The player moves up

    public virtual void MoveRight()
    {

    } //The player moves right

    public virtual void MoveBackwards()
    {

    } //The player moves down

    public virtual void MoveLeft()
    {

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
