using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDischargeMovement : MonoBehaviour
{
    public Rigidbody2D rb; //We use this in able to apply physics to our object
    public GameObject Player;

    float baseSpeed;

    Vector2 xscale;

    private bool start;

    IEnumerator delayCoroutine;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
        xscale = gameObject.transform.localScale;
        xscale.x = Player.transform.localScale.x;
        gameObject.transform.localScale = xscale;

        baseSpeed = Magic_Discharge.buffSpeed;

        delayCoroutine = Delay(1f);
    }
    void Start()
    {
        switch (Player.transform.localScale.x)
        {
            case 1:
                if (Mathf.Sign(Mathf.Cos(Magic_Movement.angle)) == -1)
                {
                    baseSpeed = Magic_Discharge.buffSpeed;
                }
                else
                {
                    baseSpeed += 5;
                }
                break;
            case -1:
                if (Mathf.Sign(Mathf.Cos(Magic_Movement.angle)) == 1)
                {
                    baseSpeed = Magic_Discharge.buffSpeed;
                }
                else
                {
                    baseSpeed += 5;
                }
                break;
        }
        rb.velocity = transform.right * baseSpeed * Player.transform.localScale.x;

        StartCoroutine(delayCoroutine);
    }

    void Update()
    {
        Debug.Log("Buff Speed is currently " + baseSpeed);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Destroy(col.gameObject);        
        }
    }

    IEnumerator Delay(float time)
    {
        start = true;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        start = false;
    }
}


    
