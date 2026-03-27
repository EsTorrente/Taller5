using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class UIParallax: MonoBehaviour
{
    public float movementIntensity = 20f;
    public bool invertX = false;
    public bool invertY = false;

    private Vector3 startPos;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float x = (mousePos.x / Screen.width) - 0.5f;
        float y = (mousePos.y / Screen.height) - 0.5f;

        if (invertX) x *= -1;
        if (invertY) y *= -1;

        Vector3 offset = new Vector3(x * movementIntensity, y * movementIntensity, 0);

        rectTransform.anchoredPosition = startPos + offset;
    }
}
