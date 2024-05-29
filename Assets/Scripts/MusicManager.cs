using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioSource mainMusicTheme;
    public AudioSource gameSceneTheme;

    private static MusicManager instance;

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
        ApplySavedMusicState();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySavedMusicState();
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
        if (mainMusicTheme.isPlaying)
        {
            mainMusicTheme.Stop();
        }
        if (gameSceneTheme.isPlaying)
        {
            gameSceneTheme.Stop();
        }
    }

    private void ApplySavedMusicState()
    {
        if (PlayerPrefs.GetInt("MusicToggle", 1) == 1)
        {
            if (SceneManager.GetActiveScene().name == "PlayScreen")
            {
                PlayGameSceneMusic();
            }
            else
            {
                PlayMainMusic();
            }
        }
        else
        {
            StopMusic();
        }
    }
}