using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman_Guard_Pawn : Pawn
{
    public GameObject HitBoxPrefab;
    public static GameObject HitBox;

    

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        HitBox = HitBoxPrefab;
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
    }

    public override void StandIdle()
    {
        animator.Play("Idle");
        Debug.Log("Doing nothing Desu!!!");
    }

    public override void ChaseAfter()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) animator.Play("Chase");

        Debug.Log("Going after Player Desu!!!");

        if (HitBox != null) Destroy(HitBox);

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        
    }

    public override void Attack()
    {
        

        Debug.Log("Attacking Desu!!!");

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
