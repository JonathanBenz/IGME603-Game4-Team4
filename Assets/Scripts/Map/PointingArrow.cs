using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Pointing Arrow will point from the Home Base to the player's mouse cursor every frame
/// </summary>
public class PointingArrow : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
