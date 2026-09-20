// CrewJournal — DNA visual procedural (GDD-31, TDD-36).
// DNA = funcao pura de (worldSeed, id). Estado mutavel minimo: scar/outfitMod no DTO.
using System;
using System.Collections.Generic;

namespace CrewJournal.Logic
{
    public class CharacterVisual
    {
        public int body;       // 0-7
        public int face;       // 0-11
        public int hair;       // 0-14
        public int hairColor;  // 0-5
        public int skinTone;   // 0-4
        public string outfit;  // job_xx
        public int accessory;  // 0-7 (0 none,1 hat,2 bandana,3 glasses,4 earring,5 necklace,6 pipe,7 hood)
        public int beard;      // 0-2
        public bool scar;
    }

    public class IslandPiece
    {
        public string kind;
        public float x; // 0-1 relativo
        public float y;
    }

    public class IslandVisual
    {
        public string biome; // Tropical, Arid, Temperate, Cold
        public int silhouette;
        public List<IslandPiece> pieces = new List<IslandPiece>();
    }

    public class ShipVisual
    {
        public int hull;   // 0-2
        public int masts;  // 1-2
        public int sail;   // 0-3
        public int cannon; // 0-2
        public int decor;  // 0-2
    }

    public static class VisualDNA
    {
        public static int StableHash(string s)
        {
            int h = 7;
            for (int i = 0; i < s.Length; i++) h = h * 31 + s[i];
            return Math.Abs(h);
        }

        public static int IndexOfId(string id)
        {
            int u = id.LastIndexOf('_');
            int n;
            if (u >= 0 && int.TryParse(id.Substring(u + 1), out n)) return n;
            return StableHash(id) % 1000;
        }

        // ---- personagens ----
        public static CharacterVisual Character(int worldSeed, string charId, string mainJob, List<string> traits, bool scar, int outfitMod)
        {
            int h = StableHash(worldSeed + ":" + charId);
            Random rng = new Random(h);
            CharacterVisual v = new CharacterVisual();
            v.body = h % 8;
            v.face = (h / 8) % 12;
            v.skinTone = (h / 96) % 5;
            v.hairColor = (h / 480) % 6;
            v.hair = WeightedHair(rng, mainJob);
            v.beard = WeightedBeard(rng, mainJob);
            v.accessory = WeightedAccessory(rng, mainJob, traits);
            v.outfit = OutfitCode(mainJob, outfitMod);
            v.scar = scar || rng.NextDouble() < ScarChance(mainJob);
            return v;
        }

        private static int WeightedHair(Random rng, string job)
        {
            // 0 curto,1 longo,2 careca+bandana,3 rabo,4 baguncado; +5..14 variantes mapeadas no renderer (estilo = %5)
            int style = rng.Next(5);
            if (job == "Combatente" && rng.NextDouble() < 0.4) style = 2;
            if (job == "Medico" && rng.NextDouble() < 0.4) style = 0;
            return style + rng.Next(3) * 5;
        }

        private static int WeightedBeard(Random rng, string job)
        {
            double p = 0.25;
            if (job == "Navegador") p = 0.55;
            if (job == "Combatente") p = 0.45;
            if (job == "Medico") p = 0.1;
            if (rng.NextDouble() > p) return 0;
            return rng.Next(2) + 1;
        }

        private static int WeightedAccessory(Random rng, string job, List<string> traits)
        {
            double r = rng.NextDouble();
            if (job == "Medico" && r < 0.5) return 3;      // oculos
            if (job == "Navegador" && r < 0.35) return 4;  // brinco
            if (job == "Combatente" && r < 0.4) return 2;  // bandana
            if (job == "Cozinheiro" && r < 0.4) return 1;  // chapeu
            if (job == "Atirador" && r < 0.3) return 7;    // capuz
            if (traits != null && traits.Contains("ambicioso") && r < 0.5) return 5; // colar
            if (r < 0.2) return 6; // cachimbo
            int[] pool = new int[] { 0, 0, 1, 2, 3, 4, 5 };
            return pool[rng.Next(pool.Length)];
        }

        private static double ScarChance(string job)
        {
            if (job == "Combatente") return 0.35;
            if (job == "Atirador") return 0.2;
            return 0.1;
        }

        public static string OutfitCode(string job, int mod)
        {
            string base_ = "sailor";
            if (job == "Navegador") base_ = "navigator";
            else if (job == "Cozinheiro") base_ = "cook";
            else if (job == "Medico") base_ = "medic";
            else if (job == "Carpinteiro") base_ = "carpenter";
            else if (job == "Combatente") base_ = "fighter";
            else if (job == "Atirador") base_ = "shooter";
            return base_ + "_" + (1 + Math.Abs(mod) % 3).ToString();
        }

        // ---- ilhas ----
        public static IslandVisual Island(int worldSeed, string islandId, IslandArchetype arch)
        {
            int idx = IndexOfId(islandId);
            Random rng = new Random(worldSeed * 31 + idx * 1013 + 5);
            IslandVisual v = new IslandVisual();
            double temp = rng.NextDouble();
            double hum = rng.NextDouble();
            if (temp > 0.6 && hum > 0.45) v.biome = "Tropical";
            else if (temp > 0.6) v.biome = "Arid";
            else if (temp < 0.3) v.biome = "Cold";
            else v.biome = "Temperate";
            v.silhouette = rng.Next(4);
            foreach (Zone z in Grammar(arch))
            {
                int n = z.count;
                for (int i = 0; i < n; i++)
                {
                    IslandPiece p = new IslandPiece();
                    p.kind = z.kind;
                    p.x = Clamp01(z.x + (rng.NextDouble() - 0.5) * z.jitter);
                    p.y = Clamp01(z.y + (rng.NextDouble() - 0.5) * z.jitter);
                    v.pieces.Add(p);
                }
            }
            return v;
        }

