// CrewJournal — navegacao (TDD secoes 9-10, GDD 7-10). Deterministica via seed.
using System;

namespace CrewJournal.Logic
{
    public class TravelResult
    {
        public int days;
        public float risk;
        public int foodCost;
        public int waterCost;
        public float eventChance;
        public int quality; // 0 ruim, 1 normal, 2 boa
    }

    public static class NavigationSystem
    {
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static TravelResult Calculate(IslandData from, IslandData to, ShipData ship, int navSkill, int danger)
        {
            TravelResult r = new TravelResult();
            float dist = Distance(from.x, from.y, to.x, to.y);
            float baseDays = dist / 20.0f / Math.Max(0.5f, ship.speed);
            int navBonus = Math.Max(0, Math.Min(90, navSkill));
            float daysF = baseDays * (1.25f - navBonus / 120.0f);
            r.days = Math.Max(1, (int)Math.Ceiling(daysF));
            float riskF = 0.08f + danger * 0.05f + dist / 400.0f - navBonus / 500.0f;
            if (riskF < 0.02f) r.risk = 0.02f;
            else if (riskF > 0.85f) r.risk = 0.85f;
            else r.risk = riskF;
            r.foodCost = r.days * Balance.FoodPerCrewPerDay;
            r.waterCost = r.days * Balance.WaterPerCrewPerDay;
            r.eventChance = 0.25f + r.risk * 0.5f;
            if (r.eventChance > 0.9f) r.eventChance = 0.9f;
            if (navBonus >= 70) r.quality = 2;
            else if (navBonus >= 35) r.quality = 1;
            else r.quality = 0;
            return r;
        }

        public static TravelEventKind RollEvent(Random rng, float eventChance)
        {
            if (rng.NextDouble() > eventChance)
            {
                return TravelEventKind.None;
            }
            double v = rng.NextDouble();
            if (v < 0.22) return TravelEventKind.Storm;
            if (v < 0.40) return TravelEventKind.AbandonedShip;
            if (v < 0.58) return TravelEventKind.UnknownShip;
            if (v < 0.75) return TravelEventKind.SeaCreature;
            if (v < 0.90) return TravelEventKind.Whirlpool;
            return TravelEventKind.CalmBonus;
        }
    }
}
