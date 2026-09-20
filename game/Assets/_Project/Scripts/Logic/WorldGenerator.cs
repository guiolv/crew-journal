// CrewJournal — geracao deterministica do mundo (TDD secoes 7-8, GDD secao 12).
using System;

namespace CrewJournal.Logic
{
    public static class WorldGenerator
    {
        public static WorldData Generate(int seed)
        {
            Random rng = new Random(seed);
            WorldData world = new WorldData();
            world.seed = seed;
            world.day = 1;

            IslandArchetype[] archs = new IslandArchetype[]
            {
                IslandArchetype.FishingVillage,
                IslandArchetype.TradingPort,
                IslandArchetype.Capital,
                IslandArchetype.Uninhabited,
                IslandArchetype.Dangerous
            };
            int[] dangers = new int[] { 1, 2, 2, 3, 5 };

            double angleStep = Math.PI * 2.0 / archs.Length;
            for (int i = 0; i < archs.Length; i++)
            {
                IslandData isl = new IslandData();
                isl.id = "isl_" + i.ToString();
                isl.displayName = Balance.IslandNames[rng.Next(Balance.IslandNames.Length)] + " " + (i + 1).ToString();
                isl.archetype = archs[i];
                double a = angleStep * i + (rng.NextDouble() - 0.5) * 0.4;
                double r = 30.0 + rng.NextDouble() * 20.0;
                isl.x = (float)(Math.Cos(a) * r);
                isl.y = (float)(Math.Sin(a) * r);
                isl.danger = dangers[i];
                ApplyPrices(isl, rng);
                world.islands.Add(isl);
            }
            return world;
        }

        private static void ApplyPrices(IslandData isl, Random rng)
        {
            float[] mults = BaseMults(isl.archetype);
            ResourceId[] ids = new ResourceId[]
            {
                ResourceId.Food, ResourceId.Water, ResourceId.Wood,
                ResourceId.Metal, ResourceId.Medicine
            };
            for (int i = 0; i < ids.Length; i++)
            {
                float jitter = 1.0f + ((float)rng.NextDouble() - 0.5f) * 0.1f;
                PriceEntry p = new PriceEntry();
                p.id = ids[i];
                p.mult = mults[i] * jitter;
                isl.prices.Add(p);
            }
        }

        private static float[] BaseMults(IslandArchetype arch)
        {
            // ordem: food, water, wood, metal, medicine (GED secao 11)
            switch (arch)
            {
                case IslandArchetype.FishingVillage:
                    return new float[] { 0.6f, 0.8f, 1.1f, 1.4f, 1.2f };
                case IslandArchetype.TradingPort:
                    return new float[] { 0.9f, 0.9f, 1.0f, 1.0f, 1.1f };
                case IslandArchetype.Capital:
                    return new float[] { 1.0f, 1.0f, 1.0f, 0.9f, 0.9f };
                case IslandArchetype.Uninhabited:
                    return new float[] { 1.3f, 1.3f, 0.8f, 1.2f, 1.5f };
                default:
                    return new float[] { 1.5f, 1.5f, 1.2f, 0.8f, 1.3f };
            }
        }

        public static float IslandMult(IslandData isl, ResourceId id)
        {
            for (int i = 0; i < isl.prices.Count; i++)
            {
                if (isl.prices[i].id == id)
                {
                    return isl.prices[i].mult;
                }
            }
            return 1.0f;
        }
    }
}
