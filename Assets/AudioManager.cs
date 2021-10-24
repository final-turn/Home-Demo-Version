using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip titleScreenBackground;
    [SerializeField] private AudioClip gameBackground;
    [SerializeField] private AudioClip endingBackground;

    public static AudioManager Instance { get; private set; }
    private AudioSource a_source;

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
        a_source = GetComponent<AudioSource>();
    }

    public void PlayBackgroundSong(string song)
    {
        switch (song)
        {
            case "title":
                a_source.clip = titleScreenBackground;
                break;
            case "game":
                a_source.clip = gameBackground;
                break;
            case "end":
                a_source.clip = endingBackground;
                break;
        }
        a_source.Play();
    }

    public void StopBackgroundSong()
    {
        a_source.Stop();
    }
}