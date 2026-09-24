// CrewJournal — retratos 40x48 em grade fixa (cabeca y2-14, corpo y16-34).
// Pontos de encaixe constantes p/ todas as variantes (GDD-31).
using System.Collections.Generic;
using UnityEngine;
using CrewJournal.Logic;

public static class PortraitRenderer
{
    static readonly Color[][] Skins = new Color[][] {
        new Color[] { new Color(0.99f, 0.87f, 0.74f), new Color(0.93f, 0.76f, 0.60f) },
        new Color[] { new Color(0.96f, 0.79f, 0.62f), new Color(0.87f, 0.66f, 0.48f) },
        new Color[] { new Color(0.87f, 0.64f, 0.45f), new Color(0.76f, 0.52f, 0.34f) },
        new Color[] { new Color(0.68f, 0.47f, 0.31f), new Color(0.56f, 0.37f, 0.23f) },
        new Color[] { new Color(0.45f, 0.30f, 0.20f), new Color(0.35f, 0.22f, 0.14f) }
    };
    static readonly Color[] HairCols = new Color[] {
        new Color(0.12f, 0.10f, 0.10f), new Color(0.35f, 0.22f, 0.12f), new Color(0.65f, 0.28f, 0.12f),
        new Color(0.85f, 0.70f, 0.40f), new Color(0.60f, 0.60f, 0.62f), new Color(0.92f, 0.92f, 0.94f)
    };

    static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

    public static Texture2D Render(int worldSeed, string charId, string job, List<string> traits, bool scar, int outfitMod, bool grave)
    {
        string key = worldSeed + "|" + charId + "|" + scar + "|" + outfitMod + "|" + grave;
        Texture2D t;
        if (cache.TryGetValue(key, out t)) return t;
        CharacterVisual v = VisualDNA.Character(worldSeed, charId, job, traits, scar, outfitMod);
        Texture2D lib;
        Color[] libPx = null;
        if (PartSources.Current != null
            && PartSources.Current.TryGet("Portraits", SpriteLib.PortraitKey(job, worldSeed, charId), out lib))
            libPx = Pixel.ClonePixels(lib, 40, 48);
        if (libPx != null)
        {
            // Base externa (IA) + cicatriz procedural por cima (historia).
            t = Pixel.ToTexture(libPx, 40, 48);
            ApplyScar(t, v);
        }
        else
        {
            t = Draw(v, false, grave);
        }
        cache[key] = t;
        return t;
    }

    public static Texture2D RenderEnemy(int seed, int typeIdx, int n)
    {
        string key = "e|" + seed + "|" + typeIdx + "|" + n;
        Texture2D t;
        if (cache.TryGetValue(key, out t)) return t;
        CharacterVisual v = VisualDNA.Enemy(seed, typeIdx, n);
        Texture2D lib;
        Color[] libPx = null;
        if (PartSources.Current != null
            && PartSources.Current.TryGet("Portraits", SpriteLib.EnemyKey(typeIdx, n), out lib))
            libPx = Pixel.ClonePixels(lib, 40, 48);
        if (libPx != null)
        {
            t = Pixel.ToTexture(libPx, 40, 48);
            ApplyScar(t, v);
        }
        else
        {
            t = Draw(v, true, false);
        }
        cache[key] = t;
        return t;
    }

    static Color OutfitBase(string code)
    {
        if (code.StartsWith("navigator")) return new Color(0.13f, 0.25f, 0.45f);
        if (code.StartsWith("cook")) return new Color(0.92f, 0.90f, 0.85f);
        if (code.StartsWith("medic")) return new Color(0.94f, 0.93f, 0.88f);
        if (code.StartsWith("carpenter")) return new Color(0.48f, 0.32f, 0.18f);
        if (code.StartsWith("fighter")) return new Color(0.55f, 0.16f, 0.14f);
        if (code.StartsWith("shooter")) return new Color(0.20f, 0.42f, 0.22f);
        if (code.StartsWith("enemy")) return new Color(0.30f, 0.22f, 0.38f);
        return new Color(0.35f, 0.45f, 0.58f);
    }

