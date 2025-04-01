using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Advertisements : MonoBehaviour
{
    [SerializeField] Sprite[] ads;
    Image displayImage;
    Button exitButton;
    Clock clock;
    int adNumber;
    int dayNumber;

    private void Awake()
    {
        displayImage = GetComponentInChildren<Image>();
        displayImage.enabled = false;

        exitButton = GetComponentInChildren<Button>();
        exitButton.gameObject.SetActive(false);

        clock = FindObjectOfType<Clock>();
    }
    private void OnEnable()
    {
        clock.NewDay.AddListener(NewDay);
    }
    private void OnDisable()
    {
        clock.NewDay.RemoveListener(NewDay);
    }
    private void DisplayAd()
    {
        Time.timeScale = 0;
        displayImage.enabled = true;
        exitButton.gameObject.SetActive(true);

        adNumber++;
        adNumber %= ads.Length;

        displayImage.sprite = ads[adNumber];
    }
    public void NewDay()
    {
        dayNumber++;

        // Show ad every 5 days
        if (dayNumber % 5 == 0) DisplayAd();
    }

    public void ExitAd()
    {
        Time.timeScale = 1;
        displayImage.enabled = false;
    }
}
