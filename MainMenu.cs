using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("SongSelect");
    }

    public void ToSettingsMenu()
    {
        SceneManager.LoadScene("SettingsMenu");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ToLevel1()
    {
        SceneManager.LoadScene("Song1Scene");
    }

    public void ToLevel2()
    {
        SceneManager.LoadScene("Song2Scene");
    }

    public void ToCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
