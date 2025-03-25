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
    [SerializeField] TextMeshProUGUI numScoutsText;
    [SerializeField] TextMeshProUGUI numFoodText;
    [SerializeField] TextMeshProUGUI numShovelsText;
    [SerializeField] TextMeshProUGUI numFlashlightsText;

    public bool cursorOutOfBounds;
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
        numScoutsText.text = "0";
        numScoutsText.color = Color.red;
        numFoodText.text = "0";
        numFoodText.color = Color.red;
        numShovelsText.text = "0";
        numShovelsText.color = Color.red;
        numFlashlightsText.text = "0";
        numFlashlightsText.color = Color.red;
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
}
