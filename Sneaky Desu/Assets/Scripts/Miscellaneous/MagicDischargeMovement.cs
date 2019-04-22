using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDischargeMovement : MonoBehaviour, IPooledObject
{
    public Rigidbody2D rb; //We use this in able to apply physics to our object
    public GameObject Player; //A reference to our player game object

    float baseSpeed; //The default speed when we shoot

    Vector2 xscale; //Our x scale, which we'll use to flip the image to have face right or left.

    private bool start = true; //Start delay boolean

    IEnumerator delayCoroutine; //Delay coroutine 

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        xscale = gameObject.transform.localScale;
        xscale.x = Player.transform.localScale.x;
        gameObject.transform.localScale = xscale;

        baseSpeed = Magic_Discharge.buffSpeed; //The basespeed will be modified depend on the level

        
    }
    public void OnObjectSpawn()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        delayCoroutine = Delay(0.60f); //delay by 1 second
        float speedInFront = Magic_Discharge.buffSpeed + 5;
        float speedBehind = Magic_Discharge.buffSpeed - 2;
        StartCoroutine(delayCoroutine);
        //Whenever we are shooting, our point of fire will be rotating around the player
        //As we move forward, there are times when the bullets are closer to each other
        //whenever there'a max or min of our point of fire's movement (below the player, or above the player)
        //Through this set of code, we prevent that from happening
        switch (Player.transform.localScale.x)
        {
            case 1:
                if (Mathf.Sign(Mathf.Cos(Magic_Movement.angle)) == -1)
                {
                    baseSpeed = speedInFront;
                }
                else
                {
                    baseSpeed = speedBehind;
                }
                break;
            case -1:
                if (Mathf.Sign(Mathf.Cos(Magic_Movement.angle)) == 1)
                {
                    baseSpeed = speedInFront;
                }
                else
                {
                    baseSpeed = speedBehind;
                }
                break;
        }

        //The projectory of our "bullet"
        rb.velocity = transform.right * baseSpeed * Player.transform.localScale.x;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {

        Magic_Discharge magic_discharge = FindObjectOfType<Magic_Discharge>();

        //If we hit an enemy, destory itself and the object that it collises with.
        if (col.gameObject.tag == "Enemy")
        {
            switch(magic_discharge.type)
            {
                case 0:
                    TakeDamage(col.gameObject, 1f);
                    break;
                case 1:
                    TakeDamage(col.gameObject, 5f);
                    break;
                case 2:
                    TakeDamage(col.gameObject, 10f);
                    break;
            }
            gameObject.SetActive(false); //Deactivates itself since it's from an Object Pool
        }
        
    }

    IEnumerator Delay(float time)
    {
        if (start == false)
        {
            start = true;
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false); //Destroys self after the yield time
            start = false;
        }
    }

    void TakeDamage(GameObject victim, float damage)
    {
        Pawn enemy = victim.GetComponent<Pawn>();
        enemy.enemyHealth -= damage;
    }
}


    
