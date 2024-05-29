using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioSource mainMusicTheme;
    public AudioSource gameSceneTheme;

    private static MusicManager instance;


    public bool IsMusicPlaying()
    {
        return mainMusicTheme.isPlaying || gameSceneTheme.isPlaying;
    }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayMainMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScreen")
        {
            PlayGameSceneMusic();
        }
        else
        {
            PlayMainMusic();
        }
    }

    public void PlayMainMusic()
    {
        if (!mainMusicTheme.isPlaying)
        {
            mainMusicTheme.Play();
        }
        if (gameSceneTheme.isPlaying)
        {
            gameSceneTheme.Stop();
        }
    }

    public void PlayGameSceneMusic()
    {
        if (!gameSceneTheme.isPlaying)
        {
            gameSceneTheme.Play();
        }
        if (mainMusicTheme.isPlaying)
        {
            mainMusicTheme.Stop();
        }
    }
    public void StopMusic()
    {
        mainMusicTheme.Stop();
        gameSceneTheme.Stop();
    }
}