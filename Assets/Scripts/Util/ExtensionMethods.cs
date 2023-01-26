using Dreamteck.Splines;
using UnityEngine;
using static Dreamteck.Splines.Node;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2, bool clamped = false)
    {
        float mapped = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        return (clamped == true) ? Mathf.Clamp(mapped, from2, to2) : mapped;
    }

        /// <summary>
        /// Set pivot without changing the position of the element
        /// </summary>
    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
        deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
        deltaPosition.Scale(rectTransform.localScale);          // apply scaling
        deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

        rectTransform.pivot = pivot;                            // change the pivot
        rectTransform.localPosition -= deltaPosition;           // reverse the position change
    }

    public static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
    {
        T component = behaviour.gameObject.GetComponent<T>();

        if (component == null)
            component = behaviour.gameObject.AddComponent<T>() as T;

        return component;

    }
    public static int GetConnectionIndex(this Node value, SplineComputer computer, int pointIndex)
    {
        Connection[] connections = value.GetConnections();
        for (int i = 0; i < connections.Length; i++)
        {
            Connection c = connections[i];
            if (c.pointIndex == pointIndex && c.computer == computer) return i;
        }

        return 0;
    }
}