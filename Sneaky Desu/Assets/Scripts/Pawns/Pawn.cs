using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Pawn : MonoBehaviour
{
    public static Vector3 originPosition, currentPosition; //the start position and our current position
    public Vector2 move; //this will be used for our player movement

    //Accessing our animation;
    public Animator animator;
    public AnimatorStateInfo stateInfo;
    public int layerIndex;


    public bool col = false, step = false; //One is used if we collide with something, the other is for making our stepping noises

    public bool isWaiting = false; //If the player is idle

    public bool isWalking = false; //If the player is walking

    //Initializing speed and the speed of rotation
    public float rateOfSpeed, maxSpeed;

    public float speed; //Speed (this will be set for our Enemy pawn)

    public Vector2 heading; //This will be used for our Enemey pawn to track down the player

    public float distance; //This is for the enemy to know the distance between it and the player

    public Transform playerPosition; //Player position

    public GameObject target; //Our target (which will be the player position)

    public bool fowardDown, backwardsDown, rightDown, leftDown; //Boolean used for player movement

    public Rigidbody2D rb; //Giving an identifier (or name) that'll reference our RigidBody!!

    public IEnumerator coroutine; //our corountine identifier

    [Header("Key Mapping")]
    //Map movement to a selected key
    public KeyCode right = KeyCode.RightArrow;
    public KeyCode left = KeyCode.LeftArrow;
    public KeyCode jump = KeyCode.Z;

    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //Grab the component of the local RigidBody
        originPosition = gameObject.transform.position; //The start position of this gameObject
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform; //The player's position
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //All fo this is so the enemy can chase down the player
        currentPosition = gameObject.transform.position;

        heading = transform.position - playerPosition.position;

        distance = heading.magnitude; Debug.Log(distance);
    }

    //Player Behaviour Functions

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
        //This will produce footstep noises as we move
        if (step == false)
        {
            FindObjectOfType<AudioManager>().Play("Walk");
            step = true;
            yield return new WaitForSeconds((float)0.15);
            step = false;
        }
    }

    public IEnumerator RecoveryWhileIdle(float value)
    {
        //If we take any amount of damage and we happen to not be moving, we have our health increase by 1 by a defined value of seconds
        if (GameManager.instance.healthUI.fillAmount != GameManager.instance.maxHealth)
        {
            GameManager.instance.IncreaseHealth(1f);
            isWaiting = true;
        }
        yield return new WaitForSeconds(value);
        isWaiting = false;
    }

    //Enemy Behaviour Functions

    //State for staying still
    public virtual void StandIdle()
    {

    }
    //State for chasing after the player
    public virtual void ChaseAfter()
    {

    }
    //State for attacking the player
    public virtual void Attack()
    {

    }

}
