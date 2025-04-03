using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MapLine : MonoBehaviour
{
    [SerializeField] float height = 5f;
    float width;
    float clockSpeed;
    float startPos;
    RectTransform rectTransform;
    Vector2 targetCanvasPosition;
    CanvasScaler canvasScaler;

    [SerializeField] GameObject kangarooImagePrefab;
    GameObject kangarooInstance;
    int daysToTravel;
    bool returnToBase;

    private void Update()
    {
        if (returnToBase) kangarooInstance.transform.localPosition -= new Vector3(2 * (width / daysToTravel) * clockSpeed * Time.deltaTime, 0f, 0f);
        else kangarooInstance.transform.localPosition += new Vector3(2 * (width / daysToTravel) * clockSpeed * Time.deltaTime, 0f, 0f);

        if (kangarooInstance.transform.localPosition.x >= width && !returnToBase)
        {
            returnToBase = true;
            kangarooInstance.transform.right *= -1;
            FlipSprite();
        }
    }

    public float Width { get { return width; } }

    private void Awake()
    {
        this.transform.SetSiblingIndex(3);
        rectTransform = GetComponent<RectTransform>();
        canvasScaler = transform.parent.GetComponent<CanvasScaler>();

        Clock[] clocks = FindObjectsOfType<Clock>();
        foreach (Clock c in clocks)
        {
            if (c.IsMain) { clockSpeed = c.ClockSpeed; startPos = c.GetComponent<Image>().fillAmount; print("StartPos: " + startPos); }
        }
    }

    /// <summary>
    /// Sets the length and orientation of the line
    /// </summary>
    /// <param name="length"></param>
    public void SetLine(Vector3 length, int difficultyReceived)
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

        kangarooInstance = Instantiate(kangarooImagePrefab, this.transform.position, Quaternion.identity, this.transform);
        kangarooInstance.transform.right = length.normalized;

        // Flip the sprite if going to the left
        if (length.x < 0) FlipSprite();

        daysToTravel = difficultyReceived;
        if (startPos > daysToTravel / 2f)
        {
            returnToBase = true;
            kangarooInstance.transform.localPosition = new Vector3(2 * width * (1-startPos), 0f, 0f);
            kangarooInstance.transform.right *= -1;
            FlipSprite();
        }
        else kangarooInstance.transform.localPosition = new Vector3(2 * (width / daysToTravel) * startPos, 0f, 0f);
    }

    void FlipSprite()
    {
        kangarooInstance.transform.localScale = new Vector3(kangarooInstance.transform.localScale.x, -kangarooInstance.transform.localScale.y, kangarooInstance.transform.localScale.z);
    }
}