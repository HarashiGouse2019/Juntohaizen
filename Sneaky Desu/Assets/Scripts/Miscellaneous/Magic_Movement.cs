﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic_Movement : MonoBehaviour
{

    public GameObject Player; //The player game object
    public float rotateSpeed = 5f; //Rotations speed
    public float radius = 0.1f; //The radius our magic will be taking

    private Vector2 centre; //The center origin point or rotation (which will be the player)

    private float _angleValue = 0; //This will have our angle value

    public static float angle; //This will be the static variable for other scripts to use this.

    private void Awake()
    {
        angle = _angleValue;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Player == null)
        {
            Player = FindObjectOfType<Player_Pawn>().gameObject;
        }
        //All this will cause our game object to circle around the player once activated
        centre = Player.transform.position;

        angle += rotateSpeed * Time.deltaTime;

        var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
        transform.position = centre + offset;
    }
}