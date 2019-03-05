using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public Pawn pawn; //The pawn in which we'll control

    // Start is called before the first frame update
    public virtual void Start()
    {
        pawn = GetComponent<Pawn>(); //Grab anything that is of type Pawn
    }

}
