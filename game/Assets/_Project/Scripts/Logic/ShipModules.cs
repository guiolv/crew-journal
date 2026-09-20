// CrewJournal — modulos do navio (GDD secao 22, refs tela Navio).
using System;

namespace CrewJournal.Logic
{
    public static class ShipModules
    {
        public static readonly string[] Types = new string[] { "kitchen", "dorm", "cargo", "equip", "special" };

        public static string NameOf(string t)
        {
            if (t == "kitchen") return "Cozinha";
            if (t == "dorm") return "Dormitorio";
            if (t == "cargo") return "Carga";
            if (t == "equip") return "Equipamento";
            return "Especial";
        }

        public static System.Collections.Generic.List<ModuleEntry> ModsOf(ShipData s)
        {
            if (s.mods == null) s.mods = new System.Collections.Generic.List<ModuleEntry>();
            return s.mods;
        }

        public static int CountOf(ShipData s, string t)
        {
            System.Collections.Generic.List<ModuleEntry> m = ModsOf(s);
            for (int i = 0; i < m.Count; i++)
            {
                if (m[i].type == t) return m[i].count;
            }
            return 0;
        }

        public static int UsedSlots(ShipData s)
        {
            int n = 0;
            System.Collections.Generic.List<ModuleEntry> m = ModsOf(s);
            for (int i = 0; i < m.Count; i++) n += m[i].count;
            return n;
        }

        public static int WoodCost(string t)
        {
            if (t == "kitchen") return 4;
            if (t == "dorm") return 5;
            if (t == "cargo") return 3;
            if (t == "equip") return 0;
            return 0;
        }

        public static int MoneyCost(string t)
        {
            if (t == "kitchen") return 120;
            if (t == "dorm") return 150;
            if (t == "cargo") return 100;
            if (t == "equip") return 200;
            return 300;
        }

        public static int MetalCost(string t)
        {
            if (t == "equip") return 2;
            if (t == "special") return 3;
            return 0;
        }

        // Efeitos: cozinha -15% comida/dia cada (min 50%); dorm +2 trip cada; carga +4 limite cada;
        // equip +1 atk de todos em combate cada (max 2); especial +10 casco max cada.
        public static float FoodFactor(ShipData s)
        {
            float f = 1.0f - 0.15f * CountOf(s, "kitchen");
            if (f < 0.5f) f = 0.5f;
            return f;
        }

        public static int EffectiveCrewCap(ShipData s)
        {
            return s.crewCap + CountOf(s, "dorm") * 2;
        }

        public static int EffectiveCargoCap(ShipData s)
        {
            return s.cargoCap + CountOf(s, "cargo") * 4;
        }

        public static int EquipBonus(ShipData s)
        {
            int b = CountOf(s, "equip");
            if (b > 2) b = 2;
            return b;
        }

        public static bool Buy(GameData d, string t, out string msg)
        {
            if (Array.IndexOf(Types, t) < 0) { msg = "Modulo invalido."; return false; }
            if (d.ship.defId == "barrel") { msg = "O barril nao suporta modulos."; return false; }
            if (UsedSlots(d.ship) >= d.ship.modules) { msg = "Sem slots livres (" + d.ship.modules + ")."; return false; }
            int wood = WoodCost(t), money = MoneyCost(t), metal = MetalCost(t);
            if (d.GetResource(ResourceId.Money) < money) { msg = "Custa " + money + " moedas."; return false; }
            if (d.GetResource(ResourceId.Wood) < wood) { msg = "Exige " + wood + " madeira."; return false; }
            if (d.GetResource(ResourceId.Metal) < metal) { msg = "Exige " + metal + " metal."; return false; }
            d.AddResource(ResourceId.Money, -money);
            d.AddResource(ResourceId.Wood, -wood);
            d.AddResource(ResourceId.Metal, -metal);
            System.Collections.Generic.List<ModuleEntry> m = ModsOf(d.ship);
            bool found = false;
            for (int i = 0; i < m.Count; i++)
            {
                if (m[i].type == t) { m[i].count++; found = true; }
            }
            if (!found)
            {
                ModuleEntry e = new ModuleEntry();
                e.type = t;
                e.count = 1;
                m.Add(e);
            }
            if (t == "special") { d.ship.maxHull += 10; d.ship.hull += 10; }
            GameSession.AddJournal(d, "Modulo instalado: " + NameOf(t) + ".");
            msg = NameOf(t) + " instalado!";
            return true;
        }
    }
}
