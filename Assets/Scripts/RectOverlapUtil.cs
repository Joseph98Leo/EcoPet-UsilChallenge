using UnityEngine;

public static class RectOverlapUtil
{
    public static bool Overlaps(RectTransform a, RectTransform b)
    {
        if (a == null || b == null) return false;
        return WorldRect(a).Overlaps(WorldRect(b));
    }

    private static Rect WorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }
}
