using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Pin represents one of the locations which the player can send a team of archaeologists to
/// </summary>
public class Pin : MonoBehaviour
{
    [SerializeField] SOPinLocation locationInfo;
    [SerializeField] GameObject mapLineGO;
    GameObject pinLight;
    GameObject pinMapLine;
    PopUp popUp;
    int difficultyLevel;
    int numOfHits;
    static bool popUpActive; // static so that it applies across all pins
    bool isOccupied;
    Shop shop;
    private ScoutManagement scoutManagement;
    public int DifficultyLevel { get { return difficultyLevel; } }
    public bool PopUpActive { set { popUpActive = value; } }
    public bool IsOccupied { get { return isOccupied; } }

    private void Awake()
    {
        pinLight = transform.GetChild(0).gameObject;
        popUp = FindObjectOfType<PopUp>();
        shop = FindObjectOfType<Shop>();
    }
    private void Start()
    {
        scoutManagement = ScoutManagement.Instance;

        pinLight.SetActive(false);
        difficultyLevel = CalculateRandomDifficulty();
        popUp.gameObject.SetActive(false);
    }

    /// <summary>
    /// When the cursor is hovering over the Pin, increase its scale and turn on its light
    /// </summary>
    public void OnPointerEnter()
    {
        if (isOccupied || popUpActive) return;

        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        pinLight.SetActive(true);
    }

    /// <summary>
    /// When the cursor stops hovering over the Pin, reset its scale and deactivate its light
    /// </summary>
    public void OnPointerExit()
    {
        transform.localScale = Vector3.one;
        pinLight.SetActive(false);
    }

    /// <summary>
    /// When the Pin is clicked, activate and set the Pop Up
    /// </summary>
    public void OnPointerClick()
    {
        if (isOccupied || popUpActive) return;

        popUp.gameObject.SetActive(true);
        popUp.SetupPopup(locationInfo, this);
        popUpActive = true;
    }

    /// <summary>
    /// If the player sends scouts to this pin, set as occupied and draw a line to the pin.
    /// </summary>
    public void SetAsOccupied(int numKangaroosSent, int onions)
    {
        Transform baseTransform = FindObjectOfType<HomeBase>().transform;
        Vector3 basePosition = baseTransform.GetComponent<RectTransform>().position;
        //Vector3 basePosition = baseTransform.TransformPoint(Vector3.zero);

        pinMapLine = Instantiate(mapLineGO, basePosition, Quaternion.identity, baseTransform.parent);
        Vector3 lengthBetween = (Vector3)this.transform.GetComponent<RectTransform>().position - basePosition;
        //Vector3 lengthBetween = this.transform.TransformPoint(Vector3.zero) - basePosition;
        isOccupied = true;
        //shop.kangaroos -= numScoutsSentToPin;
        shop.DecrementKangaroo(numKangaroosSent);

        //scoutManagement.requiredDays = difficultyLevel - numScoutsSent;
        int difficultyMitigation = numKangaroosSent - 1;
        difficultyLevel -= difficultyMitigation;
        // base on the number of onions sent, set the number of hits required to complete the expedition
        if (onions <= 2)
        {
            numOfHits = 1;
        }
        else
        {
            numOfHits = onions - 2;
        }
        scoutManagement.hits = numOfHits;
        if (difficultyLevel <= 1) difficultyLevel = 1;
        scoutManagement.AddOccupiedPin(this, difficultyLevel);

        pinMapLine.GetComponent<MapLine>().SetLine(lengthBetween, difficultyLevel);
    }

    /// <summary>
    /// Once an expedition is complete, reset the pin to be used again.
    /// </summary>
    public void Reset()
    {
        isOccupied = false;
        difficultyLevel = CalculateRandomDifficulty();
        Destroy(pinMapLine);
    }

    private int CalculateRandomDifficulty()
    {
        return Random.Range(1, 6);
    }
}
