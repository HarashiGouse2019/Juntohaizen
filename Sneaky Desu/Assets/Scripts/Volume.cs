using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{

    public Slider volumeAdjust;
    public AudioSource music;

    // Update is called once per frame
    void Update()
    {
        music.volume = volumeAdjust.value;
    }
}
