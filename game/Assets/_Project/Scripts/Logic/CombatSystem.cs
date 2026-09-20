// CrewJournal — combate por turnos (TDD secoes 21-23, GDD 19-20).
// Puro e deterministico: mesma seed + mesmos inputs = mesmo resultado.
using System;
using System.Collections.Generic;

namespace CrewJournal.Logic
{
    public class EnemyData
    {
        public string name;
        public int hp;
        public int maxHp;
        public int atk;
        public int def;
        public int speed;
    }

    public class BattleResult
    {
        public bool victory;
        public bool fled;
        public int rounds;
        public List<string> deadCrewIds = new List<string>();
        public List<string> log = new List<string>();
    }

    public static class CombatSystem
    {
        public static List<EnemyData> GenerateEnemies(int seed, int danger, int day)
        {
            Random rng = new Random(seed + day * 37 + danger * 1001);
            string[] pool = new string[] { "Saqueador", "Arraia Gigante", "Corsario", "Sirena Hostil", "Golem de Coral" };
            int count = 1 + rng.Next(Math.Min(3, danger));
            List<EnemyData> list = new List<EnemyData>();
            for (int i = 0; i < count; i++)
            {
                EnemyData e = new EnemyData();
                e.name = pool[rng.Next(pool.Length)] + " " + (i + 1).ToString();
                e.maxHp = 20 + danger * 6 + rng.Next(12);
                e.hp = e.maxHp;
                e.atk = 3 + danger + rng.Next(4);
                e.def = danger / 2 + rng.Next(3);
                e.speed = 2 + rng.Next(6);
                list.Add(e);
            }
            return list;
        }

        // Acoes automaticas simples para v0.1: cada tripulante ataca o primeiro inimigo vivo.
        // Retorna log + mortes. Dano deterministico via rng com seed.
        public static BattleResult Simulate(List<CharacterData> party, List<EnemyData> enemies, int seed, bool tryFlee)
        {
            Random rng = new Random(seed);
            BattleResult res = new BattleResult();
            if (tryFlee && rng.NextDouble() < 0.45)
            {
                res.fled = true;
                res.log.Add("A tripulacao fugiu da batalha.");
                return res;
            }
            int round = 0;
            while (round < 60)
            {
                round++;
                // turno do jogador
                for (int i = 0; i < party.Count; i++)
                {
                    CharacterData c = party[i];
                    if (!c.alive || c.stats.hp <= 0) continue;
                    EnemyData target = FirstAlive(enemies);
                    if (target == null) break;
                    int dmg = c.stats.atk + c.fighter / 15 + rng.Next(4) - target.def;
                    if (dmg < 1) dmg = 1;
                    target.hp -= dmg;
                    res.log.Add(c.displayName + " causou " + dmg + " em " + target.name + ".");
                }
                if (FirstAlive(enemies) == null)
                {
                    res.victory = true;
                    break;
                }
                // turno inimigo
                for (int e = 0; e < enemies.Count; e++)
                {
                    if (enemies[e].hp <= 0) continue;
                    CharacterData target = FirstAliveCrew(party);
                    if (target == null) break;
                    int dmg = enemies[e].atk + rng.Next(3) - target.stats.def;
                    if (dmg < 1) dmg = 1;
                    target.stats.hp -= dmg;
                    res.log.Add(enemies[e].name + " causou " + dmg + " em " + target.displayName + ".");
                    if (target.stats.hp <= 0)
                    {
                        target.stats.hp = 0;
                        target.alive = false;
                        res.deadCrewIds.Add(target.id);
                        res.log.Add(target.displayName + " morreu em combate.");
                    }
                }
                if (FirstAliveCrew(party) == null)
                {
                    res.victory = false;
                    break;
                }
            }
            res.rounds = round;
            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].alive)
                {
                    party[i].battles++;
                }
            }
            return res;
        }

        private static EnemyData FirstAlive(List<EnemyData> enemies)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].hp > 0) return enemies[i];
            }
            return null;
        }

        private static CharacterData FirstAliveCrew(List<CharacterData> party)
        {
            CharacterData slowest = null;
            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].alive && party[i].stats.hp > 0)
                {
                    if (slowest == null || party[i].stats.speed < slowest.stats.speed)
                    {
                        slowest = party[i];
                    }
                }
            }
            return slowest;
        }
    }
}
