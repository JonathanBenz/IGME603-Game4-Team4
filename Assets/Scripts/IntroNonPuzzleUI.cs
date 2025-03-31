using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroNonPuzzleUI : MonoBehaviour
{

    public GameObject helpPanel;
    public GameObject WelcomePanel;

    public void SkipIntro()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel.activeSelf)
        {
            helpPanel.SetActive(false);
            WelcomePanel.SetActive(true);
            return;
        }
        else helpPanel.SetActive(true);
    }

}
