using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickSFX;
    [Header("Background Music")]
    [SerializeField] private AudioClip titleScreenBackground;
    [SerializeField] private AudioClip endingBackground;

    public static AudioManager Instance { get; private set; }

    public AudioSource MusicSource;
    public AudioSource SFXSource;

    void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayBackgroundSong(string song)
    {
        switch (song)
        {
            case "title":
                MusicSource.clip = titleScreenBackground;
                break;
            case "end":
                MusicSource.clip = endingBackground;
                MusicSource.time = 5f;
                break;
        }
        MusicSource.Play();
    }

    public void SetBackgroundMusicVolume(float value)
    {
        MusicSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        SFXSource.volume = value;
    }

    public void StopBackgroundSong()
    {
        if(MusicSource)
        {
            MusicSource.Stop();
        }
    }

    public void ClickSoundEffect()
    {
        PlaySFX(clickSFX, 1);
    }

    public void PlaySFX(AudioClip soundEffect, float relativeVolume)
    {
        SFXSource.PlayOneShot(soundEffect, relativeVolume);
    }
}