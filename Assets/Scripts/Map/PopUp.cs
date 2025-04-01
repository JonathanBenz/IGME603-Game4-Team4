using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script is responsible for handling the UI for the Pin Pop Up
/// </summary>
public class PopUp : MonoBehaviour
{
    [SerializeField] Image locationImg;
    [SerializeField] TextMeshProUGUI locationNameText;
    [SerializeField] TextMeshProUGUI blurbText;
    [SerializeField] TextMeshProUGUI difficultyText;
    [SerializeField] TextMeshProUGUI numKangaroosText;
    [SerializeField] TextMeshProUGUI numBloominOnionsText;

    int kangaroos;
    int bloominOnions;

    bool cursorOutOfBounds;
    Shop shop;
    Pin lastActivePin;

    private void Awake()
    {
        shop = FindObjectOfType<Shop>();
        lastActivePin = FindObjectOfType<Pin>(); // load in first pin as default
    }

    private void Update()
    {
        // If mouse is clicked outside of the bounds of the Pop Up, exit the Pop Up.
        if (cursorOutOfBounds && Input.GetMouseButtonDown(0)) ExitPopUp();
    }

    /// <summary>
    /// The Pin should call this method before setting the Pop Up as active
    /// </summary>
    /// <param name="locationInfo"></param>
    public void SetupPopup(SOPinLocation locationInfo, Pin pin)
    {
        locationImg.sprite = locationInfo.image;
        locationNameText.text = locationInfo.locationName;
        blurbText.text = locationInfo.blurb;
        difficultyText.text = "Difficulty: " + pin.DifficultyLevel.ToString();
        numKangaroosText.text = "0";
        kangaroos = 0;
        numKangaroosText.color = Color.red;
        numBloominOnionsText.text = "0";
        bloominOnions = 0;
        numBloominOnionsText.color = Color.red;
        lastActivePin = pin;
    }

    public void OnPointerEnter()
    {
        cursorOutOfBounds = false;
    }
    public void OnPointerExit()
    {
        cursorOutOfBounds = true;
    }
    private void ExitPopUp()
    {
        lastActivePin.PopUpActive = false;
        this.gameObject.SetActive(false);
    }
    public void SendTeamButtonPressed()
    {
        if (kangaroos == 0) { ExitPopUp(); return; }
        shop.DecrementOnion(bloominOnions);
        lastActivePin.SetAsOccupied(kangaroos);
        ExitPopUp();
    }

    // Increases or Decreases scout number (to be used on Button Press)
    public void IncreaseOrDecreaseScoutNumber(bool isIncreasing)
    {
        if (isIncreasing)
        {
            if (kangaroos >= shop.kangaroos) return;
            if (shop.kangaroos >= 1) kangaroos++;
            numKangaroosText.color = Color.green;
        }
        else
        {
            kangaroos--;
            if (kangaroos <= 0)
            {
                kangaroos = 0;
                numKangaroosText.color = Color.red;
            }
        }
        numKangaroosText.text = kangaroos.ToString();
    }

    // Increases or Decreases shovel number (to be used on Button Press)
    public void IncreaseOrDecreaseShovelNumber(bool isIncreasing)
    {
        if (isIncreasing)
        {
            if (bloominOnions >= shop.bloominOnions) return;
            if (shop.bloominOnions >= 1) bloominOnions++;
            numBloominOnionsText.color = Color.green;
        }
        else
        {
            bloominOnions--;
            if (bloominOnions <= 0)
            {
                bloominOnions = 0;
                numBloominOnionsText.color = Color.red;
            }
        }
        numBloominOnionsText.text = bloominOnions.ToString();
    }
}
