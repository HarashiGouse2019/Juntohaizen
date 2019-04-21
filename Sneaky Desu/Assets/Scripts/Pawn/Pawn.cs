using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Pawn : MonoBehaviour
{
    public static Vector3 originPosition, currentPosition; //the start position and our current position
    public Vector2 move; //this will be used for our player movement

    public Player_Controller controller;

    //Accessing our animation;
    [Header("Animator")]
    public Animator animator;
    public AnimatorStateInfo stateInfo;
    public int layerIndex;

    [HideInInspector] public bool transitionOn = false; //The transition between hiding in the ground and ascending from the ground

    [HideInInspector] public bool col = false, step = false; //One is used if we collide with something, the other is for making our stepping noises

    [HideInInspector] public bool isWaiting = false; //If the player is idle

    [HideInInspector] public bool manaDrop = false;

    //Initializing speed and the speed of rotation
    public float rateOfSpeed, maxSpeed;

    public float speed; //Speed (this will be set for our Enemy pawn)

    public float transitionDuration = 0.35f;

    public Vector2 heading; //This will be used for our Enemey pawn to track down the player

    public float distance; //This is for the enemy to know the distance between it and the player

    [HideInInspector] public float enemyHealth;

    public Transform playerPosition; //Player position

    public GameObject target; //Our target (which will be the player position)

    public bool fowardDown, backwardsDown, rightDown, leftDown; //Boolean used for player movement

    public Rigidbody2D rb; //Giving an identifier (or name) that'll reference our RigidBody!!

    public IEnumerator coroutine; //our corountine identifier
    public IEnumerator transitionCoroutine; //Specifically for transitions from hiding in ground to ascending
    public IEnumerator manaUsageCoroutine;

    public virtual void Awake()
    {

    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //Grab the component of the local RigidBody
        controller = GetComponent<Player_Controller>();
        originPosition = gameObject.transform.position; //The start position of this gameObject
        playerPosition = FindObjectOfType<Player_Pawn>().transform; //The player's position
    }

    // Update is called once per frame
    public virtual void Update()
    {

        //All fo this is so the enemy can chase down the player
        currentPosition = gameObject.transform.position;

        heading = transform.position - playerPosition.position;

        distance = heading.magnitude;
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

    public virtual void Descend()
    {

    } //The player transforms as he merges to the ground

    public virtual void Ascend()
    {

    } //The player transforms as he surfaces from the ground

    public virtual void LockTarget(bool enable)
    {
     
    }

    public virtual GameObject GetClosestEnemy()
    {

        return null;
    }

    public virtual void SavePlayer()
    {

    }

    public IEnumerator Walk()
    {
        //This will produce footstep noises as we move
        if (step == false)
        {
            AudioManager.audioManager.Play("Walk");
            step = true;
            yield return new WaitForSeconds((float)0.15);
            step = false;
        }
    }

    public IEnumerator DescendTransition(float duration)
    {
        transitionOn = true;
        yield return new WaitForSeconds(duration);
        controller.isDescending = false;
        controller.isInGround = true;
        transitionOn = false;
    }

    public IEnumerator AscendTransition(float duration)
    {
        transitionOn = true;
        yield return new WaitForSeconds(duration);
        controller.isAscending = false;
        controller.isInGround = false;
        manaDrop = false;
        transitionOn = false;
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

    //This is for example, when you are hiding in the ground, you are using a little bit of your mana.
    public IEnumerator PassiveManaUsage(float duration, float decreaseBy)
    {
        if (GameManager.instance.manaUI.fillAmount != 0)
        {

            GameManager.instance.DecreaseMana(decreaseBy);
            manaDrop = true;
            yield return new WaitForSeconds(duration);
            manaDrop = false;
        }
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

    //These functions are made for enemy Sorcerer
    public virtual void MoveAbout()
    {

    }

}
