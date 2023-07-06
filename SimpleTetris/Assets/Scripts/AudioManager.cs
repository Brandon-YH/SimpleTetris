using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public Sound[] sounds;

    void Awake()
    {
        // Checks for audio instance to prevent duplicate sound tracks
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Prevent restarting of main theme whenever screen switches
        DontDestroyOnLoad(gameObject);

        foreach (var s in sounds)
        {
            if (s.name == "MainTheme")
                s.source = musicSource;
            else
                s.source = sfxSource;
        }
    }

    private void Start()
    {
        Play("MainTheme");
    }

    public void Play (string name)
    {
        Sound s = sounds.Where(s=> s.name == name).FirstOrDefault();
        if (s == null) {
            Debug.LogWarning("**WARNING** Sound file not found: " + name);
            return;
        }

        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        s.source.Play();
    }
}
