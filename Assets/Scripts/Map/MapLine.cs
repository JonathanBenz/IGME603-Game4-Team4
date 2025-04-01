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

    void IncrementKangaroo()
    {

    }

    public float Width { get { return width; } }

    private void Awake()
    {
        this.transform.SetSiblingIndex(3);
        rectTransform = GetComponent<RectTransform>();
        canvasScaler = transform.parent.GetComponent<CanvasScaler>();
    }

    /// <summary>
    /// Sets the length and orientation of the line
    /// </summary>
    /// <param name="length"></param>
    public void SetLine(Vector3 length)
    {
        // Need to convert to screen space and take canvas scale into account
        targetCanvasPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, length);
        Vector2 scaleReference = new Vector2(canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);
        targetCanvasPosition.Scale(scaleReference);

        // IDK WHY the world to screen conversion doesn't work correctly ???
        width = targetCanvasPosition.x - 960f; // Random fudge number to get the horizontal component of the line close to matching.
        rectTransform.right = length;

        // Need to calculate angle and then divide the width by cos in order to get the proper hypotenuse length
        float angle = Vector2.SignedAngle(Vector2.right, (Vector2)length);
        angle *= Mathf.Deg2Rad;
        width /= Mathf.Cos(angle);

        rectTransform.sizeDelta = new Vector2(width, height);
    }
}