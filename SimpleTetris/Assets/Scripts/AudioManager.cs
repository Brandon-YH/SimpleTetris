using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;

    void Awake()
    {
        // Checks for audio instance to prevent duplicate sound tracks
        if (instance == null)
            instance= this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Prevent restarting of main theme whenever screen switches
        DontDestroyOnLoad(gameObject);

        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

    }

    public void Play (string name)
    {
        Sound s = sounds.Where(s=> s.name == name).FirstOrDefault();
        if (s == null) {
            Debug.LogWarning("**WARNING** Sound file not found: " + name);
            return;
        }
        s.source.Play();
    }
}
