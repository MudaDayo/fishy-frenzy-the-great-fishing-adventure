using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        // Set the initial state of the toggle button based on the music playing status
        toggleButton.isOn = musicManager.IsMusicPlaying();
    }

    public void ToggleMusic()
    {
        Debug.Log("ToggleMusic called");
        if (toggleButton.isOn)
        {
            Debug.Log("Music is toggled on");
            musicManager.PlayMainMusic(); // Play the music when the toggle is on
        }
        else
        {
            Debug.Log("Music is toggled off");
            musicManager.StopMusic(); // Stop the music when the toggle is off
        }
    }
}
