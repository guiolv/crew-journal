// CrewJournal — navios 64x40: casco/mastro/vela/canhaos/decor por DNA (GDD-31).
using System.Collections.Generic;
using UnityEngine;
using CrewJournal.Logic;

public static class ShipRenderer
{
    static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

    public static Texture2D Render(string defId, int worldSeed, int specialMods)
    {
        string key = defId + "|" + worldSeed + "|" + specialMods;
        Texture2D t;
        if (cache.TryGetValue(key, out t)) return t;
        t = Draw(VisualDNA.Ship(defId, worldSeed, specialMods));
        cache[key] = t;
        return t;
    }

    static Texture2D Draw(ShipVisual v)
    {
        int W = 64, H = 40;
        Color[] b = new Color[W * H];
        Pixel.Fill(b, W, H, new Color(0, 0, 0, 0));
        Color sea = new Color(0.10f, 0.32f, 0.46f);
        Color hullC = v.hull == 2 ? new Color(0.30f, 0.28f, 0.30f) : new Color(0.45f, 0.30f, 0.16f);
        Color sailC = new Color(0.93f, 0.89f, 0.78f);
        if (v.sail == 1) sailC = new Color(0.85f, 0.80f, 0.65f);
        if (v.sail == 2) sailC = new Color(0.75f, 0.30f, 0.22f);
        if (v.sail == 3) sailC = new Color(0.85f, 0.82f, 0.70f);
        Color dark = new Color(0.12f, 0.10f, 0.10f);
        Color gold = new Color(0.85f, 0.68f, 0.20f);

        if (v.masts == 0)
        {
            // barril
            Pixel.Rect(b, W, H, 26, 12, 12, 14, hullC);
            Pixel.Rect(b, W, H, 26, 12, 12, 2, dark);
            Pixel.Rect(b, W, H, 26, 24, 12, 2, dark);
            Pixel.Rect(b, W, H, 26, 18, 12, 1, dark);
            Pixel.Rect(b, W, H, 20, 28, 24, 4, sea);
            return Pixel.ToTexture(b, W, H);
        }
        int ySea = 8;
        Pixel.Rect(b, W, H, 0, H - ySea - 4, W, 4, sea);
        int deck = H - ySea - 4; // yTop do conves
        int len = v.hull == 2 ? 44 : 34;
        int x0 = (W - len) / 2;
        // casco trapezoidal
        for (int r = 0; r < 8; r++)
        {
            int inset = r / 2;
            Pixel.Rect(b, W, H, x0 + inset, deck - 8 + r, len - inset * 2, 1, r == 0 ? Pixel.Shade(hullC, 0.7f) : hullC);
        }
        if (v.decor >= 1) Pixel.Rect(b, W, H, x0 + 2, deck - 3, len - 4, 1, gold);
        // canhoes
        for (int i = 0; i <= v.cannon + 1; i++)
        {
            Pixel.Px(b, W, H, x0 + 4 + i * ((len - 8) / (v.cannon + 2)), deck - 5, dark);
        }
        // mastros + velas
        int[] masts = v.masts == 2 ? new int[] { W / 2 - 10, W / 2 + 10 } : new int[] { W / 2 };
        for (int mi = 0; mi < masts.Length; mi++)
        {
            int mx = masts[mi];
            Pixel.Rect(b, W, H, mx, 4, 2, deck - 8 - 4, new Color(0.35f, 0.24f, 0.14f));
            int sw = v.hull == 2 ? 14 : 11;
            Pixel.Rect(b, W, H, mx - sw / 2, 5, sw, 10, sailC);
            if (v.sail == 2) Pixel.Rect(b, W, H, mx - sw / 2, 9, sw, 2, new Color(0.93f, 0.89f, 0.78f));
            Pixel.Rect(b, W, H, mx - sw / 2, 5, sw, 1, Pixel.Shade(sailC, 0.8f));
        }
        // figura de proa / decor
        if (v.decor >= 2) { Pixel.Px(b, W, H, x0 + len - 1, deck - 9, gold); Pixel.Px(b, W, H, x0 + len - 1, deck - 10, gold); }
        Pixel.Rect(b, W, H, masts[masts.Length - 1] + 1, 2, 4, 3, new Color(0.75f, 0.15f, 0.12f));
        return Pixel.ToTexture(b, W, H);
    }
}
