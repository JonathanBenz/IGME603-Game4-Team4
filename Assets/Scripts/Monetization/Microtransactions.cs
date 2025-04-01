using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Microtransactions : MonoBehaviour
{
    bool isDisablingAds;

    private void OnEnable()
    {
        Time.timeScale = 0;
    }
   
    public void IsDisablingAds(bool bValue)
    {
        isDisablingAds = bValue;
    }

    public void DisableAds()
    {
        if (isDisablingAds)
        {
            FindObjectOfType<Advertisements>().gameObject.SetActive(false);
        }
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }
}
