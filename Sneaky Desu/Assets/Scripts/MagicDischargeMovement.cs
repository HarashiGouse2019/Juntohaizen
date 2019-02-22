using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDischargeMovement : MonoBehaviour
{
    public Rigidbody2D rb; //We use this in able to apply physics to our object

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * Magic_Discharge.buffSpeed;
    }
}


    
