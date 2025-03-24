using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MapLine : MonoBehaviour
{
    [SerializeField] float height = 5f;
    float width;
    RectTransform rectTransform;
    Vector2 targetCanvasPosition;
    CanvasScaler canvasScaler;

    public float Width { get { return width; } }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasScaler = transform.parent.GetComponent<CanvasScaler>();
    }

    /// <summary>
    /// Sets the length and orientation of the line
    /// </summary>
    /// <param name="length"></param>
    public void SetLine(Vector3 length)
    {
        targetCanvasPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, length);
        Vector2 scaleReference = new Vector2(canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);
        targetCanvasPosition.Scale(scaleReference);
        width = targetCanvasPosition.x;
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.right = length;
    }
}