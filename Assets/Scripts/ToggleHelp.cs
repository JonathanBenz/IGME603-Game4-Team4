using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHelp : MonoBehaviour
{
    public GameObject helpPanel;

    public void ToggleHelpPanel()
    {
        if (helpPanel.activeSelf)
        {
            helpPanel.SetActive(false);
            return;
        }
        else helpPanel.SetActive(true);
    }
}
