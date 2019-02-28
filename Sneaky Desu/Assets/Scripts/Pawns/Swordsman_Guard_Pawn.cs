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
        
        if (animator.GetBool("isChasing") == false)
        {
            Destroy(this.HitBoxPrefab);
        }
    }

    public override void StandIdle()
    {
        
        animator.SetBool("isChasing", false);
    }

    public override void ChaseAfter()
    {

        animator.SetBool("isChasing", true);
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, target.position, speed * Time.deltaTime);

        Vector2 heading = animator.transform.position - target.position;

        float distance = heading.magnitude;

        
    }

    public override void Attack()
    {
        animator.SetBool("isAtPlayer", true);
        if (HitBox == null)
        {
            HitBox = Instantiate(HitBox);
        }
        HitBox.transform.position = currentPosition;
    }

    public override void ReturnToLastPosition()
    {
        
    }
}
