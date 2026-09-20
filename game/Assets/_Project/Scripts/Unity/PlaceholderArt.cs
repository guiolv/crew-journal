// CrewJournal — placeholders procedurais (sem assets externos, sem licenca).
using UnityEngine;

public static class PlaceholderArt
{
    public static Sprite SeaSprite;
    public static Sprite IslandSprite;
    public static Sprite ShipSprite;
    public static Sprite DangerSprite;

    public static void Init()
    {
        if (SeaSprite != null) return;
        SeaSprite = Flat(Color.cyan * 0.55f);
        IslandSprite = Flat(new Color(0.35f, 0.75f, 0.35f));
        ShipSprite = Flat(new Color(0.55f, 0.35f, 0.2f));
        DangerSprite = Flat(new Color(0.8f, 0.25f, 0.25f));
    }

    static Sprite Flat(Color c)
    {
        Texture2D t = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color[] px = new Color[256];
        for (int i = 0; i < px.Length; i++) px[i] = c;
        t.SetPixels(px);
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
    }
}
