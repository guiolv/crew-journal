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
        public bool interactable = true;
        public Selectable.Transition transition;
        public SpriteState spriteState;
    }

    public struct SpriteState
    {
        public Sprite highlightedSprite;
        public Sprite pressedSprite;
        public Sprite selectedSprite;
        public Sprite disabledSprite;
    }

    public class Selectable : MonoBehaviour
    {
        public enum Transition { None, ColorTint, SpriteSwap, Animation }
    }

    public class Image : MonoBehaviour
    {
        public enum Type { Simple, Sliced, Tiled, Filled }
        public enum FillMethod { Horizontal, Vertical, Radial90, Radial180, Radial360 }
        public Color color;
        public Sprite sprite;
        public Type type;
        public FillMethod fillMethod;
        public float fillAmount;
        public bool raycastTarget;
    }

    public class HorizontalLayoutGroup : MonoBehaviour
    {
        public float spacing;
        public RectOffset padding;
        public bool childControlWidth;
        public bool childControlHeight;
        public bool childForceExpandWidth;
        public bool childForceExpandHeight;
    }

    public class VerticalLayoutGroup : MonoBehaviour
    {
        public float spacing;
        public RectOffset padding;
        public bool childControlWidth;
        public bool childControlHeight;
        public bool childForceExpandWidth;
        public bool childForceExpandHeight;
    }

    public class ContentSizeFitter : MonoBehaviour
    {
        public enum FitMode { Unconstrained, MinSize, PreferredSize }
        public FitMode horizontalFit;
        public FitMode verticalFit;
    }

    public class LayoutElement : MonoBehaviour
    {
        public float minWidth;
        public float minHeight;
        public float preferredWidth;
        public float preferredHeight;
        public float flexibleWidth;
        public float flexibleHeight;
    }

    public class ScrollRect : MonoBehaviour
    {
        public RectTransform content;
        public RectTransform viewport;
        public bool horizontal;
        public bool vertical;
    }

    public class RectMask2D : MonoBehaviour
    {
    }
}

namespace UnityEngine.Events
{
    public class ButtonClickedEvent : UnityEvent
    {
    }
}
