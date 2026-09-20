// CrewJournal — missoes (TDD secao 20, GED secao 12). Geracao deterministica.
using System;

namespace CrewJournal.Logic
{
    public static class MissionSystem
    {
        public static MissionData Generate(int worldSeed, int index, WorldData world)
        {
            Random rng = new Random(worldSeed * 7919 + index * 131 + 17);
            MissionData m = new MissionData();
            m.id = "mis_" + index.ToString();
            int t = rng.Next(5);
            m.type = (MissionType)t;
            m.status = MissionStatus.Available;
            int a = rng.Next(world.islands.Count);
            int b = rng.Next(world.islands.Count);
            if (b == a) b = (b + 1) % world.islands.Count;
            m.fromIslandId = world.islands[a].id;
            m.targetIslandId = world.islands[b].id;
            m.danger = 1 + rng.Next(5);
            m.reward = RewardFor(m.type, rng);
            m.title = TitleFor(m, world);
            return m;
        }

        private static int RewardFor(MissionType t, Random rng)
        {
            switch (t)
            {
                case MissionType.Transport: return 100 + rng.Next(151);
                case MissionType.Trade: return 50 + rng.Next(251);
                case MissionType.Rescue: return 100 + rng.Next(301);
                case MissionType.Combat: return 100 + rng.Next(401);
                default: return 80 + rng.Next(250);
            }
        }

        private static string TitleFor(MissionData m, WorldData world)
        {
            string target = m.targetIslandId;
            for (int i = 0; i < world.islands.Count; i++)
            {
                if (world.islands[i].id == m.targetIslandId)
                {
                    target = world.islands[i].displayName;
                }
            }
            return m.type.ToString() + ": " + target;
        }

        // Transporte/exploracao completam ao chegar na ilha alvo.
        public static bool CheckArrivalComplete(MissionData m, string arrivedIslandId)
        {
            if (m.status != MissionStatus.Accepted && m.status != MissionStatus.InProgress)
            {
                return false;
            }
            if (m.type == MissionType.Transport || m.type == MissionType.Explore || m.type == MissionType.Trade)
            {
                return m.targetIslandId == arrivedIslandId;
            }
            return false;
        }
    }
}
