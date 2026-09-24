// CrewJournal — skin da UI por PNGs externos (Tools/sprite-gen, categoria "UI").
// Tudo com fallback: PNG ausente = cores chapadas atuais, nada quebra.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrewJournal.Logic;

public static class UIStyle
{
    static Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static Sprite Get(string key)
    {
        Sprite s;
        if (cache.TryGetValue(key, out s)) return s;
        Texture2D t = Resources.Load<Texture2D>("Art/UI/" + key);
        if (t == null) t = Resources.Load<Texture2D>("Art/Icons/" + key);
        if (t == null) return null;
        s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), 100f);
        cache[key] = s;
        return s;
    }

    static bool Same(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) && Mathf.Approximately(a.g, b.g) && Mathf.Approximately(a.b, b.b);
    }

    // Botoes: mapeia a cor atual p/ estilo. Assinaturas do GameUI nao mudam.
    public static void SkinButton(GameObject go, Color bg)
    {
        string base_ = null;
        if (Same(bg, new Color(0.11f, 0.36f, 0.43f))) base_ = "btn_teal";
        else if (Same(bg, new Color(0.08f, 0.25f, 0.31f))) base_ = "btn_teal_dark";
        else if (Same(bg, new Color(0.95f, 0.77f, 0.25f))) base_ = "btn_gold";
        if (base_ == null) return;
        Sprite n = Get(base_);
        if (n == null) return;
        Image img = go.GetComponent<Image>();
        img.sprite = n;
        img.color = Color.white;
        img.type = Image.Type.Sliced;
        Button b = go.GetComponent<Button>();
        if (b == null) return;
        b.transition = Selectable.Transition.SpriteSwap;
        SpriteState st = new SpriteState();
        Sprite p = Get(base_ + "_pressed");
        st.pressedSprite = p != null ? p : n;
        st.highlightedSprite = n;
        st.disabledSprite = n;
        b.spriteState = st;
    }

    public static void SkinCard(GameObject go)
    {
        Sprite s = Get("panel_parch");
        if (s == null) return;
        Image img = go.GetComponent<Image>();
        img.sprite = s;
        img.color = Color.white;
        img.type = Image.Type.Sliced;
    }

    // Icone 32px como primeiro filho (ex.: recursos no TradeRow). Null se ausente.
    public static GameObject Icon(Transform parent, string key)
    {
        Sprite s = Get(key);
        if (s == null) return null;
        GameObject go = new GameObject("Icon");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.sprite = s;
        img.raycastTarget = false;
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minWidth = 32;
        le.minHeight = 32;
        le.preferredWidth = 32;
        le.preferredHeight = 32;
        go.transform.SetSiblingIndex(0);
        return go;
    }

    // Pin do mapa: icone do arquetipo a esquerda dentro do botao.
    public static void PinIcon(GameObject pin, IslandArchetype arch)
    {
        Sprite s = Get("pin_" + arch.ToString());
        if (s == null) return;
        GameObject go = new GameObject("PinIcon");
        go.transform.SetParent(pin.transform, false);
        Image img = go.AddComponent<Image>();
        img.sprite = s;
        img.raycastTarget = false;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.anchoredPosition = new Vector2(26, 0);
        rt.sizeDelta = new Vector2(44, 44);
    }

    public static void SkinProgress(Image track, Image fill)
    {
        Sprite t = Get("prog_track");
        if (t != null) { track.sprite = t; track.color = Color.white; }
        Sprite f = Get("prog_fill");
        if (f != null) { fill.sprite = f; fill.color = Color.white; }
    }

    // Moldura sobre retrato/isla/navio.
    public static void Frame(Transform holder, float w, float h)
    {
        Sprite s = Get("frame_portrait");
        if (s == null) return;
        GameObject go = new GameObject("Frame");
        go.transform.SetParent(holder, false);
        Image img = go.AddComponent<Image>();
        img.sprite = s;
        img.raycastTarget = false;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(w + 8, h + 8);
    }

    public static void Compass(RectTransform sea)
    {
        Sprite s = Get("compass_rose");
        if (s == null) return;
        GameObject go = new GameObject("Compass");
        go.transform.SetParent(sea, false);
        Image img = go.AddComponent<Image>();
        img.sprite = s;
        img.raycastTarget = false;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-40, -40);
        rt.sizeDelta = new Vector2(64, 64);
    }
}
