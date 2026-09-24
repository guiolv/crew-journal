// CrewJournal — ilhas 96x64: silhueta + bioma + pecas da gramatica (GDD-31).
using System.Collections.Generic;
using UnityEngine;
using CrewJournal.Logic;

public static class IslandRenderer
{
    static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

    public static Texture2D Render(int worldSeed, string islandId, IslandArchetype arch)
    {
        string key = worldSeed + "|" + islandId;
        Texture2D t;
        if (cache.TryGetValue(key, out t)) return t;
        IslandVisual v = VisualDNA.Island(worldSeed, islandId, arch);
        Texture2D lib;
        Color[] libPx = null;
        if (PartSources.Current != null
            && PartSources.Current.TryGet("Islands", SpriteLib.IslandKey(arch, v.silhouette), out lib))
            libPx = Pixel.ClonePixels(lib, 96, 64);
        Color[] b = libPx != null ? libPx : PaintBase(v);
        // Pecas da gramatica sempre procedurais por cima (layout deterministico).
        DrawPieces(b, 96, 64, v);
        t = Pixel.ToTexture(b, 96, 64);
        cache[key] = t;
        return t;
    }

    static Color[] PaintBase(IslandVisual v)
    {
        int W = 96, H = 64;
        Color[] b = new Color[W * H];
        Color shallow = new Color(0.20f, 0.50f, 0.62f);
        Color sand = new Color(0.87f, 0.78f, 0.55f);
        Color grass = new Color(0.32f, 0.58f, 0.28f);
        Color forest = new Color(0.18f, 0.42f, 0.20f);
        Color rock = new Color(0.50f, 0.48f, 0.45f);
        Pixel.Fill(b, W, H, new Color(0, 0, 0, 0));

        float cx = W / 2f, cy = H / 2f + 2;
        float rx = 30 + (v.silhouette % 2) * 6;
        float ry = 20 + (v.silhouette / 2) * 5;
        Color in1 = grass, in2 = forest;
        if (v.biome == "Arid") { in1 = new Color(0.80f, 0.68f, 0.42f); in2 = new Color(0.66f, 0.52f, 0.30f); }
        else if (v.biome == "Cold") { in1 = new Color(0.82f, 0.85f, 0.88f); in2 = rock; }
        else if (v.biome == "Temperate") { in1 = new Color(0.38f, 0.60f, 0.32f); in2 = forest; }
        for (int x = 0; x < W; x++)
        {
            float wob = 1f + 0.12f * (float)System.Math.Sin(x * (1 + v.silhouette) * 0.35);
            for (int yy = 0; yy < H; yy++)
            {
                float dx = (x - cx) / (rx * wob);
                float dy = (yy - cy) / ry;
                float dd = dx * dx + dy * dy;
                int yTop = H - 1 - yy;
                if (dd < 0.62) Pixel.Px(b, W, H, x, yTop, in2);
                else if (dd < 0.85) Pixel.Px(b, W, H, x, yTop, in1);
                else if (dd < 1.0) Pixel.Px(b, W, H, x, yTop, sand);
                else if (dd < 1.15) Pixel.Px(b, W, H, x, yTop, shallow);
            }
        }
        // (pecas vao por cima no Render via DrawPieces)
        return b;
    }

    static void DrawPieces(Color[] b, int W, int H, IslandVisual v)
    {
        for (int i = 0; i < v.pieces.Count; i++)
        {
            IslandPiece p = v.pieces[i];
            int px = (int)(p.x * W);
            int py = (int)(p.y * H);
            DrawPiece(b, W, H, px, py, p.kind);
        }
    }

