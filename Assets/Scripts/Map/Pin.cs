using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Pin represents one of the locations which the player can send a team of archaeologists to
/// </summary>
public class Pin : MonoBehaviour
{
    [SerializeField] GameObject mapLineGO;
    GameObject pinLight;
    bool isOccupied;
    
    private void Awake()
    {
        pinLight = transform.GetChild(0).gameObject;
    }
    private void Start()
    {
        pinLight.SetActive(false);
    }

    /// <summary>
    /// When the cursor is hovering over the Pin, increase its scale and turn on its light
    /// </summary>
    public void OnPointerEnter()
    {
        if (isOccupied) return;

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
    /// When the Pin is clicked, draw a Map Line
    /// </summary>
    public void OnPointerClick()
    {
        if (isOccupied) return;

        Transform baseTransform = FindObjectOfType<HomeBase>().transform;
        Vector3 basePosition = baseTransform.GetComponent<RectTransform>().position;
        //Vector3 basePosition = baseTransform.TransformPoint(Vector3.zero);

        GameObject line = Instantiate(mapLineGO, basePosition, Quaternion.identity, baseTransform.parent);
        Vector3 lengthBetween = (Vector3)this.transform.GetComponent<RectTransform>().position - basePosition;
        //Vector3 lengthBetween = this.transform.TransformPoint(Vector3.zero) - basePosition;
        line.GetComponent<MapLine>().SetLine(lengthBetween);
        isOccupied = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.GetComponent<RectTransform>().position, FindObjectOfType<HomeBase>().transform.GetComponent<RectTransform>().position);
    }
}
