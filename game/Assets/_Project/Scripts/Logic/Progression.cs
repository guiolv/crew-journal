// CrewJournal — XP e niveis por funcao (GED secoes 15-16).
using System;

namespace CrewJournal.Logic
{
    public static class Progression
    {
        public static int XpForLevel(int level)
        {
            return 100 * level;
        }

        // Retorna niveis ganhos.
        public static int AddXp(CharacterData c, int amount)
        {
            c.xp += amount;
            int ups = 0;
            while (c.xp >= XpForLevel(c.level))
            {
                c.xp -= XpForLevel(c.level);
                c.level++;
                c.stats.maxHp += 6;
                c.stats.hp = Math.Min(c.stats.maxHp, c.stats.hp + 6);
                c.stats.atk += 1;
                if (c.level % 3 == 0) c.stats.def += 1;
                ups++;
            }
            return ups;
        }

        public static void AddXpBestNav(GameData d, int amount)
        {
            int best = GameSession.BestNavigator(d);
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].alive && d.crew[i].nav == best)
                {
                    int ups = AddXp(d.crew[i], amount);
                    if (ups > 0) GameSession.AddJournal(d, d.crew[i].displayName + " subiu para o nivel " + d.crew[i].level + "!");
                    return;
                }
            }
        }
    }
}
