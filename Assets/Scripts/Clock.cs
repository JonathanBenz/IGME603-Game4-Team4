using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script controls the clock UI element and is responsible for handling when a day passes.
/// </summary>
public class Clock : MonoBehaviour
{
    [SerializeField] float clockSpeed = 0.1f;
    [SerializeField] int amountToDeduct = 10;
    Image clockImg;
    GameObject dayPassedTextGO;
    TMP_Text dayPassedText;
    Shop shop;

    public bool canStart;
    [SerializeField] bool isMainClock;
    Clock mainClock;
    Image mainClockImage;

    private void OnDisable()
    {
        dayPassedTextGO.SetActive(false);
    }
    private void Awake()
    {
        clockImg = GetComponent<Image>();
        dayPassedTextGO = transform.GetChild(0).gameObject;
        dayPassedText = dayPassedTextGO.GetComponent<TMP_Text>();
        shop = FindObjectOfType<Shop>();
    }
    // Start is called before the first frame update
    void Start()
    {
        clockImg.fillAmount = 0f;
        dayPassedTextGO.SetActive(false);

        // If this instance of the clock is not the main one, retrieve a reference to the main clock
        if (!isMainClock)
        {
            Clock[] clocks = FindObjectsOfType<Clock>();
            foreach(Clock clock in clocks)
            {
                // The clock that is not this one must be the main one
                if (clock != this) { mainClock = clock; break; }
            }
            mainClockImage = mainClock.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canStart) return; // WAIT until player is done reading tutorial !!
        if (clockImg.fillAmount >= 1) ResetClock();

        // Synchronize the World clock to the Main clock
        if (isMainClock) clockImg.fillAmount += Time.deltaTime * clockSpeed;
        else if (!isMainClock) clockImg.fillAmount = mainClockImage.fillAmount;
    }

    /// <summary>
    /// This method is called when fill amount reaches maximum value, and then calls that a day has passed.
    /// </summary>
    void ResetClock()
    {
        clockImg.fillAmount = 0f;
        DayHasPassed();
    }

    /// <summary>
    /// This method is responsible for handling what happens when a day has passed.
    /// </summary>
    public void DayHasPassed()
    {
        //TODO: update map progress
        StartCoroutine(DayPassedTextAnim());

        // Prevent any clock from calling this function unless it is the Main Clock!!
        if (!isMainClock) return;
        shop.DeductMoney(amountToDeduct);
    }

    /// <summary>
    /// When the player exits the tutorial, this event is called to start the timers
    /// </summary>
    public void StartClock()
    {
        mainClock.canStart = true;
        this.canStart = true;
    }

    /// <summary>
    /// Scale up, move up, fade away text
    /// </summary>
    IEnumerator DayPassedTextAnim()
    {
        dayPassedTextGO.SetActive(true);
        dayPassedTextGO.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        dayPassedTextGO.transform.localPosition = Vector3.zero;

        // This would take 1 seconds
        for (float ft = 1f; ft >= 0; ft -= 0.01f)
        {
            dayPassedTextGO.transform.localScale *= 1.01f;
            dayPassedTextGO.transform.localPosition += new Vector3(0f, 1f, 0f);
            dayPassedText.alpha = ft;
            yield return new WaitForSeconds(.01f);
        }

        dayPassedTextGO.SetActive(false);
    }
}
