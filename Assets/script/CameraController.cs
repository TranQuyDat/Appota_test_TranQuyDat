using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float padding = 1f;

    public void AdjustCameraSize(int gridWidth, int gridHeight)
    {
        float aspect = Camera.main.aspect;
        float size = Mathf.Max(gridWidth / aspect, gridHeight) / 2f + padding;
        Camera.main.orthographicSize = Mathf.Max(5f, size);

        Vector3 centerGrid = new Vector3((gridWidth - 1f) / 2f, (gridHeight - 1f) / 2f, 0f);

        Vector3 forward = transform.forward.normalized;
        float distance = 10f;
        transform.position = centerGrid - forward * distance;

    }
    public bool showGizmos = false;
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Rect b = ScreenInfo.ViewportRectOnXY;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}
