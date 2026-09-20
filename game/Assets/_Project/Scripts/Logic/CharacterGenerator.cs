// CrewJournal — gerador procedural de personagens (TDD secao 13, GDD 13-16).
using System;

namespace CrewJournal.Logic
{
    public static class CharacterGenerator
    {
        public static CharacterData Generate(int worldSeed, int index)
        {
            Random rng = new Random(worldSeed * 100003 + index * 101 + 7);
            CharacterData c = new CharacterData();
            c.id = "char_" + index.ToString();
            string first = Balance.FirstNames[rng.Next(Balance.FirstNames.Length)];
            string nick = Balance.Nicknames[rng.Next(Balance.Nicknames.Length)];
            c.displayName = first + " \"" + nick + "\"";
            c.age = 18 + rng.Next(30);
            c.level = 1;
            c.nav = Roll(rng, 5, 90);
            c.cook = Roll(rng, 5, 90);
            c.medic = Roll(rng, 5, 90);
            c.carpenter = Roll(rng, 5, 90);
            c.fighter = Roll(rng, 5, 90);
            c.shooter = Roll(rng, 5, 90);
            string mainJob = Balance.Jobs[rng.Next(Balance.Jobs.Length)];
            c.jobs.Add(mainJob);
            BoostForJob(c, mainJob, rng);
            c.traits.Add(Balance.Traits[rng.Next(Balance.Traits.Length)]);
            if (rng.NextDouble() < 0.35)
            {
                c.traits.Add(Balance.Traits[rng.Next(Balance.Traits.Length)]);
            }
            c.stats.maxHp = 30 + c.fighter / 3 + rng.Next(15);
            c.stats.hp = c.stats.maxHp;
            c.stats.atk = 4 + c.fighter / 12 + rng.Next(4);
            c.stats.def = 1 + c.carpenter / 20 + rng.Next(3);
            c.stats.speed = 3 + rng.Next(6);
            c.morale = 50 + rng.Next(30);
            c.loyalty = 30 + rng.Next(40);
            c.battles = 0;
            c.kills = 0;
            c.hireCost = ComputeHireCost(c);
            c.wage = ComputeWage(c);
            return c;
        }

        private static int Roll(Random rng, int min, int max)
        {
            return min + rng.Next(max - min + 1);
        }

        private static void BoostForJob(CharacterData c, string job, Random rng)
        {
            int bonus = 15 + rng.Next(20);
            if (job == "Navegador") c.nav = Math.Min(99, c.nav + bonus);
            else if (job == "Cozinheiro") c.cook = Math.Min(99, c.cook + bonus);
            else if (job == "Medico") c.medic = Math.Min(99, c.medic + bonus);
            else if (job == "Carpinteiro") c.carpenter = Math.Min(99, c.carpenter + bonus);
            else if (job == "Combatente") c.fighter = Math.Min(99, c.fighter + bonus);
            else if (job == "Atirador") c.shooter = Math.Min(99, c.shooter + bonus);
        }

        public static int ComputeHireCost(CharacterData c)
        {
            int best = Math.Max(c.nav, Math.Max(c.fighter, Math.Max(c.shooter, Math.Max(c.cook, Math.Max(c.medic, c.carpenter)))));
            int cost = 50 + best; // GED: 50-250
            if (cost > 250) cost = 250;
            return cost;
        }

        public static int ComputeWage(CharacterData c)
        {
            int best = Math.Max(c.nav, Math.Max(c.fighter, Math.Max(c.shooter, Math.Max(c.cook, Math.Max(c.medic, c.carpenter)))));
            int wage = 5 + best / 8; // GED: 5-20/dia
            if (wage > 20) wage = 20;
            return wage;
        }
    }
}