    static Texture2D Draw(CharacterVisual v, bool hostile, bool grave)
    {
        int W = 40, H = 48;
        Color[] b = new Color[W * H];
        Pixel.Fill(b, W, H, new Color(0, 0, 0, 0));
        Color[] skin = Skins[v.skinTone % Skins.Length];
        if (hostile) { skin = new Color[] { Pixel.Shade(skin[0], 0.75f), Pixel.Shade(skin[1], 0.75f) }; }
        Color hair = HairCols[v.hairColor % HairCols.Length];
        Color shirt = OutfitBase(v.outfit);
        Color dark = new Color(0.10f, 0.08f, 0.08f);
        Color gold = new Color(0.85f, 0.68f, 0.20f);
        Color red = new Color(0.75f, 0.15f, 0.12f);

        // corpo
        Pixel.Rect(b, W, H, 12, 20, 16, 14, shirt);
        Pixel.Rect(b, W, H, 26, 20, 2, 14, Pixel.Shade(shirt, 0.8f));
        Pixel.Rect(b, W, H, 10, 22, 2, 10, skin[0]);
        Pixel.Rect(b, W, H, 28, 22, 2, 10, skin[0]);
        Pixel.Rect(b, W, H, 12, 32, 16, 2, Pixel.Shade(shirt, 0.7f));
        if (v.outfit.StartsWith("navigator")) Pixel.Rect(b, W, H, 12, 30, 16, 1, gold);
        if (v.outfit.StartsWith("medic")) { Pixel.Rect(b, W, H, 19, 24, 2, 6, red); Pixel.Rect(b, W, H, 17, 26, 6, 2, red); }
        // pescoco + cabeca
        Pixel.Rect(b, W, H, 18, 15, 4, 5, skin[1]);
        Pixel.Rect(b, W, H, 14, 3, 12, 12, skin[0]);
        Pixel.Rect(b, W, H, 13, 8, 1, 3, skin[0]);
        Pixel.Rect(b, W, H, 26, 8, 1, 3, skin[0]);
        // olhos/boca por variante
        Color eye = hostile ? red : dark;
        int fv = v.face % 4;
        if (fv == 0) { Pixel.Px(b, W, H, 17, 8, eye); Pixel.Px(b, W, H, 22, 8, eye); }
        else if (fv == 1) { Pixel.Rect(b, W, H, 16, 6, 3, 1, dark); Pixel.Rect(b, W, H, 21, 6, 3, 1, dark); Pixel.Px(b, W, H, 17, 8, eye); Pixel.Px(b, W, H, 22, 8, eye); }
        else if (fv == 2) { Pixel.Rect(b, W, H, 16, 8, 2, 1, eye); Pixel.Rect(b, W, H, 22, 8, 2, 1, eye); }
        else { Pixel.Rect(b, W, H, 17, 8, 1, 1, Pixel.Shade(skin[0], 0.7f)); Pixel.Rect(b, W, H, 22, 8, 1, 1, Pixel.Shade(skin[0], 0.7f)); }
        Pixel.Rect(b, W, H, 19, 12, 2, 1, Pixel.Shade(skin[0], 0.65f));
        // cabelo
        int style = v.hair % 5;
        if (style == 0) { Pixel.Rect(b, W, H, 14, 1, 12, 3, hair); Pixel.Rect(b, W, H, 13, 3, 2, 5, hair); Pixel.Rect(b, W, H, 25, 3, 2, 5, hair); }
        else if (style == 1) { Pixel.Rect(b, W, H, 14, 1, 12, 3, hair); Pixel.Rect(b, W, H, 12, 3, 2, 13, hair); Pixel.Rect(b, W, H, 26, 3, 2, 13, hair); }
        else if (style == 2) { Pixel.Rect(b, W, H, 14, 3, 12, 2, gold); }
        else if (style == 3) { Pixel.Rect(b, W, H, 14, 1, 12, 3, hair); Pixel.Rect(b, W, H, 27, 4, 2, 10, hair); }
        else { for (int x = 13; x <= 26; x++) { int hgt = 2 + ((x + v.hair) % 3); Pixel.Rect(b, W, H, x, 3 - hgt + 1, 1, hgt + 1, hair); } }
        // barba
        if (v.beard == 1) { Pixel.Rect(b, W, H, 15, 12, 10, 1, Pixel.Shade(hair, 0.9f)); }
        else if (v.beard == 2) { Pixel.Rect(b, W, H, 15, 11, 10, 4, Pixel.Shade(hair, 0.9f)); Pixel.Rect(b, W, H, 19, 12, 2, 1, skin[0]); }
        // acessorios
        if (v.accessory == 1) { Pixel.Rect(b, W, H, 11, 4, 18, 2, Pixel.Shade(shirt, 0.6f)); Pixel.Rect(b, W, H, 15, 0, 10, 4, Pixel.Shade(shirt, 0.6f)); }
        else if (v.accessory == 2) { Pixel.Rect(b, W, H, 13, 4, 14, 2, red); }
        else if (v.accessory == 3) { Pixel.Rect(b, W, H, 15, 8, 10, 1, dark); Pixel.Px(b, W, H, 15, 7, dark); Pixel.Px(b, W, H, 24, 7, dark); }
        else if (v.accessory == 4) { Pixel.Px(b, W, H, 26, 10, gold); }
        else if (v.accessory == 5) { Pixel.Rect(b, W, H, 18, 21, 4, 1, gold); }
        else if (v.accessory == 6) { Pixel.Rect(b, W, H, 25, 12, 3, 1, new Color(0.4f, 0.25f, 0.12f)); }
        else if (v.accessory == 7) { Pixel.Rect(b, W, H, 12, 1, 3, 14, Pixel.Shade(shirt, 0.55f)); Pixel.Rect(b, W, H, 25, 1, 3, 14, Pixel.Shade(shirt, 0.55f)); Pixel.Rect(b, W, H, 12, 1, 16, 2, Pixel.Shade(shirt, 0.55f)); }
        // cicatriz (tambem aplicada sobre base externa via ApplyScar)
        if (v.scar) { Pixel.Px(b, W, H, 22, 9, red); Pixel.Px(b, W, H, 23, 10, red); Pixel.Px(b, W, H, 24, 11, red); }
        return Pixel.ToTexture(b, W, H);
    }

    // Cicatriz sobre textura pronta. SetPixel usa y de baixo p/ cima:
    // y = H-1-yTop, H = 48.
    static void ApplyScar(Texture2D t, CharacterVisual v)
    {
        if (!v.scar) return;
        Color red = new Color(0.75f, 0.15f, 0.12f);
        t.SetPixel(22, 38, red);
        t.SetPixel(23, 37, red);
        t.SetPixel(24, 36, red);
        t.Apply();
    }
}
