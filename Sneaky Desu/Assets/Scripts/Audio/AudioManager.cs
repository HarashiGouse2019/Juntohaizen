using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;

    public AudioMixerGroup audioMixer;

    public Audio[] getAudio;
    public Slider soundVolumeAdjust;  //Reference to our volume slider in the options menu

    // Start is called before the first frame update
    void Awake()
    {
        if (audioManager == null)
        {
            audioManager = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(gameObject);
        }

        foreach (Audio audio in getAudio)
        {
            audio.source = gameObject.AddComponent<AudioSource>();
            audio.source.clip = audio.clip;

            audio.source.volume = audio.volume;
            audio.source.pitch = audio.pitch;
            audio.source.loop = audio.enableLoop;

            audio.source.outputAudioMixerGroup = audioMixer;
        }
    }

    private void Update()
    {
        UpdateVolume();
    }

    // Update is called once per frame
    public void Play(string name)
    {
        Audio a = Array.Find(getAudio, sound => sound.name == name);
        if (a == null) { Debug.LogWarning("Sound name " + name + " was not found."); return; }
        if (a != null)
            a.source.Play();
    }

    public void Stop(string name)
    {
        Audio a = Array.Find(getAudio, sound => sound.name == name);
        if (a == null) { Debug.LogWarning("Sound name " + name + " was not found."); return; }
        if (a != null)
            a.source.Stop();
    }

    public void AudioVolume(float value)
    {
        audioMixer.audioMixer.SetFloat("SoundVolume", value);
    }

    void UpdateVolume()
    {
        //Changing Volume
        if (soundVolumeAdjust == null)
        {
            soundVolumeAdjust = FindObjectOfType<Slider>();
        }
        try
        {
            AudioVolume(soundVolumeAdjust.value);
        }
        catch
        {
            return;
        }
    }
}

