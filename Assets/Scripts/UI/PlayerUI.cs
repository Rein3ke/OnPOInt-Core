using UnityEngine;

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

    private void OnGUI()
    {
        GUI.DrawTexture(
            new Rect(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), Vector2.one * CrosshairSize), 
            m_cursorHighlight ? CrossHairTexHover : CrossHairTexDefault);
    }

    public void HighlightCursor(bool _isHighlighted)
    {
        m_cursorHighlight = _isHighlighted;
    }

    private static Texture2D CreateSimpleTex(Color _color)
    {
        var tx = new Texture2D(1, 1, TextureFormat.RGB24, false);
        tx.SetPixel(1, 1, _color);
        tx.Apply();
        return tx;
    }
}
