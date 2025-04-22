using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public CharacterStats CharacterStats;
    public PlayerController PlayerController;

    public GameObject quitScreen;
    public GameObject settingsScreen;
    public GameObject notesScreen;
    public GameObject notesTextScreen;
    public GameObject buttons;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            settingsScreen.SetActive(false); //Always close open screens when pressing escape
            notesScreen.SetActive(false);
            notesTextScreen.SetActive(false);
            buttons.SetActive(true);

            quitScreen.SetActive(!quitScreen.activeSelf);
            Pause();
        }
    }

    public void ToggleBoolOnClick() //To toggle the isPaused bool when clicking the cancel button on the pause screen
    {
        isPaused = !isPaused;
    }

    public void Pause()
    {
        if (isPaused)
        {
            Time.timeScale = 0f;

            CharacterStats.enabled = false; //Player cannot move or pickup items while game is paused
            PlayerController.enabled = false;
        }
        else if (!isPaused)
        {
            Time.timeScale = 1f;
            CharacterStats.enabled = true;
            PlayerController.enabled = true;
        }
    }
}