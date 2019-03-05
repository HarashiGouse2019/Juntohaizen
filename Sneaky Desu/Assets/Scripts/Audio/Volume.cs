using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Volume : MonoBehaviour
{

    public Slider volumeAdjust; //Reference to our volume slider in the options menu
    public AudioSource music; //The audio source we're referencing

    // Update is called once per frame
    void Update()
    {
        music.volume = volumeAdjust.value; //Chaning the volume of our audio based on our slider fill value.
    }
}
