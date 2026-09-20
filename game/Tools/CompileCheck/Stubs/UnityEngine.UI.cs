// STUBS apenas para type-check local com csc (NAO vai para Assets/Unity).
// Espelham a API real usada em GameUI.cs.
using UnityEngine;

namespace UnityEngine.UI
{
    public class CanvasScaler : MonoBehaviour
    {
        public enum ScaleMode { ConstantPixelSize, ScaleWithScreenSize, ConstantPhysicalSize }
        public ScaleMode uiScaleMode;
        public Vector2 referenceResolution;
    }

    public class GraphicRaycaster : MonoBehaviour
    {
    }

    public class Text : MonoBehaviour
    {
        public Font font;
        public int fontSize;
        public Color color;
        public TextAnchor alignment;
        public string text;
    }

    public class Button : MonoBehaviour
    {
        public UnityEngine.Events.ButtonClickedEvent onClick = new UnityEngine.Events.ButtonClickedEvent();
    }

    public class Image : MonoBehaviour
    {
        public Color color;
        public Sprite sprite;
    }

    public class HorizontalLayoutGroup : MonoBehaviour
    {
        public float spacing;
        public RectOffset padding;
    }
}

namespace UnityEngine.Events
{
    public class ButtonClickedEvent : UnityEvent
    {
    }
}
