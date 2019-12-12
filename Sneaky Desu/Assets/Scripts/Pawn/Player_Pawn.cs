using UnityEngine;
using UnityEngine.UI;

public class Player_Pawn : Pawn
{
    public static Player_Pawn playerpawn;
    public GameObject gameplayCameraPrefab;
    public GameObject crossHair;
    public GameObject closestObject;
    public GameObject enemyTarget;
    public GameObject MagicSource; //This will be a prefab that we instantiate when our level value is greater than 0
    public float lockOnDistance;
    public bool targetNear = false;
    public float dist = 0f, targetDist, closest = 4f;
    public CircleCollider2D player_collider;

    //This will be to add a blink effect
    public bool isRendererEnabled = true;
    private new SpriteRenderer renderer;

    //And let's check if we've actually been hit
    public bool gotHit = false;

    //The duration of dashing
    public float dashDuration = 0.5f;

    private GameManager manager;

    public float dAxisX, dAxisY;
    public override void Awake()
    {
        playerpawn = this;
        player_collider = GetComponent<CircleCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        manager = GameManager.instance;
        DontDestroyOnLoad(this);
    }
    public override void Start()
    {
        if (gameObject.activeInHierarchy)
            Instantiate(gameplayCameraPrefab);

        base.Start(); //Our parent start method
    }
    public override void Update()
    {
        base.Update(); //Our parent update method

        ReadJoystickValues();

        manaUsageCoroutine = PassiveManaUsage(0.05f, 1f);

        //Set up all animator parameters
        animator.SetBool("isWalking", controller.isWalking);
        animator.SetBool("isDescending", controller.isDescending);
        animator.SetBool("isInGround", controller.isInGround);
        animator.SetBool("isAscending", controller.isAscending);
        animator.SetBool("isDashing", controller.isDashing);

        if (GameManager.instance.currentHealth == 0)
        {
            step = false;
            isWaiting = false;
            GameManager.instance.ResetAllValues();
            closestObject = null;
            controller.toggleLock = false;
            player_collider.isTrigger = true;
            MagicSource.SetActive(false);
            gameObject.SetActive(false);
        }

        if (GameManager.instance.currentMana == 0)
            Ascend();

        GetClosestEnemy();
        if (closestObject == null)
        {
            targetNear = false;
        }


        //Checking if hiding in the ground
        if (controller.isInGround)
        {

            player_collider.radius = 0.1f;
            player_collider.offset = new Vector2(-0.00999999f, -0.4f);

            if (manaDrop == false) StartCoroutine(manaUsageCoroutine);

        }
        else
        {
            player_collider.radius = 0.32f;
            player_collider.offset = new Vector2(-0.00999999f, 0.02180004f);
            player_collider.isTrigger = false;

        }

        if (controller.toggleLock)
        {
            if (transform.position.x > closestObject.transform.position.x)
            {
                Flip(-1);
            }
            else if (transform.position.x < closestObject.transform.position.x)
            {
                Flip(1);
            }
        }

        //For our blinking effect
        if (gotHit == true)
        {
            GetHurt(0.15f, 5f);
            renderer.enabled = isRendererEnabled;
        }
        else
        {
            isRendererEnabled = true;
            renderer.enabled = isRendererEnabled;
        }
    }
    private void ReadJoystickValues()
    {
        dAxisX = Input.GetAxis("DPadX");
        dAxisY = Input.GetAxis("DPadY");
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

                if (!controller.toggleLock)
                {
                    Flip(1);
                }
                else
                {
                    if (transform.position.x > closestObject.transform.position.x)
                    {
                        Flip(-1);
                    }
                }

                move = (new Vector2(rb.velocity.x, (float)rateOfSpeed)) / 2;
            }
            else if (Input.GetKey(controller.left) == true)
            {
                if (controller.toggleLock == false)
                {
                    Flip(-1);
                }
                else
                {
                    if (transform.position.x < closestObject.transform.position.x)
                    {
                        Flip(1);
                    }
                }


                move = (new Vector2(-rb.velocity.x, (float)rateOfSpeed)) / 2;
            }
            else
                move = (new Vector2(rb.velocity.x, (float)rateOfSpeed));

