using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicToggle : MonoBehaviour
{
    public Toggle toggleButton;

    private MusicManager musicManager; // Reference to MusicManager instance

    private void Start()
    {
        // Find the MusicManager instance in the scene
        musicManager = FindObjectOfType<MusicManager>();
        if (musicManager == null)
        {
            Debug.LogError("MusicManager not found!");
        }

        // Load the saved toggle state
        toggleButton.isOn = PlayerPrefs.GetInt("MusicToggle", 1) == 1;
        ApplyToggleState();

        // Add listener to handle toggle value change
        toggleButton.onValueChanged.AddListener(delegate { ToggleMusic(); });
    }

    private void ApplyToggleState()
    {
        if (toggleButton.isOn)
        {
            if (SceneManager.GetActiveScene().name == "PlayScreen")
            {
                musicManager.PlayGameSceneMusic();
            }
            else
            {
                musicManager.PlayMainMusic();
            }
        }
        else
        {
            musicManager.StopMusic();
        }
    }

    public void ToggleMusic()
    {
        ApplyToggleState();

        // Save the toggle state
        PlayerPrefs.SetInt("MusicToggle", toggleButton.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
