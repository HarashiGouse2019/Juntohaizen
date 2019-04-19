using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MasterSounds
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager audioManager;

        public Audio[] getAudio;
        public Slider volumeAdjust; //Reference to our volume slider in the options menu


        // Start is called before the first frame update
        void Awake()
        {
            audioManager = this;
            foreach (Audio a in getAudio)
            {
                a.source = gameObject.AddComponent<AudioSource>();
                a.source.clip = a.clip;

                a.source.volume = a.volume;
                a.source.pitch = a.pitch;
                a.source.loop = a.enableLoop;
            }
        }

        private void Update()
        {
            try
            {
                MusicVolume(volumeAdjust.value);
            } catch
            {
                return;
            }
        }

        // Update is called once per frame
        public void Play(string name)
        {
            Audio a = Array.Find(getAudio, sound => sound.name == name);
            if (a == null) { Debug.LogWarning("Sound name " + name + " was not found."); return; }
            a.source.Play();
        }

        public void Stop(string name)
        {
            Audio a = Array.Find(getAudio, sound => sound.name == name);
            if (a == null) { Debug.LogWarning("Sound name " + name + " was not found."); return; }
            a.source.Stop();
        }

        public void MusicVolume(float value)
        {
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i].volume = value;
            }

        }
    }
}

