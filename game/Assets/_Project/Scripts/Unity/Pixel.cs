// CrewJournal — base pixel-art: buffer + primitivas. Estilo unico: pixel duro, sem blur.
using UnityEngine;

public static class Pixel
{
    public static void Fill(Color[] b, int W, int H, Color c)
    {
        for (int i = 0; i < b.Length; i++) b[i] = c;
    }

    public static void Rect(Color[] b, int W, int H, int x, int yTop, int w, int h, Color c)
    {
        for (int y = 0; y < h; y++)
        {
            for (int x2 = 0; x2 < w; x2++)
            {
                Px(b, W, H, x + x2, yTop + y, c);
            }
        }
    }

    public static void Px(Color[] b, int W, int H, int x, int yTop, Color c)
    {
        if (x < 0 || x >= W || yTop < 0 || yTop >= H) return;
        if (c.a <= 0f) return;
        b[(H - 1 - yTop) * W + x] = c;
    }

    public static Color Shade(Color c, float f)
    {
        return new Color(c.r * f, c.g * f, c.b * f, c.a);
    }

    public static Texture2D ToTexture(Color[] b, int W, int H)
    {
        Texture2D t = new Texture2D(W, H, TextureFormat.RGBA32, false);
        t.filterMode = FilterMode.Point;
        t.SetPixels(b);
        t.Apply();
        return t;
    }
}
