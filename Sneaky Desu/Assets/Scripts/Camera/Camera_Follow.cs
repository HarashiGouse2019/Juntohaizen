using System.Collections;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    public static Camera_Follow camerafollow;

    //The Targeted GameObject to manipulate its position through it's Transform Component
    public GameObject target;
    public GameObject crossHair;

    //Used to set the duration of the camera smoothing out and in towards the player
    public float smoothOutDuration = 0.125f;

    //Setting the offset of the camera
    public Vector3 offset;

    void Start()
    {
        DontDestroyOnLoad(this);
        target = FindObjectOfType<Player_Pawn>().gameObject;
    }

    void FixedUpdate()
    {
        Vector3 setCoordinate;
        Vector3 smoothPosition;
        if (Player_Controller.player_controller.toggleLock == false)
        {  setCoordinate = target.transform.position + offset;
            smoothPosition = Vector3.Lerp(transform.position, setCoordinate, smoothOutDuration);
            transform.position = smoothPosition;
        } else
        {
            Vector3 combinedView = target.transform.position + crossHair.transform.position;
            setCoordinate = combinedView + offset;
            smoothPosition = Vector3.Lerp(transform.position, setCoordinate/2, smoothOutDuration);
            transform.position = smoothPosition;
        }
                                                            
                                                                                          
                                                                                          
    }

    public void InitiateLockOn(Transform target)
    {
        gameObject.transform.LookAt(target.position);
    }
}

