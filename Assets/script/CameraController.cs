using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float padding = 1f;

    public void AdjustCameraSize(int gridWitdh, int gridHeight)
    {
        float aspect = Camera.main.aspect;
        float size =  Mathf.Max(gridWitdh/aspect, gridHeight) / 2f + padding;
        Camera.main.orthographicSize = Mathf.Max(5f,size);

        Vector3 centerGrid = new Vector3((gridWitdh - 1f) / 2f, (gridHeight - 1f) / 2f, -10f);
        float angle = transform.eulerAngles.x * Mathf.Rad2Deg;
        float h = (transform.position - centerGrid).magnitude;
        float deltaY = Mathf.Sin(angle * Mathf.Deg2Rad) * h;
        deltaY = Mathf.Abs(deltaY);
        transform.position = new Vector3(centerGrid.x,centerGrid.y - deltaY , -10f);

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
