using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Audio[] getAudio;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Audio a in getAudio)
        {
            a.source = gameObject.AddComponent<AudioSource>();
            a.source.clip = a.clip;

            a.source.volume = a.volume;
            a.source.pitch = a.pitch;
            a.source.loop = a.enableLoop;
        }
    }

    // Update is called once per frame
    public void Play(string name)
    {
        Audio a = Array.Find(getAudio, sound => sound.name == name);
        if (a == null) { Debug.LogWarning("Sound name " + name + " was not found."); return; }
        a.source.Play();
    }
}

