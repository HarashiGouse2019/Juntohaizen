using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaMovement : MonoBehaviour, IPooledObject
{
    ObjectPooler pooler;

    float speed = 2f;
    Transform player;
    Vector3 target;
    private GameObject objectToDisable;
    Rigidbody2D rb;

    public void OnObjectSpawn()
    {
       
            player = FindObjectOfType<Player_Pawn>().transform;
            target = player.position - transform.position;
            rb = GetComponent<Rigidbody2D>();
            rb.velocity = speed * target;
        
        //transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
