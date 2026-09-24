// CrewJournal — ilhas 128x128 compostas de tiles Kenney (monochrome Default).
// Gramatica + seed identicas (VisualDNA); so a pintura mudou. Fallback chapado se faltar tile.
// Convencao: yTop (0 = topo), igual a Pixel.Px.
using System.Collections.Generic;
using UnityEngine;
using CrewJournal.Logic;

public static class IslandRenderer
{
    const int T = 16;
    const int N = 8;
    const int W = 128;
    const int H = 128;

    static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();
    static Dictionary<string, Texture2D> tiles = new Dictionary<string, Texture2D>();

    public static Texture2D Render(int worldSeed, string islandId, IslandArchetype arch)
    {
        string key = worldSeed + "|" + islandId;
        Texture2D t;
        if (cache.TryGetValue(key, out t)) return t;
        IslandVisual v = VisualDNA.Island(worldSeed, islandId, arch);
        Color[] b = Compose(worldSeed, islandId, arch, v);
        t = Pixel.ToTexture(b, W, H);
        cache[key] = t;
        return t;
    }

    static Texture2D Tex(string name)
    {
        Texture2D t;
        if (tiles.TryGetValue(name, out t)) return t;
        t = Resources.Load<Texture2D>("Art/kenney/tiles/" + name);
        tiles[name] = t;
        return t;
    }

    static Color[] PxOf(Texture2D t)
    {
        if (t == null || t.width != T || t.height != T) return null;
        return t.GetPixels();
    }

    // Blit de tile 16x16 em coords yTop (topo do tile). GetPixels: linha 0 = base.
    static void Blit(Color[] b, Color[] s, int dx, int dyTop)
    {
        if (s == null) return;
        for (int sy = 0; sy < T; sy++)
        {
            for (int sx = 0; sx < T; sx++)
            {
                Color c = s[sy * T + sx];
                if (c.a < 0.5f) continue;
                int x = dx + sx;
                int yTop = dyTop + (T - 1 - sy);
                if (x < 0 || x >= W || yTop < 0 || yTop >= H) continue;
                b[(H - 1 - yTop) * W + x] = c;
            }
        }
    }

    static int HashStr(string s)
    {
        int h = 7;
        for (int i = 0; i < s.Length; i++) h = h * 31 + s[i];
        return h < 0 ? -h : h;
    }

    static string KindTile(string kind)
    {
        if (kind == "port" || kind == "dock") return "dock";
        if (kind == "market") return "barrel";
        if (kind == "house" || kind == "suburb") return "house";
        if (kind == "farm" || kind == "field") return "dirt";
        if (kind == "forest") return "tree";
        if (kind == "trees") return "palm";
        if (kind == "castle") return "castle";
        if (kind == "plaza") return "sand_fill";
        if (kind == "beach") return "sand_fill";
        if (kind == "church") return "cross";
        if (kind == "gov") return "hut";
        if (kind == "warehouse") return "chest";
        if (kind == "tavern") return "bottle";
        if (kind == "ruins") return "rock";
        if (kind == "cave") return "cave";
        if (kind == "boss") return "skull";
        return null;
    }

    static float BlobR(float wx, float wy, float rad, float ph1, float ph2, float ph3)
    {
        float th = (float)System.Math.Atan2(wy / 0.82f, wx);
        float edge = 1f + 0.32f * (float)System.Math.Sin(2 * th + ph1)
            + 0.20f * (float)System.Math.Sin(3 * th + ph2)
            + 0.12f * (float)System.Math.Sin(5 * th + ph3);
        float ux = wx / rad;
        float uy = wy / (rad * 0.82f);
        float r = (float)System.Math.Sqrt(ux * ux + uy * uy);
        return r / edge;
    }

    static bool IsSettlement(string kind)
    {
        return kind == "house" || kind == "suburb" || kind == "castle"
            || kind == "market" || kind == "warehouse" || kind == "tavern"
            || kind == "church" || kind == "gov" || kind == "plaza";
    }

