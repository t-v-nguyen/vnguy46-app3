using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject helpPage;
    public GameObject bonusInfo;

    public void Start()
    {
        mainMenu.SetActive(true);
        helpPage.SetActive(false);
        bonusInfo.SetActive(false);
    }

    public void StartGame()
    {
        //Load Scene
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Help()
    {
        mainMenu.SetActive(false);
        helpPage.SetActive(true);
        bonusInfo.SetActive(false);
    }

    public void Menu()
    {
        mainMenu.SetActive(true);
        helpPage.SetActive(false);
        bonusInfo.SetActive(false);
    }

    public void TraitsRacesInfo()
    {
        helpPage.SetActive(false);
        bonusInfo.SetActive(true);
    }
}
