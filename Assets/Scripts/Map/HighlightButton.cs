using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For some reason, default highlight functionality only works before the button is clicked. 
/// Once the button is clicked, it doesn't highlight anymore. Hence, this is why I created this script. 
/// </summary>
public class HighlightButton : MonoBehaviour
{
    Image buttonImg;
    private void Awake()
    {
        buttonImg = GetComponent<Image>();
    }
    public void OnPointerEnter()
    {
        buttonImg.color = new Color(0.9333333f, 0.7529412f, 0.682353f);
    }
    public void OnPointerExit()
    {
        buttonImg.color = new Color(0.7098039f, 0.572549f, 0.5176471f);
    }
}