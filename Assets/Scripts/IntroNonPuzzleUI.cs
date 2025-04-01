using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class IntroNonPuzzleUI : MonoBehaviour
{

    public GameObject helpPanel;
    public GameObject WelcomePanel;
    public GameObject congratulationsPanel;
    public GameObject FeedbackText;

    

    public void SkipIntro()
    {
        SceneManager.LoadScene("Main Scene");
    }

    void Update()
    {
        CheckCodeComplete();
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

    private void CheckCodeComplete()
    {
        if (FeedbackText.GetComponent<TMP_Text>().text == "All letters discovered! Cipher complete.")
        {
            congratulationsPanel.SetActive(true);
        }
    }
}
