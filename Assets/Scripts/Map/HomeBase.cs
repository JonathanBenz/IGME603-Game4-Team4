using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Home Base represents the player's base of operations. It brings the player back to the default canvas.
/// </summary>
public class HomeBase : MonoBehaviour
{
    GameObject baseLight;
    private void Awake()
    {
        baseLight = transform.GetChild(0).gameObject;
    }
    private void Start()
    {
        baseLight.SetActive(false);
    }

    /// <summary>
    /// When the cursor is hovering over the Home Base, increase its scale and turn on its light
    /// </summary>
    public void OnPointerEnter()
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        baseLight.SetActive(true);
    }

    /// <summary>
    /// When the cursor stops hovering over the Home Base, reset its scale and deactivate its light
    /// </summary>
    public void OnPointerExit()
    {
        transform.localScale = Vector3.one;
        baseLight.SetActive(false);
    }

    /// <summary>
    /// When the Home Base is clicked, disable the Map Canvas
    /// </summary>
    public void OnPointerClick()
    {
        this.transform.parent.gameObject.GetComponent<Canvas>().enabled = false;
    }
}
