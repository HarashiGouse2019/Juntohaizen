using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : Controller
{
    public static Player_Controller player_controller;

    public Player_Pawn player;

    [HideInInspector] public bool toggleLock = false;

    [HideInInspector] public bool isWalking = false; //If the player is walking

    [HideInInspector] public bool isDescending = false; //If the player is starting to merge into ground

    [HideInInspector] public bool isInGround = false; //If the player is merged into the ground

    [HideInInspector] public bool isAscending = false; //If the player is surfacing.

    [Header("Key Mapping")]
    //Map movement to a selected key
    int d = 0; //For toggling
    public KeyCode right = KeyCode.RightArrow; //Default for moving right
    public KeyCode left = KeyCode.LeftArrow; //Default for moving left
    public KeyCode up = KeyCode.UpArrow; //Default for moving up
    public KeyCode down = KeyCode.DownArrow; //Default for moving down
    public KeyCode descendKey = KeyCode.X; // //Default for Descending/Ascending; These two will be the same button
    public KeyCode ascendKey = KeyCode.X;  //
    public KeyCode shoot = KeyCode.Z; //Default for shooting
    public KeyCode lockOnKey = KeyCode.LeftShift; //Default for locking on/off
    public KeyCode interact = KeyCode.C; //interacting with save points
    
    public void Awake()
    {
        if (player_controller == null)
            player_controller = this;
        player = FindObjectOfType<Player_Pawn>();

    }

    public override void Start()
    {
        base.Start(); //Calls our parent start function
        pawn.transitionCoroutine = pawn.DescendTransition(1f);
    }

    void Update()
    {
        ascendKey = descendKey;
    }

    void FixedUpdate()
    {
        //Game Controls Updated Every Frame
        if (pawn.transitionOn == false)
        {
            pawn.coroutine = pawn.Walk(); //Coroutine to make our walking sound
            if (pawn.col == false)
            {

                if (Input.GetKey(up))
                    pawn.MoveFoward(); //Player will move up

                else if (Input.GetKey(down))
                    pawn.MoveBackwards(); //Player will move down

                else if (Input.GetKey(left))
                    pawn.MoveLeft(); //Player will move left

                else if (Input.GetKey(right))
                    pawn.MoveRight(); //Player will move right

                else
                {
                    pawn.coroutine = pawn.RecoveryWhileIdle(1f); //Increases our health when we are not moving; Our player is resting
                    if (pawn.isWaiting == false) StartCoroutine(pawn.coroutine); else StopCoroutine(pawn.coroutine);
                    isWalking = false;
                }

                if (Input.GetKeyDown(descendKey))
                    pawn.Descend(); //Hides 

                else if (Input.GetKeyUp(ascendKey))
                    pawn.Ascend();

                if (Input.GetKeyDown(lockOnKey))
                {
                    if (d == 0)
                    {

                        switch (toggleLock)
                        {
                            case false:
                                if (player.targetNear == true)
                                    toggleLock = true;
                                else
                                   toggleLock = false;
                                break;
                            case true:
                                toggleLock = false;
                                break;

                        }
                        pawn.LockTarget(toggleLock);
                        d = 1;
                    }
                }
                else if (Input.GetKeyUp(lockOnKey))
                {
                    d = 0;
                }
            }

            if (isWalking == true)
            {
                StartCoroutine(pawn.coroutine); //Start our coroutine walking steps
            }
        } 
    }
}
