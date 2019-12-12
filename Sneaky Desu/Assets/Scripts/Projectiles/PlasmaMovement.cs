using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlasmaMovement : MonoBehaviour, IPooledObject
{ 
    Transform player;
    Vector3 target;
    Rigidbody2D rb;

    float speed;
    readonly ObjectPooler pooler;
    readonly private GameObject objectToDisable;

    public void OnObjectSpawn()
    {
            speed = Random.Range(125f, 500f);
            player = FindObjectOfType<Player_Pawn>().transform;
            target = (player.position - transform.position).normalized;
            rb = GetComponent<Rigidbody2D>();
            rb.velocity = (target * speed) * Time.deltaTime;
        
        //transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
