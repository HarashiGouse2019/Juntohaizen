using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disperse : MonoBehaviour , IPooledObject
{
    Rigidbody2D rb;
    Vector3 randomDirection;
    public void OnObjectSpawn()
    {
        float speed = 10f;
        rb = GetComponent<Rigidbody2D>();
        randomDirection = new Vector3(Random.Range(-speed, speed), Random.Range(-speed, speed), 0);
        rb.AddForce(randomDirection);
    }
}
