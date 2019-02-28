using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : StateMachineBehaviour
{

    public Transform playerPosition;
    public Vector3 position;

    public float speed;

    public bool hitBoxExists = false;

    public GameObject hitbox;
    public GameObject Enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play("Attack");
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, playerPosition.position, speed * Time.deltaTime);

        Vector2 heading = animator.transform.position - playerPosition.position;

        float distance = heading.magnitude;

        if (playerPosition.position.x < animator.transform.position.x)
        {
            Vector2 xscale = animator.transform.localScale;
            xscale.x = -1;
            animator.transform.localScale = xscale;
        }
        else if (playerPosition.position.x > animator.transform.position.x)
        {
            Vector2 xscale = animator.transform.localScale;
            xscale.x = 1;
            animator.transform.localScale = xscale;
        }

        //If Player is out of reach!!!
        if (distance > 1)
        {
            Destroy(Swordsman_Guard_Pawn.HitBox);
            animator.SetBool("isAtPlayer", false);
        }
        if (hitBoxExists == false)
        {
            hitBoxExists = !false;
            Instantiate(hitbox);
            hitbox.transform.position = position;
        }
        Debug.Log("AHHHHHHHHHHHHHHHH");
        
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
