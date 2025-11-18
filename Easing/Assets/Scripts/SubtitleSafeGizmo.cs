using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class SubtitleSafeGizmo : MonoBehaviour
{
    [Header("Safe Area Settings")]
    [Range(0f, 0.5f)]
    [Tooltip("How much of the bottom of the screen (0–0.5) is reserved for subtitles.")]
    public float subtitleHeight = 0.2f; // 20% of screen height

    [Range(0f, 0.5f)]
    [Tooltip("Horizontal margins on left/right as a fraction of width.")]
    public float horizontalMargin = 0.05f; // 5% on each side

    [Header("Colors")]
    public Color subtitleAreaColor = Color.yellow;
    public Color gameplaySafeAreaColor = Color.green;

    private Camera cam;

    void OnValidate()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void OnDrawGizmos()
    {
        if (cam == null) cam = GetComponent<Camera>();

        // Depth at which to draw the gizmo
        float depth = cam.orthographic ? 0f : cam.nearClipPlane + 1f;

        // --- SUBTITLE AREA (bottom strip) ---
        float x0 = horizontalMargin;
        float x1 = 1f - horizontalMargin;
        float y0 = 1f;
        float y1 = 1f - subtitleHeight;

        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(x0, y0, depth));
        Vector3 br = cam.ViewportToWorldPoint(new Vector3(x1, y0, depth));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(x1, y1, depth));
        Vector3 tl = cam.ViewportToWorldPoint(new Vector3(x0, y1, depth));

        Gizmos.color = subtitleAreaColor;
        DrawRect(bl, br, tr, tl);

        // --- GAMEPLAY SAFE AREA (below subtitles) ---
        float safeY0 = 1f - subtitleHeight;
        float safeY1 = 0f;

        Vector3 sbl = cam.ViewportToWorldPoint(new Vector3(x0, safeY0, depth));
        Vector3 sbr = cam.ViewportToWorldPoint(new Vector3(x1, safeY0, depth));
        Vector3 str = cam.ViewportToWorldPoint(new Vector3(x1, safeY1, depth));
        Vector3 stl = cam.ViewportToWorldPoint(new Vector3(x0, safeY1, depth));

        Gizmos.color = gameplaySafeAreaColor;
        DrawRect(sbl, sbr, str, stl);
    }

    private void DrawRect(Vector3 bl, Vector3 br, Vector3 tr, Vector3 tl)
    {
        Gizmos.DrawLine(bl, br);
        Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl);
        Gizmos.DrawLine(tl, bl);
    }
}