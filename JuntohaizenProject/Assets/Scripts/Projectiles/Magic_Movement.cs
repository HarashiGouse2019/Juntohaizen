using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic_Movement : MonoBehaviour
{

    public GameObject Wielder; //The player game object

    public Pawn pawn;

    public float rotateSpeed = 5f; //Rotations speed
    [HideInInspector] public static float radius = 0.5f; //The radius our magic will be taking
     public float _radius;

    private Vector2 centre; //The center origin point or rotation (which will be the player)

    private float _angleValue = 0; //This will have our angle value

    public static float angle; //This will be the static variable for other scripts to use this.

    public const float defaultRadius = 1f;
    private void Awake()
    {
        angle = _angleValue;
       
        Wielder = GameObject.FindGameObjectWithTag("Player");
        pawn = FindObjectOfType<Player_Pawn>();
    }

    private void Update()
    {
        _radius = radius;
        if (Wielder == null)
        {
            Wielder = FindObjectOfType<Player_Pawn>().gameObject;
        }

        //All this will cause our game object to circle around the player once activated
        centre = Wielder.transform.position;

        angle += rotateSpeed * Time.deltaTime;


        var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * _radius;
        transform.position = centre + offset;


    }
}
