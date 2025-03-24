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
    Image clockImg;
    GameObject dayPassedTextGO;
    TMP_Text dayPassedText;

    private void Awake()
    {
        clockImg = GetComponent<Image>();
        dayPassedTextGO = transform.GetChild(0).gameObject;
        dayPassedText = dayPassedTextGO.GetComponent<TMP_Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        clockImg.fillAmount = 0f;
        dayPassedTextGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (clockImg.fillAmount >= 1) ResetClock();

        clockImg.fillAmount += Time.deltaTime * clockSpeed;
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
        //TODO: Deduct resources, update map progress
        StartCoroutine(DayPassedTextAnim());
    }

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
