using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disperse : MonoBehaviour
{
    private void OnEnable()
    {
        float speed = 2f;
        Vector3 randomDirection = new Vector3(0, 0, Random.Range(0f, 360f));
        transform.Rotate(randomDirection);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.right * speed * randomDirection;
    }
}
