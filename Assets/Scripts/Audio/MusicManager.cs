using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip music;

    public static MusicManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    public void ChangeMusic(AudioClip newMusic)
    {
        if (musicSource.clip != newMusic)
        {
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }
}
