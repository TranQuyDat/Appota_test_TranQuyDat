using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInfo
{
    public static float Height => Camera.main.orthographicSize * 2f;
    public static float Width => Height * Camera.main.aspect;
    public static float Depth => Camera.main.farClipPlane - Camera.main.nearClipPlane;
    public static Vector3 Center => Camera.main.transform.position;

    public static Rect ViewportRectOnXY
    {
        get
        {
            var c = Camera.main;
            if (!c) return Rect.zero;
            var p = new Plane(Vector3.forward, 0);
            var v = c.rect;
            Vector2 min = Vector2.positiveInfinity, max = Vector2.negativeInfinity;
            foreach (var corner in new[] {
            new Vector3(v.xMin,v.yMin), new Vector3(v.xMin,v.yMax),
            new Vector3(v.xMax,v.yMin), new Vector3(v.xMax,v.yMax)})
            {
                var r = c.ViewportPointToRay(corner);
                if (p.Raycast(r, out var e))
                {
                    var pt = r.GetPoint(e);
                    var pt2 = new Vector2(pt.x, pt.y);
                    min = Vector2.Min(min, pt2);
                    max = Vector2.Max(max, pt2);
                }
            }
            return min == Vector2.positiveInfinity ? Rect.zero : new Rect(min, max - min);
        }
    }

}

