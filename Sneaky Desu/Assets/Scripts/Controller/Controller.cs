using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSL;

public abstract class Controller : DSLBehaviour
{
    public Pawn pawn; //The pawn in which we'll control

    // Start is called before the first frame update
    protected virtual void Initalize()
    {
        pawn = GetComponent<Pawn>(); //Grab anything that is of type Pawn
    }
}