    static void DrawPiece(Color[] b, int W, int H, int x, int yTop, string kind)
    {
        Color wood = new Color(0.45f, 0.30f, 0.16f);
        Color wall = new Color(0.82f, 0.72f, 0.52f);
        Color roof = new Color(0.65f, 0.22f, 0.16f);
        Color gray = new Color(0.55f, 0.53f, 0.50f);
        Color dark = new Color(0.16f, 0.14f, 0.12f);
        Color leaf = new Color(0.16f, 0.40f, 0.18f);
        Color gold = new Color(0.85f, 0.68f, 0.20f);
        Color red = new Color(0.75f, 0.15f, 0.12f);
        if (kind == "house" || kind == "suburb")
        {
            Pixel.Rect(b, W, H, x - 2, yTop - 2, 5, 4, wall);
            Pixel.Rect(b, W, H, x - 3, yTop - 4, 7, 2, roof);
        }
        else if (kind == "market") { Pixel.Rect(b, W, H, x - 3, yTop - 2, 7, 4, wall); Pixel.Rect(b, W, H, x - 3, yTop - 4, 7, 2, gold); }
        else if (kind == "port" || kind == "dock") { Pixel.Rect(b, W, H, x - 4, yTop - 1, 9, 3, wood); Pixel.Rect(b, W, H, x - 3, yTop + 2, 1, 3, dark); Pixel.Rect(b, W, H, x + 3, yTop + 2, 1, 3, dark); }
        else if (kind == "warehouse") { Pixel.Rect(b, W, H, x - 4, yTop - 3, 9, 5, wood); Pixel.Rect(b, W, H, x - 4, yTop - 5, 9, 2, Pixel.Shade(wood, 0.7f)); }
        else if (kind == "tavern") { Pixel.Rect(b, W, H, x - 2, yTop - 2, 5, 4, wood); Pixel.Rect(b, W, H, x - 3, yTop - 4, 7, 2, roof); Pixel.Px(b, W, H, x, yTop - 1, gold); }
        else if (kind == "castle") { Pixel.Rect(b, W, H, x - 4, yTop - 6, 9, 8, gray); Pixel.Rect(b, W, H, x - 6, yTop - 8, 3, 10, gray); Pixel.Rect(b, W, H, x + 4, yTop - 8, 3, 10, gray); Pixel.Px(b, W, H, x, yTop - 9, red); }
        else if (kind == "church") { Pixel.Rect(b, W, H, x - 2, yTop - 3, 5, 5, new Color(0.92f, 0.90f, 0.84f)); Pixel.Rect(b, W, H, x, yTop - 6, 1, 3, gold); Pixel.Rect(b, W, H, x - 1, yTop - 5, 3, 1, gold); }
        else if (kind == "gov") { Pixel.Rect(b, W, H, x - 3, yTop - 3, 7, 5, new Color(0.55f, 0.62f, 0.70f)); Pixel.Rect(b, W, H, x - 3, yTop - 5, 7, 2, gray); }
        else if (kind == "plaza") { Pixel.Rect(b, W, H, x - 3, yTop - 2, 7, 4, new Color(0.78f, 0.74f, 0.62f)); }
        else if (kind == "farm") { Pixel.Rect(b, W, H, x - 4, yTop - 2, 9, 4, new Color(0.45f, 0.62f, 0.25f)); Pixel.Rect(b, W, H, x - 4, yTop - 1, 9, 1, Pixel.Shade(leaf, 1.1f)); }
        else if (kind == "forest") { Pixel.Rect(b, W, H, x - 3, yTop - 3, 7, 5, leaf); Pixel.Rect(b, W, H, x - 1, yTop - 2, 3, 3, Pixel.Shade(leaf, 0.7f)); }
        else if (kind == "trees") { Pixel.Px(b, W, H, x, yTop, leaf); Pixel.Px(b, W, H, x + 2, yTop - 1, leaf); Pixel.Px(b, W, H, x - 2, yTop + 1, leaf); }
        else if (kind == "ruins") { Pixel.Rect(b, W, H, x - 3, yTop - 1, 3, 3, gray); Pixel.Rect(b, W, H, x + 1, yTop - 2, 2, 4, gray); }
        else if (kind == "cave") { Pixel.Rect(b, W, H, x - 3, yTop - 3, 7, 5, dark); Pixel.Rect(b, W, H, x - 1, yTop - 1, 3, 3, new Color(0.05f, 0.05f, 0.06f)); }
        else if (kind == "boss") { Pixel.Rect(b, W, H, x - 2, yTop - 2, 5, 5, red); Pixel.Px(b, W, H, x - 1, yTop - 1, new Color(1, 1, 1)); Pixel.Px(b, W, H, x + 1, yTop - 1, new Color(1, 1, 1)); Pixel.Px(b, W, H, x, yTop, new Color(1, 1, 1)); }
        else if (kind == "beach") { Pixel.Rect(b, W, H, x - 4, yTop - 1, 9, 3, new Color(0.87f, 0.78f, 0.55f)); }
    }
}
