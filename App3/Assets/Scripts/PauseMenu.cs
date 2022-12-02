using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseUI;
    public GameObject helpUI;
    public GameObject traitsRacesUI;

    public GameObject grid;
    public GameObject units;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        units.SetActive(true);
        grid.SetActive(true);
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        units.SetActive(false);
        grid.SetActive(false);
    }

    public void Help()
    {
        helpUI.SetActive(true);
    }

    public void ReturnToPause()
    {
        helpUI.SetActive(false);
    }

    public void TraitsRaces()
    {
        traitsRacesUI.SetActive(true);
    }

    public void ReturnToHelp()
    {
        traitsRacesUI.SetActive(false);
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
