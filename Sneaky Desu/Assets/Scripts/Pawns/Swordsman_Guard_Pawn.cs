using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman_Guard_Pawn : Pawn
{
    Transform target;
    public GameObject HitBoxPrefab;
    public static GameObject HitBox;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        HitBox = HitBoxPrefab;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void StandIdle()
    {
        Debug.Log("Doing nothing Desu!!!");
        animator.SetBool("isChasing", false);
    }

    public override void ChaseAfter()
    {
        Debug.Log("Going after Player Desu!!!");

        animator.SetBool("isAtPlayer", false);
        animator.SetBool("isChasing", true);
        if (HitBox != null) Destroy(HitBox);

        animator.transform.position = Vector2.MoveTowards(animator.transform.position, target.position, speed * Time.deltaTime);

        Vector2 heading = animator.transform.position - target.position;

        float distance = heading.magnitude;

        
    }

    public override void Attack()
    {
        Debug.Log("Attacking Desu!!!");
        animator.SetBool("isAtPlayer", true);

        if (HitBox == null)
        {
            HitBox = Instantiate(HitBox, gameObject.transform);
            HitBox.transform.position = currentPosition;
        }

    }

    public override void ReturnToLastPosition()
    {
        
    }
}
