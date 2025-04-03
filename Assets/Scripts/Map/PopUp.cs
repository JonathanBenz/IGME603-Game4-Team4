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
    int difficulty; // temp difficulty to be used to dynamically display new difficulty levels upon the player potentially sending or removing kangaroos
    int startingDifficulty; // Hold reference to the initial difficulty

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
        startingDifficulty = pin.DifficultyLevel;
        difficulty = startingDifficulty;
        difficultyText.text = "Difficulty: " + difficulty.ToString();
        SetDifficultyColor();
        numKangaroosText.text = "0";
        kangaroos = 0;
        numKangaroosText.color = Color.red;
        numBloominOnionsText.text = "0";
        bloominOnions = 0;
        numBloominOnionsText.color = Color.red;
        lastActivePin = pin;
    }

    /// <summary>
    /// Set difficulty text color as green if 1-2, yellow for 3-4, red for 5
    /// </summary>
    void SetDifficultyColor()
    {
        // Make sure difficulty never goes past its starting amount or below 1
        if (difficulty <= 1) difficulty = 1;
        if (difficulty >= startingDifficulty) difficulty = startingDifficulty;

        // Change color based on projected difficulty
        if (difficulty <= 2) difficultyText.color = Color.green;
        else if (difficulty <= 4) difficultyText.color = Color.yellow;
        else difficultyText.color = Color.red;

        difficultyText.text = "Difficulty: " + difficulty.ToString();
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
        int onions = bloominOnions;
        shop.DecrementOnion(bloominOnions);
        lastActivePin.SetAsOccupied(kangaroos, onions);
        ExitPopUp();
    }

    // Increases or Decreases kangaroo number (to be used on Button Press)
    public void IncreaseOrDecreaseKangarooNumber(bool isIncreasing)
    {
        if (isIncreasing)
        {
            if (kangaroos >= shop.kangaroos) return;
            if (difficulty <= 1 && kangaroos > 0) return; // Don't let the player waste kangaroos if the pop-up is at lowest difficulty
            if (kangaroos >= 1) difficulty--;

            if (shop.kangaroos >= 1)
            {
                kangaroos++;
            }
            numKangaroosText.color = Color.green;
        }
        else
        {
            if (kangaroos > 0) difficulty++;

            kangaroos--;
            if (kangaroos <= 0)
            {
                kangaroos = 0;
                numKangaroosText.color = Color.red;
            }

        }
        numKangaroosText.text = kangaroos.ToString();
        SetDifficultyColor();
    }

    // Increases or Decreases onion number (to be used on Button Press)
    public void IncreaseOrDecreaseOnionNumber(bool isIncreasing)
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
