using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHitBox : StateMachineBehaviour
{
    public GameObject HitBoxPrefab; //Our hit box prefab
    public GameObject target; //Our target (which will be the player
    GameObject hitbox;

    AnimatorClipInfo animation;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = GameObject.FindGameObjectWithTag("Player"); //Find the player

        
            hitbox = Instantiate(HitBoxPrefab); //Instantiate our hit box
            hitbox.transform.position = target.transform.position; //The hit box will be placed on the player

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      
      Destroy(hitbox); //Once the animation ends, we destory our prefab
    }
}
