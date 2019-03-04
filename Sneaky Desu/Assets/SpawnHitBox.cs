using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHitBox : StateMachineBehaviour
{
    public GameObject HitBoxPrefab;
    public GameObject target;
    GameObject hitbox;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = GameObject.FindGameObjectWithTag("Player");

        
            hitbox = Instantiate(HitBoxPrefab);
            hitbox.transform.position = target.transform.position;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
       hitbox.transform.position = target.transform.position;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      
           Destroy(hitbox);
    }
}
