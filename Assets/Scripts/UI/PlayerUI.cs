using UnityEngine;

/// <summary>
/// Part of the PlayerController - Responsible for drawing a dot (crosshair) in the center of the screen
/// </summary>
public class PlayerUI : MonoBehaviour
{
    public float CrosshairSize = 4f;

    private static Texture2D CrossHairTexDefault
    {
        get
        {
            if (s_crossHairDefault == null)
            {
                s_crossHairDefault = CreateSimpleTex(Color.white);
            }
            return s_crossHairDefault;
        }
    }
    private static Texture2D s_crossHairDefault = null;

    private static Texture2D CrossHairTexHover
    {
        get
        {
            if (s_crossHairHover == null)
            {
                s_crossHairHover = CreateSimpleTex(Color.green);
            }
            return s_crossHairHover;
        }
    }
    private static Texture2D s_crossHairHover = null;

    private bool m_cursorHighlight;

    /// <summary>
    /// Draws a dot in the center of the screen.
    /// The color of the dot depends on the m_cursorHighlight variable.
    /// </summary>
    private void OnGUI()
    {
        GUI.DrawTexture(
            new Rect(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), Vector2.one * CrosshairSize), 
            m_cursorHighlight ? CrossHairTexHover : CrossHairTexDefault);
    }

    /// <summary>
    /// Function gets passed a boolean and determines whether to be highlighted or not.
    /// </summary>
    /// <param name="_isHighlighted">Boolean - sets the value whether to highlight or not.</param>
    public void HighlightCursor(bool _isHighlighted)
    {
        m_cursorHighlight = _isHighlighted;
    }

    /// <summary>
    /// Creates a small texture (dot).
    /// </summary>
    /// <param name="_color">Determines the color of the texture.</param>
    /// <returns>Texture</returns>
    private static Texture2D CreateSimpleTex(Color _color)
    {
        var tx = new Texture2D(1, 1, TextureFormat.RGB24, false);
        tx.SetPixel(1, 1, _color);
        tx.Apply();
        return tx;
    }
}
