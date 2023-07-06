using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetSfxValue(float value)
    {
        audioMixer.SetFloat("SFX", value);
    }
    public void SetMusicValue(float value)
    {
        audioMixer.SetFloat("Music", value);
    }
}
