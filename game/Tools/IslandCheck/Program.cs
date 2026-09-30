// Diagnostico offline do compose: replica base+anel com System.Drawing.
// Uso: csc ... (ver comando no vault). NAO vai para Assets (Unity ignoraria? Tools/ fora de Assets).
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CrewJournal.Logic;

public static class IslandCheck
{
    const int T = 16, N = 8, W = 128, H = 128;
    static string Art = "C:/Users/guilh/Documents/OPENCODE/game/Assets/Resources/Art/kenney/tiles/";

    static float BlobR(float wx, float wy, float rad, float ph1, float ph2, float ph3)
    {
        float th = (float)Math.Atan2(wy / 0.82f, wx);
        float edge = 1f + 0.32f * (float)Math.Sin(2 * th + ph1)
            + 0.20f * (float)Math.Sin(3 * th + ph2)
            + 0.12f * (float)(Math.Sin(5 * th + ph3));
        float ux = wx / rad;
        float uy = wy / (rad * 0.82f);
        return (float)Math.Sqrt(ux * ux + uy * uy) / edge;
    }

    static int HashStr(string s)
    {
        int h = 7;
        for (int i = 0; i < s.Length; i++) h = h * 31 + s[i];
        return h < 0 ? -h : h;
    }

    static Bitmap Load(string n)
    {
        string p = Art + n + ".png";
        if (!File.Exists(p)) { Console.WriteLine("MISSING " + n); return null; }
        return new Bitmap(p);
    }

    public static int Main()
    {
        IslandArchetype[] archs = new IslandArchetype[] {
            IslandArchetype.FishingVillage, IslandArchetype.TradingPort,
            IslandArchetype.Capital, IslandArchetype.Uninhabited, IslandArchetype.Dangerous };
        Dictionary<string, int> use = new Dictionary<string, int>();
        for (int a = 0; a < archs.Length; a++)
        {
            int seed = 4242;
            string islandId = "isl_" + a;
            IslandVisual v = VisualDNA.Island(seed, islandId, archs[a]);
            int hh = HashStr(seed + ":" + islandId + ":blob");
            float ph1 = (hh % 628) / 100f, ph2 = ((hh / 7) % 628) / 100f, ph3 = ((hh / 13) % 628) / 100f;
            float rad = 40 + (v.silhouette % 3) * 4;
            Bitmap bmp = new Bitmap(W, H);
            bool[,] land = new bool[N, N];
            float[,] rrm = new float[N, N];
            for (int qy = 0; qy < N; qy++)
                for (int qx = 0; qx < N; qx++)
                {
                    float rr = BlobR(qx * T + 8 - 64f, qy * T + 8 - 98f, rad, ph1, ph2, ph3);
                    rrm[qx, qy] = rr;
                    if (rr < 1f) land[qx, qy] = true;
                }
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                for (int qy = 0; qy < N; qy++)
                    for (int qx = 0; qx < N; qx++)
                    {
                        if (!land[qx, qy]) continue;
                        float rr = rrm[qx, qy];
                        if (rr < 0.55f) continue; // grama (pintada depois)
                        bool wN = qy == 0 || !land[qx, qy - 1];
                        bool wS = qy == N - 1 || !land[qx, qy + 1];
                        bool wW = qx == 0 || !land[qx - 1, qy];
                        bool wE = qx == N - 1 || !land[qx + 1, qy];
                        string tn = "sand_fill";
                        int n = (wN ? 1 : 0) + (wS ? 1 : 0) + (wW ? 1 : 0) + (wE ? 1 : 0);
                        if (n == 1)
                        {
                            if (wN) tn = "sand_t"; else if (wS) tn = "sand_b";
                            else if (wW) tn = "sand_l"; else tn = "sand_r";
                        }
                        else if (n == 2)
                        {
                            if (wN && wW) tn = "sand_tl"; else if (wN && wE) tn = "sand_tr";
                            else if (wS && wW) tn = "sand_bl"; else if (wS && wE) tn = "sand_br";
                        }
                        if (!use.ContainsKey(tn)) use[tn] = 0;
                        use[tn]++;
                    }
            }
            bmp.Save("C:/Users/guilh/AppData/Local/Temp/opencode/ring_" + archs[a] + ".png");
        }
        Console.WriteLine("== tile usage (ring) ==");
        foreach (var kv in use) Console.WriteLine(kv.Key + ": " + kv.Value);
        return 0;
    }
}
