using System.Collections;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    //The Targeted GameObject to manipulate its position through it's Transform Component
    public Transform target;

    //Used to set the duration of the camera smoothing out and in towards the player
    public float smoothOutDuration = 0.125f;

    //Setting the offset of the camera
    public Vector3 offset;

    void FixedUpdate()
    {                                                                                               /********************************************************/
        Vector3 setCoordinate = target.position + offset;                                           //We create a Vector3 that will grab the Player's       *
                                                                                                    //position and add it to the camera offset              *
        Vector3 smoothPosition = Vector3.Lerp(transform.position, setCoordinate, smoothOutDuration);//We'll create another Vector3 that will go from        *
                                                                                                    //it's current spot to the player in a smooth motion    *
                                                                                                    //with a set amount of time.                            *
        transform.position = smoothPosition;                                                        //The smoothPosition variable will then be added to     *
                                                                                                    //the Camera's Transforma Component, applying the change*
                                                                                                    //in position every frame assuring that it smoothes out.*
                                                                                                    /********************************************************/
    }
}