    static Color[] Compose(int worldSeed, string islandId, IslandArchetype arch, IslandVisual v)
    {
        Color[] sea = PxOf(Tex("sea"));
        Color[] sand = PxOf(Tex("sand"));
        Color[] grass = PxOf(Tex("grass"));
        if (sea == null || sand == null || grass == null) return Fallback(v);
        Color[] b = new Color[W * H];
        Pixel.Fill(b, W, H, new Color(0, 0, 0, 0));
        float hw = 44 + (v.silhouette % 2) * 6;
        bool[,] isGrass = new bool[N, N];
        bool[,] busy = new bool[N, N];
        Color cSea = new Color(92f / 255f, 89f / 255f, 124f / 255f);
        Color cSand = new Color(142f / 255f, 139f / 255f, 183f / 255f);
        Color cGrass = new Color(106f / 255f, 116f / 255f, 141f / 255f);
        // Blob organico: raio com ruido angular por seed (nada de diamante).
        int hh2 = HashStr(worldSeed + ":" + islandId + ":blob");
        float ph1 = (hh2 % 628) / 100f;
        float ph2 = ((hh2 / 7) % 628) / 100f;
        float ph3 = ((hh2 / 13) % 628) / 100f;
        float rad = 40 + (v.silhouette % 3) * 4;
        // grade de terra + anel de areia com bitmask (agua vizinha)
        bool[,] land = new bool[N, N];
        float[,] rrm = new float[N, N];
        for (int qy = 0; qy < N; qy++)
        {
            for (int qx = 0; qx < N; qx++)
            {
                float wx = qx * T + 8 - 64f;
                float wy = qy * T + 8 - 98f;
                float rr = BlobR(wx, wy, rad, ph1, ph2, ph3);
                rrm[qx, qy] = rr;
                if (rr < 1f) land[qx, qy] = true;
            }
        }
        Color[] sandFill = PxOf(Tex("sand_fill"));
        for (int qy = 0; qy < N; qy++)
        {
            for (int qx = 0; qx < N; qx++)
            {
                if (!land[qx, qy]) continue;
                if (sandFill != null) Blit(b, sandFill, qx * T, qy * T);
                else
                {
                    for (int sy = 0; sy < T; sy++)
                    {
                        for (int sx = 0; sx < T; sx++)
                        {
                            int x = qx * T + sx;
                            int yTop = qy * T + sy;
                            b[(H - 1 - yTop) * W + x] = cSand;
                        }
                    }
                }
                if (rrm[qx, qy] < 0.55f) isGrass[qx, qy] = true;
            }
        }
        // interior de grama chapada por pixel (costa suave com o anel)
        for (int yTop = 0; yTop < H; yTop++)
        {
            for (int x = 0; x < W; x++)
            {
                float wx = x - 64f;
                float wy = (float)yTop - 98f;
                float rr = BlobR(wx, wy, rad, ph1, ph2, ph3);
                if (rr < 0.55f) b[(H - 1 - yTop) * W + x] = cGrass;
            }
        }
        // borda do anel: tile de transicao conforme agua vizinha
        for (int qy = 0; qy < N; qy++)
        {
            for (int qx = 0; qx < N; qx++)
            {
                if (!land[qx, qy] || isGrass[qx, qy]) continue;
                bool wN = qy == 0 || !land[qx, qy - 1];
                bool wS = qy == N - 1 || !land[qx, qy + 1];
                bool wW = qx == 0 || !land[qx - 1, qy];
                bool wE = qx == N - 1 || !land[qx + 1, qy];
                string tn = "sand_fill";
                int n = (wN ? 1 : 0) + (wS ? 1 : 0) + (wW ? 1 : 0) + (wE ? 1 : 0);
                if (n == 1)
                {
                    if (wN) tn = "sand_t";
                    else if (wS) tn = "sand_b";
                    else if (wW) tn = "sand_l";
                    else tn = "sand_r";
                }
                else if (n == 2)
                {
                    if (wN && wW) tn = "sand_tl";
                    else if (wN && wE) tn = "sand_tr";
                    else if (wS && wW) tn = "sand_bl";
                    else if (wS && wE) tn = "sand_br";
                }
                Color[] tp = PxOf(Tex(tn));
                if (tp == null) continue;
                Blit(b, tp, qx * T, qy * T);
            }
        }
        for (int i = 0; i < v.pieces.Count; i++)
        {
            IslandPiece p = v.pieces[i];
            Color[] tp = PxOf(Tex(KindTile(p.kind) != null ? KindTile(p.kind) : "rock"));
            if (tp == null) continue;
            float fpx = p.x * W;
            float fpy = p.y * H;
            for (int k = 0; k < 4; k++)
            {
                float wx = fpx - 64f;
                float wy = fpy - 98f;
                if (BlobR(wx, wy, rad, ph1, ph2, ph3) < 0.95f) break;
                fpx += (64f - fpx) * 0.25f;
                fpy += (98f - fpy) * 0.25f;
            }
            int dx = (int)fpx - 8;
            int dyTop = (int)fpy - 8;
            if (IsSettlement(p.kind))
            {
                Color[] dirt = PxOf(Tex("dirt"));
                if (dirt != null)
                {
                    int bx0 = (dx + 8) / T, by0 = (dyTop + 8) / T;
                    Blit(b, dirt, (bx0 - 1) * T, (by0 - 1) * T);
                    Blit(b, dirt, bx0 * T, (by0 - 1) * T);
                    Blit(b, dirt, (bx0 - 1) * T, by0 * T);
                    Blit(b, dirt, bx0 * T, by0 * T);
                }
            }
            Blit(b, tp, dx, dyTop);
            int bx = (dx + 8) / T, by = (dyTop + 8) / T;
            if (bx >= 0 && bx < N && by >= 0 && by < N) busy[bx, by] = true;
        }
        System.Random rng = new System.Random(HashStr(worldSeed + ":" + islandId));
        string[] acc = new string[] { "palm", "palm", "tree", "tree", "rock", "hill", "chest" };
        if (arch == IslandArchetype.Dangerous) acc = new string[] { "palm", "tree", "rock", "hill", "chest", "skull", "skull" };
        int nAcc = 4 + v.silhouette + (arch == IslandArchetype.Dangerous ? 2 : 0);
        List<int[]> free = new List<int[]>();
        for (int qy = 0; qy < N; qy++)
        {
            for (int qx = 0; qx < N; qx++)
            {
                if (isGrass[qx, qy] && !busy[qx, qy]) free.Add(new int[] { qx, qy });
            }
        }
        for (int i = 0; i < nAcc && free.Count > 0; i++)
        {
            int k = rng.Next(free.Count);
            int[] cell = free[k];
            free.RemoveAt(k);
            Color[] tp = PxOf(Tex(acc[rng.Next(acc.Length)]));
            if (tp == null) continue;
            Blit(b, tp, cell[0] * T, cell[1] * T);
        }
        // marcas d'agua esparsas no mar (como na referencia)
        List<int[]> seaCells = new List<int[]>();
        for (int qy = 0; qy < N; qy++)
        {
            for (int qx = 0; qx < N; qx++)
            {
                float wx = qx * T + 8 - 64f;
                float wy = qy * T + 8 - 98f;
                float rr = BlobR(wx, wy, rad, ph1, ph2, ph3);
                if (rr >= 1f && rr < 2.2f) seaCells.Add(new int[] { qx, qy });
            }
        }
        for (int i = 0; i < 8 && seaCells.Count > 0; i++)
        {
            int k = rng.Next(seaCells.Count);
            int[] cell = seaCells[k];
            seaCells.RemoveAt(k);
            Color[] tp = PxOf(Tex(rng.Next(2) == 0 ? "water1" : "water2"));
            if (tp == null) continue;
            Blit(b, tp, cell[0] * T, cell[1] * T);
        }
        // contorno escuro na beira (como nas refs) em vez de espuma clara
        Color edgeCol = new Color(40f / 255f, 40f / 255f, 80f / 255f);
        Color sandRef = new Color(142f / 255f, 139f / 255f, 183f / 255f);
        for (int yTop = 0; yTop < H; yTop++)
        {
            for (int x = 0; x < W; x++)
            {
                Color c = b[(H - 1 - yTop) * W + x];
                if (c.a <= 0.5f) continue;
                if (System.Math.Abs(c.r - sandRef.r) + System.Math.Abs(c.g - sandRef.g) + System.Math.Abs(c.b - sandRef.b) > 0.24f) continue;
                bool edge = false;
                if (x > 0 && b[(H - 1 - yTop) * W + x - 1].a <= 0.5f) edge = true;
                if (x < W - 1 && b[(H - 1 - yTop) * W + x + 1].a <= 0.5f) edge = true;
                if (yTop > 0 && b[(H - yTop) * W + x].a <= 0.5f) edge = true;
                if (yTop < H - 1 && b[(H - 2 - yTop) * W + x].a <= 0.5f) edge = true;
                if (edge) b[(H - 1 - yTop) * W + x] = edgeCol;
            }
        }
        return b;
    }

    static Color[] Fallback(IslandVisual v)
    {
        Color[] b = new Color[W * H];
        Pixel.Fill(b, W, H, new Color(0, 0, 0, 0));
        Color grass = new Color(0.45f, 0.55f, 0.70f);
        Color sand = new Color(0.60f, 0.62f, 0.75f);
        Color[] g = new Color[T * T];
        Color[] s = new Color[T * T];
        for (int i = 0; i < T * T; i++) { g[i] = grass; s[i] = sand; }
        float rad = 40 + (v.silhouette % 3) * 4;
        for (int qy = 0; qy < N; qy++)
        {
            for (int qx = 0; qx < N; qx++)
            {
                float wx = qx * T + 8 - 64f;
                float wy = qy * T + 8 - 98f;
                float rr = BlobR(wx, wy, rad, 0.5f, 1.7f, 2.9f);
                if (rr > 1f) continue;
                Blit(b, rr < 0.62f ? g : s, qx * T, qy * T);
            }
        }
        return b;
    }
}
