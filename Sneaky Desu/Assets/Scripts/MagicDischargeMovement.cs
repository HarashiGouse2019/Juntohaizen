using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDischargeMovement : MonoBehaviour
{
    public Rigidbody2D rb; //We use this in able to apply physics to our object
    public GameObject Player;

    Vector2 xscale;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
        xscale = gameObject.transform.localScale;
        xscale.x = Player.transform.localScale.x;
        gameObject.transform.localScale = xscale;



        switch (gameObject.transform.localScale.x)
        {
            case 1:
                if (Mathf.Sign(Mathf.Cos(Magic_Movement.angle)) == -1)
                {
                    Magic_Discharge.buffSpeed -= 5f;
                } else
                {
                    Magic_Discharge.buffSpeed += 5f;
                }
                break;
            case -1:
                if (Mathf.Sign(Mathf.Cos(Magic_Movement.angle)) == 1)
                {
                    Magic_Discharge.buffSpeed += 5f;
                }
                else
                {
                    Magic_Discharge.buffSpeed -= 5f;
                }
                break;
        }

        rb.velocity = transform.right * Magic_Discharge.buffSpeed * Player.transform.localScale.x;
    }
}


    