        private static float Clamp01(double v)
        {
            if (v < 0.03) return 0.03f;
            if (v > 0.97) return 0.97f;
            return (float)v;
        }

        private class Zone
        {
            public string kind;
            public float x;
            public float y;
            public int count;
            public float jitter;
        }

        private static Zone Z(string k, float x, float y, int c, float j)
        {
            Zone z = new Zone();
            z.kind = k;
            z.x = x;
            z.y = y;
            z.count = c;
            z.jitter = j;
            return z;
        }

        // Gramatica por arquetipo: porto ao sul (y~0.85), norte em cima (y~0.15).
        private static List<Zone> Grammar(IslandArchetype arch)
        {
            List<Zone> l = new List<Zone>();
            switch (arch)
            {
                case IslandArchetype.FishingVillage:
                    l.Add(Z("port", 0.5f, 0.85f, 1, 0.05f));
                    l.Add(Z("market", 0.5f, 0.65f, 1, 0.05f));
                    l.Add(Z("house", 0.35f, 0.5f, 2, 0.15f));
                    l.Add(Z("house", 0.65f, 0.5f, 2, 0.15f));
                    l.Add(Z("farm", 0.3f, 0.3f, 1, 0.1f));
                    l.Add(Z("farm", 0.7f, 0.3f, 1, 0.1f));
                    l.Add(Z("forest", 0.5f, 0.12f, 2, 0.2f));
                    break;
                case IslandArchetype.TradingPort:
                    l.Add(Z("port", 0.5f, 0.85f, 1, 0.05f));
                    l.Add(Z("warehouse", 0.3f, 0.7f, 1, 0.08f));
                    l.Add(Z("warehouse", 0.7f, 0.7f, 1, 0.08f));
                    l.Add(Z("market", 0.5f, 0.55f, 1, 0.05f));
                    l.Add(Z("house", 0.35f, 0.4f, 2, 0.12f));
                    l.Add(Z("house", 0.65f, 0.4f, 1, 0.12f));
                    l.Add(Z("tavern", 0.5f, 0.3f, 1, 0.08f));
                    break;
                case IslandArchetype.Capital:
                    l.Add(Z("castle", 0.5f, 0.12f, 1, 0.03f));
                    l.Add(Z("plaza", 0.5f, 0.3f, 1, 0.04f));
                    l.Add(Z("market", 0.3f, 0.42f, 1, 0.05f));
                    l.Add(Z("church", 0.55f, 0.42f, 1, 0.05f));
                    l.Add(Z("gov", 0.72f, 0.42f, 1, 0.05f));
                    l.Add(Z("house", 0.3f, 0.6f, 2, 0.12f));
                    l.Add(Z("house", 0.7f, 0.6f, 2, 0.12f));
                    l.Add(Z("port", 0.45f, 0.85f, 1, 0.05f));
                    l.Add(Z("suburb", 0.6f, 0.75f, 1, 0.08f));
                    break;
                case IslandArchetype.Uninhabited:
                    l.Add(Z("beach", 0.5f, 0.8f, 1, 0.1f));
                    l.Add(Z("forest", 0.35f, 0.5f, 2, 0.15f));
                    l.Add(Z("forest", 0.65f, 0.45f, 2, 0.15f));
                    l.Add(Z("ruins", 0.5f, 0.25f, 1, 0.1f));
                    l.Add(Z("trees", 0.5f, 0.6f, 2, 0.2f));
                    break;
                default: // Dangerous
                    l.Add(Z("beach", 0.5f, 0.85f, 1, 0.08f));
                    l.Add(Z("forest", 0.4f, 0.62f, 2, 0.12f));
                    l.Add(Z("ruins", 0.55f, 0.45f, 1, 0.08f));
                    l.Add(Z("cave", 0.5f, 0.25f, 1, 0.06f));
                    l.Add(Z("boss", 0.5f, 0.12f, 1, 0.04f));
                    break;
            }
            return l;
        }

        // ---- navios ----
        public static ShipVisual Ship(string defId, int worldSeed, int specialMods)
        {
            int h = StableHash(defId + ":" + worldSeed);
            ShipVisual v = new ShipVisual();
            if (defId == "barrel") { v.hull = 0; v.masts = 0; v.sail = 0; v.cannon = 0; v.decor = 0; return v; }
            v.hull = defId == "medium" ? 2 : 1;
            v.masts = defId == "medium" ? 2 : 1;
            v.sail = h % 4;
            v.cannon = defId == "medium" ? 2 : 1;
            v.decor = (h / 4 + specialMods) % 3;
            return v;
        }

        // Inimigos: variante visual por tipo (indice na pool do CombatSystem).
        public static CharacterVisual Enemy(int seed, int typeIdx, int n)
        {
            CharacterVisual v = new CharacterVisual();
            v.body = typeIdx % 8;
            v.face = (typeIdx * 3 + n) % 12;
            v.skinTone = typeIdx % 5;
            v.hairColor = 0;
            v.hair = (typeIdx + n) % 5;
            v.beard = 0;
            v.accessory = 0;
            v.outfit = "enemy_0" + (typeIdx % 3).ToString();
            v.scar = true;
            return v;
        }
    }
}
