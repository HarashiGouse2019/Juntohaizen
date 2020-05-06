using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioMixerGroup musicMixer;

    [System.Serializable]
    public class Music
    {
        public string name; // Name of the audio

        public AudioClip clip; //The Audio Clip Reference

        [Range(0f, 1f)]
        public float volume; //Adjust Volume

        [Range(.1f, 3f)]
        public float pitch; //Adject pitch

        public bool enableLoop; //If the audio can repeat

        [HideInInspector] public AudioSource source;
    }

    public Slider musicVolumeAdjust; 

    public Music[] getMusic;

    // Start is called before the first frame update
    public float timeSamples;

    public float[] positionSeconds;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            foreach (Music music in getMusic)
            {
                music.source = gameObject.AddComponent<AudioSource>();
                music.source.clip = music.clip;

                music.source.volume = music.volume;
                music.source.pitch = music.pitch;
                music.source.loop = music.enableLoop;
                music.source.outputAudioMixerGroup = musicMixer;
            }

            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdateVolume();
    }

    public void Play(string _name, float _volume = 100)
    {
        Music a = Array.Find(getMusic, sound => sound.name == _name);
        if (a == null)
        {
            Debug.LogWarning("Sound name " + _name + " was not found.");
            return;
        }
        else
        {
            a.source.Play();
            a.source.volume = _volume / 100;
        }
    }
    public void Stop(string _name)
    {
        Music a = Array.Find(getMusic, sound => sound.name == _name);
        if (a == null)
        {
            Debug.LogWarning("Sound name " + _name + " was not found.");
            return;
        }
        else
        {
            a.source.Stop();
        }
    }

    public AudioClip GetMusic(string _name)
    {
        Music a = Array.Find(getMusic, sound => sound.name == _name);
        if (a == null)
        {
            Debug.LogWarning("Sound name " + _name + " was not found.");
            return null;
        }
        else
        {
            return a.clip;
        }
    }

    public void MusicVolume(float value)
    {
        musicMixer.audioMixer.SetFloat("MusicVolume", value);
    }

    void UpdateVolume()
    {
        //Changing Volume
        if (musicVolumeAdjust == null)
        {
            musicVolumeAdjust = FindObjectOfType<Slider>();
        }
        try
        {
            MusicVolume(musicVolumeAdjust.value);
        }
        catch
        {
            return;
        }
    }
}
