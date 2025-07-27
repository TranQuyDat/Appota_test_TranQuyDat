using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFitSafeArea : MonoBehaviour
{
    RectTransform _UI;
    Rect _lastSafeArea;
    ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;
    private void Awake()
    {
        _UI = GetComponent<RectTransform>();
        ApplySafeArea();
    }
    void Update()
    {
        if (Screen.orientation == _lastOrientation && _lastSafeArea == Screen.safeArea) return;
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        _lastSafeArea = safeArea;
        _lastOrientation = Screen.orientation;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _UI.anchorMin = anchorMin;
        _UI.anchorMax = anchorMax;

    }
}