using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    #region Singleton

    private static AudioController _instance;
    public static AudioController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioController>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(AudioController).Name;
                    _instance = obj.AddComponent<AudioController>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public AudioClip[] audioFiles;
    public AudioSource audioSource;

    public AudioClip[] soundEffects;
    public AudioSource audioSourceEffect;

    void Start()
    {
        audioSource.clip = audioFiles[0];
    }

    public void PlayMusic(int musicToPlay)
    {
        audioSource.clip = audioFiles[musicToPlay];

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PlaySFX(int sfxToPlay)
    {
        audioSourceEffect.clip = soundEffects[sfxToPlay];
        if (!audioSourceEffect.isPlaying)
        {
            audioSourceEffect.Play();
        }
    }
}