            if (!controller.isDashing)
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
                Flip(-1);
            }
            else
            {
                if (transform.position.x < closestObject.transform.position.x)
                {
                    Flip(1);
                }
            }


            //Going fowards


            Vector2 move = (new Vector2((float)-rateOfSpeed, rb.velocity.y));

            if (!controller.isDashing)
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
                Flip(1);

            }
            else
            {
                if (closestObject != null && transform.position.x > closestObject.transform.position.x)
                {
                    Flip(-1);
                }
            }

            //Going fowards

            Vector2 move = (new Vector2((float)rateOfSpeed, rb.velocity.y));

            if (!controller.isDashing)
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
                    Flip(1);
                }
                else
                {
                    if (transform.position.x > closestObject.transform.position.x)
                    {
                        Flip(-1);
                    }
                }

                move = (new Vector2(rb.velocity.x, (float)-rateOfSpeed)) / 2;
            }
            else if (Input.GetKey(controller.left) == true)
            {
                if (controller.toggleLock == false)
                {
                    Flip(-1);
                }
                else
                {
                    if (transform.position.x < closestObject.transform.position.x)
                    {
                        Flip(1);
                    }
                }

                move = (new Vector2(-rb.velocity.x, (float)-rateOfSpeed)) / 2;
            }
            else
                move = (new Vector2(rb.velocity.x, (float)-rateOfSpeed));

            if (!controller.isDashing)
                if (rb.velocity.magnitude < maxSpeed)
                    rb.velocity += move * Time.fixedDeltaTime;

        }
    }
    public override void LockTarget(bool enable)
    {
        switch (enable)
        {
            case true:
                if (targetNear == true && enemyTarget != null)
                {
                    //closest = dist;
                    Instantiate(crossHair);
                    crossHair.transform.position = new Vector2(enemyTarget.transform.position.x, enemyTarget.transform.position.y);
                    AudioManager.audioManager.Play("LockOn");
                    Magic_Movement.radius = 0.5f;
                }
                break;
            case false:
                AudioManager.audioManager.Play("LockOff");
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
        if (transitionOn == false && controller.isInGround == false)
        {
            controller.isWalking = false;
            controller.isDescending = true;
            transitionCoroutine = DescendTransition(transitionDuration);
            StartCoroutine(transitionCoroutine);
        }

        //When you want to Ascend
        if (Input.GetKeyDown(controller.ascendKey))
            Ascend();
    }
    public override void Ascend()
    {
        StopCoroutine(manaUsageCoroutine);
        if (transitionOn == false && controller.isInGround == true)
        {
            controller.isWalking = false;
            controller.isAscending = true;
            manaDrop = false;
            transitionCoroutine = AscendTransition(transitionDuration);
            StartCoroutine(transitionCoroutine);
        }
    }
    public override void Dash()
    {
        if (controller.isDashing)
        {
            GameObject ghostTrail = ObjectPooler.instance.SpawnFromPool("playerGhostTrail", transform.position, Quaternion.identity);
            if (!ghostTrail.activeInHierarchy)
                ghostTrail.SetActive(true);

            controller.isWalking = !controller.isDashing;
            if (controller.keyTapped2 == controller.left)
                rb.velocity = new Vector2(-maxSpeed / 3, 0);

            else if (controller.keyTapped2 == controller.right)
                rb.velocity = new Vector2(maxSpeed / 3, 0);

            if (controller.keyTapped2 == controller.up)
                rb.velocity = new Vector2(0, maxSpeed / 3);

            else if (controller.keyTapped2 == controller.down)
                rb.velocity = new Vector2(0, -maxSpeed / 3);
        }
    }
    public override void SavePlayer()
    {
        SaveLoadSystem.SavePlayer(this);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        Collider2D obj = col;


        //If we come across some gems, we'll increase our level, our mana, decrease the existing amount of 
        //active gems in the scene and destory the gem game object that we collide with.
        switch (obj.gameObject.tag)
        {
            case "Lives":
                manager.IncreaseHealth(5f);
                obj.gameObject.SetActive(false);
                break;
            case "xLives":
                manager.IncreaseHealth(20f);
                obj.gameObject.SetActive(false);
                break;
            case "hitbox":
                if (gotHit == false)
                {
                    gotHit = true;
                    manager.DecreaseHealth(10f);
                }
                break;
            case "Plasma":
                if (gotHit == false)
                {
                    manager.DecreaseHealth(15f);
                    col.gameObject.SetActive(false);
                    gotHit = true;
                }
                break;
            case "Trigger":
                TriggerEvent.system.Trigger();
                break;
        }
    }
    private void GetHurt(float _blinkRate, float _duration)
    {
        if (gotHit == true)
        {
            timer.StartTimer(6);
            timer.StartTimer(8);
            //I want it so that the player is blinking on and off for a certain duration of time.
            //That would mean getting to the Sprite Renderer, and enabling it and disabling it after
            //certain intervals.

            //Since there's a timer in Pawn, and I've initialized 12, I'm going to use alarm 6
            //We'll pass the blink rate to our SetFor method.
            returnVal = timer.SetFor(_duration, 8, true);
            if (timer.SetFor(_blinkRate, 6))
            {
                if (isRendererEnabled) isRendererEnabled = false;
                else if (isRendererEnabled == false) isRendererEnabled = true;
            }

            if (returnVal)
            {
                gotHit = false;
                timer.SetToZero(6, true);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        Collider2D savePoint = col;

        switch (col.gameObject.tag)
        {
            case "SavePoint":
                SavePoint obj = savePoint.gameObject.GetComponent<SavePoint>();
                StartCoroutine(obj.Show(0.005f));
                bool save = false;
                if (Input.GetKeyDown(controller.interact) && rb.velocity.magnitude < 1 && save == false)
                {
                    SavePlayer();
                    Dialogue dialogueList = manager.GetComponent<Dialogue>();
                    dialogueList.Run(0, 0.05f);
                }
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        Collider2D savePoint = col;

        switch (col.gameObject.tag)
        {
            //We'll destory the hitbox so that it doesn't mysterious linger in the game invisble
            case "Enemy":
                player_collider.isTrigger = false;
                break;
            case "SavePoint":
                SavePoint obj = savePoint.gameObject.GetComponent<SavePoint>();
                obj.toggle = true;
                StartCoroutine(obj.Hide());
                break;
        }

    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Gem":
                manager.IncreaseLevel(2f);
                manager.IncreaseMana(1f);
                manager.totalGems++;
                col.gameObject.SetActive(false);
                break;
            case "Enemy":
                player_collider.isTrigger = true;
                break;

        }
    }
}