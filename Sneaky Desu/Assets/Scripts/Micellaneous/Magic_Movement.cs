using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic_Movement : MonoBehaviour
{

    public GameObject Player;
    public float rotateSpeed = 5f;
    public float radius = 0.1f;

    private Vector2 centre;

    private float _angleValue = 0;

    public static float angle;

    private void Awake()
    {
        angle = _angleValue;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        centre = Player.transform.position;

        angle += rotateSpeed * Time.deltaTime;

        var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
        transform.position = centre + offset;
    }
}
