// CrewJournal — exporta visuais procedurais em PNG para inspecao (vault/07-REFERENCIAS/preview).
// Uso: Unity -batchmode -nographics -quit -projectPath ./game -executeMethod CrewJournal.Editor.V1VisualExport.ExportAll -logFile -
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CrewJournal.Logic;

namespace CrewJournal.Editor
{
#if UNITY_EDITOR
    public static class V1VisualExport
    {
        public static void ExportAll()
        {
            string dir = Path.GetFullPath(Application.dataPath + "/../../vault/07-REFERENCIAS/preview");
            Directory.CreateDirectory(dir);
            string[] jobs = new string[] { "Navegador", "Cozinheiro", "Medico", "Carpinteiro", "Combatente", "Atirador" };
            for (int i = 0; i < jobs.Length; i++)
            {
                List<string> tr = new List<string>();
                tr.Add("corajoso");
                Texture2D t = PortraitRenderer.Render(4242, "char_" + i, jobs[i], tr, i == 4, 0, i == 4);
                File.WriteAllBytes(dir + "/portrait_" + jobs[i] + ".png", t.EncodeToPNG());
                File.WriteAllBytes(dir + "/big_portrait_" + jobs[i] + ".png", Scale4(t).EncodeToPNG());
            }
            for (int e = 0; e < 3; e++)
            {
                Texture2D t = PortraitRenderer.RenderEnemy(4242, e, 0);
                File.WriteAllBytes(dir + "/enemy_" + e + ".png", t.EncodeToPNG());
            }
            IslandArchetype[] archs = new IslandArchetype[] {
                IslandArchetype.FishingVillage, IslandArchetype.TradingPort, IslandArchetype.Capital,
                IslandArchetype.Uninhabited, IslandArchetype.Dangerous };
            for (int i = 0; i < archs.Length; i++)
            {
                Texture2D t = IslandRenderer.Render(4242, "isl_" + i, archs[i]);
                File.WriteAllBytes(dir + "/island_" + archs[i].ToString() + ".png", t.EncodeToPNG());
            }
            string[] ships = new string[] { "barrel", "boat", "medium" };
            for (int i = 0; i < ships.Length; i++)
            {
                Texture2D t = ShipRenderer.Render(ships[i], 4242, 1);
                File.WriteAllBytes(dir + "/ship_" + ships[i] + ".png", t.EncodeToPNG());
            }
            Debug.Log("[V1VisualExport] PNGs em " + dir);
        }

        static Texture2D Scale4(Texture2D t)
        {
            int W = t.width * 4, H = t.height * 4;
            Texture2D o = new Texture2D(W, H, TextureFormat.RGBA32, false);
            o.filterMode = FilterMode.Point;
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    o.SetPixel(x, y, t.GetPixel(x / 4, y / 4));
                }
            }
            o.Apply();
            return o;
        }
    }
#endif
}
