using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alarm;

public class Player_Controller : Controller
{
    public static Player_Controller player_controller;

    public Player_Pawn player;

    [HideInInspector] public bool toggleLock = false;

    public bool isWalking = false; //If the player is walking

    [HideInInspector] public bool isDescending = false; //If the player is starting to merge into ground

    [HideInInspector] public bool isInGround = false; //If the player is merged into the ground

    [HideInInspector] public bool isAscending = false; //If the player is surfacing.

    [HideInInspector] public bool isDashing = false; //If the player is dashing.

    [Header("Key Mapping KeyBoard")]
    //Map movement to a selected key
    int d = 0; //For toggling action buttons
    public int p = 0; //For toggling pause button; Unique.
    public KeyCode right = KeyCode.RightArrow; //Default for moving right
    public KeyCode left = KeyCode.LeftArrow; //Default for moving left
    public KeyCode up = KeyCode.UpArrow; //Default for moving up
    public KeyCode down = KeyCode.DownArrow; //Default for moving down
    public KeyCode descendKey = KeyCode.X; // //Default for Descending/Ascending; These two will be the same button
    public KeyCode ascendKey = KeyCode.X;  //
    public KeyCode shoot = KeyCode.Z; //Default for shooting
    public KeyCode lockOnKey = KeyCode.LeftShift; //Default for locking on/off
    public KeyCode interact = KeyCode.C; //interacting with save points
    public KeyCode pause = KeyCode.Q; //Pausing the game

    [Header("Key Mapping Game Pad")]
    public float gp_horizontal;
    public float gp_vertical;
    public bool gp_descend;
    public bool gp_shoot;
    public bool gp_lockOn;
    public bool gp_interact;
    public bool gp_pause;

    //Check if there's double tap in arrow keys
    int arrowTapAmount = 0;
    bool wasDoubleTapRegistered = false;
    Timer doubleTapTimer = new Timer(1);
    float doubleValDuration = 0.25f;

    /*We want to check whether the same
     * key has been pressed in order to 
     * register a double tap*/
    public KeyCode keyTapped1 = KeyCode.None;
    public KeyCode keyTapped2 = KeyCode.None;

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
        ReadArrowKeyInput();
    }

    void FixedUpdate()
    {
        RunPlayerControls();
    }

    void RunPlayerControls()
    {
        //Game Controls Updated Every Frame

        if (pawn.transitionOn == false)
        {
            if (pawn.col == false)
            {
                if (Input.GetKey(up))
                    pawn.MoveFoward(); //Player will move up

                else if (Input.GetKey(down))
                    pawn.MoveBackwards(); //Player will move down

                if (Input.GetKey(left))
                    pawn.MoveLeft(); //Player will move left

                else if (Input.GetKey(right))
                    pawn.MoveRight(); //Player will move right

                if ((Input.GetKeyUp(up) || Input.GetKeyUp(down) || Input.GetKeyUp(left) || Input.GetKeyUp(right)) && !isDashing)
                    isWalking = false;


                //Descending into the ground
                if (Input.GetKeyDown(descendKey))
                    if (GameManager.Instance.currentMana != 0)
                        pawn.Descend(); //Hides 

                    //Ascending from the ground
                    else if (Input.GetKeyUp(ascendKey))
                        pawn.Ascend();

                //Locking onto targets
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
                                {
                                    toggleLock = false;
                                    Magic_Movement.radius = 1f;
                                }
                                break;
                            case true:
                                toggleLock = false;
                                Magic_Movement.radius = 1f;
                                break;

                        }
                        pawn.LockTarget(toggleLock);
                        d = 1;
                    }
                }
                else if (Input.GetKeyUp(lockOnKey))
                    d = 0;
            }

            //Dashing
            if (CheckForDoubleTapping())
            {
                pawn.Dash();

            }

            //If the player is walking, play some audio
            if (isWalking == true)
            {
                pawn.Walk(); //Make our walking step noises
                pawn.isWaiting = false;
                pawn.timer.SetToZero(4, true);
            }
            else
            {
                pawn.timer.SetToZero(0, true);
                pawn.isWaiting = true;
                pawn.RecoveryWhileIdle(1f);
            }
        }
    }

    bool CheckForDoubleTapping()
    {
        if (!wasDoubleTapRegistered)
        {
            //So much freaking ifs man!!!

            //If the first tap was register, set the timer
            if (keyTapped1 != KeyCode.None)
                StartDoubleTapTimer();

            //If the 2 taps didn't match, reset them
            if (keyTapped2 != KeyCode.None && keyTapped2 != keyTapped1)
                ResetDoubleTapValues();

            isDashing = ((keyTapped1 != KeyCode.None && keyTapped2 != KeyCode.None) && keyTapped2 == keyTapped1);

        }
        return isDashing;
    }

    void ReadArrowKeyInput()
    {
        //We just assign all input through this function
        //This assigns the first and second key that's been tapped
        if (Input.GetKeyDown(left))
            AssignDoubleTapValues(left);

        if (Input.GetKeyDown(right))
            AssignDoubleTapValues(right);

        if (Input.GetKeyDown(up))
            AssignDoubleTapValues(up);

        if (Input.GetKeyDown(down))
            AssignDoubleTapValues(down);
    }

    void AssignDoubleTapValues(KeyCode _key)
    {
        if (keyTapped1 == KeyCode.None)
        {
            keyTapped1 = _key;
            return;
        }

        else if (keyTapped2 == KeyCode.None)
        {
            keyTapped2 = _key;
            return;
        }
    }

    void StartDoubleTapTimer()
    {
        doubleTapTimer.StartTimer(0);
        if (doubleTapTimer.SetFor(doubleValDuration, 0))
        {
            wasDoubleTapRegistered = false;
            ResetDoubleTapValues();
            doubleTapTimer.SetToZero(0, true);
        }
    }

    void ResetDoubleTapValues()
    {
        keyTapped1 = KeyCode.None;
        keyTapped2 = KeyCode.None;
        wasDoubleTapRegistered = false;
        doubleValDuration = 0.25f;
    }

    //void CheckForJoystick()
    //{
    //    gp_horizontal = Input.GetAxis("DPadX");
    //    gp_vertical = Input.GetAxis("DPadY");
    //    gp_descend = Input.GetButtonDown("Descend"); //2
    //    gp_shoot = Input.GetButton("Shoot"); //Axis 10
    //    gp_lockOn = Input.GetButtonDown("LockOn"); //4
    //    gp_interact = Input.GetButtonDown("Interact"); //3
    //    gp_pause = Input.GetButtonDown("Pause"); //7
    //}
}
