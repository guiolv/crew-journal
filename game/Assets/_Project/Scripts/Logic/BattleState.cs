// CrewJournal — combate interativo por turnos (refs: ordem de turnos + acoes).
// Fila por velocidade; inimigos agem automaticamente. Deterministico via seed.
using System;
using System.Collections.Generic;

namespace CrewJournal.Logic
{
    public enum BattleAction
    {
        Attack,
        Heavy,
        Defend,
        Item,
        Flee
    }

    public class BattleState
    {
        public List<CharacterData> party;
        public List<EnemyData> enemies;
        public int equipBonus;
        public int medicines;
        public int medicinesUsed;
        public bool over;
        public bool victory;
        public bool fled;
        public List<string> log = new List<string>();
        public int round = 1;

        private Random rng;
        private List<object> queue = new List<object>();
        private int ptr;
        private HashSet<string> defended = new HashSet<string>();

        public static BattleState Start(List<CharacterData> party, List<EnemyData> enemies, int equipBonus, int medicines, int seed)
        {
            BattleState s = new BattleState();
            s.party = party;
            s.enemies = enemies;
            s.equipBonus = equipBonus;
            s.medicines = medicines;
            s.rng = new Random(seed);
            s.Rebuild();
            s.Log("Batalha! Round 1.");
            return s;
        }

        private void Log(string t)
        {
            log.Add(t);
            if (log.Count > 40) log.RemoveAt(0);
        }

        private void Rebuild()
        {
            queue.Clear();
            ptr = 0;
            defended.Clear();
            List<object> all = new List<object>();
            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].alive && party[i].stats.hp > 0) all.Add(party[i]);
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].hp > 0) all.Add(enemies[i]);
            }
            all.Sort(SpeedDesc);
            queue = all;
        }

        private static int SpeedDesc(object a, object b)
        {
            return Spd(b).CompareTo(Spd(a));
        }

        private static int Spd(object o)
        {
            if (o is CharacterData) return ((CharacterData)o).stats.speed;
            return ((EnemyData)o).speed;
        }

        public CharacterData CurrentCrew()
        {
            SkipDead();
            if (over || ptr >= queue.Count) return null;
            object o = queue[ptr];
            if (o is CharacterData)
            {
                CharacterData c = (CharacterData)o;
                if (c.alive && c.stats.hp > 0) return c;
            }
            return null;
        }

        // Avanca o ponteiro sobre entradas mortas sem executar acoes (sem RNG).
        private void SkipDead()
        {
            int guard = 0;
            while (!over && ptr < queue.Count && guard < 40)
            {
                guard++;
                object o = queue[ptr];
                if (o is CharacterData)
                {
                    CharacterData c = (CharacterData)o;
                    if (c.alive && c.stats.hp > 0) return;
                    ptr++;
                    continue;
                }
                if (((EnemyData)o).hp > 0) return;
                ptr++;
            }
            if (!over && ptr >= queue.Count)
            {
                round++;
                Rebuild();
                Log("Round " + round + ".");
            }
        }

        // Ha acao inimiga pendente para resolver passo a passo (UI ritmada)?
        public bool EnemyTurnPending()
        {
            if (over) return false;
            SkipDead();
            if (over || ptr >= queue.Count) return false;
            return !(queue[ptr] is CharacterData);
        }

        // Executa UMA acao inimiga. Retorna a linha de log gerada (ou "").
        public string StepEnemy()
        {
            if (over) return "";
            SkipDead();
            if (over || ptr >= queue.Count) return "";
            if (queue[ptr] is CharacterData) return "";
            int before = log.Count;
            EnemyAct((EnemyData)queue[ptr]);
            ptr++;
            if (ptr >= queue.Count && !over)
            {
                round++;
                Rebuild();
                Log("Round " + round + ".");
            }
            if (log.Count > before) return log[log.Count - 1];
            return "";
        }

        private CharacterData WeakestCrew()
        {
            CharacterData w = null;
            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].alive && party[i].stats.hp > 0 && !party[i].grave)
                {
                    if (w == null || party[i].stats.hp < w.stats.hp) w = party[i];
                }
            }
            if (w != null) return w;
            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].alive && party[i].stats.hp > 0) return party[i];
            }
            return null;
        }

        private EnemyData FirstEnemy()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].hp > 0) return enemies[i];
            }
            return null;
        }

        private void CheckOver()
        {
            if (FirstEnemy() == null) { over = true; victory = true; Log("Vitoria!"); return; }
            if (WeakestCrew() == null) { over = true; victory = false; Log("Derrota..."); }
        }

        private void EnemyAct(EnemyData e)
        {
            if (e.hp <= 0) return;
            CharacterData t = WeakestCrew();
            if (t == null) { CheckOver(); return; }
            int dmg = e.atk + rng.Next(3) - t.stats.def;
            if (defended.Contains(t.id)) dmg /= 2;
            if (dmg < 1) dmg = 1;
            t.stats.hp -= dmg;
            Log(e.name + " causou " + dmg + " em " + t.displayName + ".");
            if (t.stats.hp <= 0)
            {
                if (!t.grave)
                {
                    t.grave = true;
                    t.stats.hp = 1;
                    Log(t.displayName + " está em estado GRAVE! Precisa de tratamento.");
                }
                else
                {
                    t.stats.hp = 0;
                    t.alive = false;
                    Log(t.displayName + " caiu!");
                }
            }
            CheckOver();
        }

        // Retorna texto do turno. targetIdx = indice no enemies (apenas Attack/Heavy).
        public string Act(BattleAction a, int targetIdx)
        {
            CharacterData c = CurrentCrew();
            if (c == null || over) return "Sem ator.";
            string out_ = "";
            if (a == BattleAction.Flee)
            {
                if (rng.NextDouble() < 0.45)
                {
                    fled = true;
                    over = true;
                    Log("Fuga bem-sucedida!");
                    return "Fugimos!";
                }
                Log("Fuga falhou!");
                out_ = "Fuga falhou! ";
                ptr++;
            }
            else if (a == BattleAction.Defend)
            {
                defended.Add(c.id);
                Log(c.displayName + " defendeu.");
                out_ = c.displayName + " em guarda. ";
                ptr++;
            }
            else if (a == BattleAction.Item)
            {
                if (medicines - medicinesUsed <= 0) return "Sem medicina!";
                medicinesUsed++;
                CharacterData w = WeakestCrew();
                if (w == null) return "Sem alvo.";
                w.stats.hp = Math.Min(w.stats.maxHp, w.stats.hp + 15);
                Log(c.displayName + " usou medicina em " + w.displayName + " (+15).");
                out_ = "Medicina usada. ";
                ptr++;
            }
            else
            {
                EnemyData t = null;
                if (targetIdx >= 0 && targetIdx < enemies.Count && enemies[targetIdx].hp > 0)
                {
                    t = enemies[targetIdx];
                }
                else
                {
                    t = FirstEnemy();
                }
                if (t == null) { CheckOver(); return "Sem inimigos."; }
                int effAtk = c.grave ? Math.Max(1, c.stats.atk / 2) : c.stats.atk;
                int dmg;
                if (a == BattleAction.Heavy)
                {
                    dmg = (effAtk * 3) / 2 + c.fighter / 10 + rng.Next(3) - t.def + equipBonus;
                    c.stats.hp = Math.Max(1, c.stats.hp - 2);
                    Log(c.displayName + " golpe pesado em " + t.name + "!");
                }
                else
                {
                    dmg = effAtk + c.fighter / 15 + rng.Next(4) - t.def + equipBonus;
                }
                if (dmg < 1) dmg = 1;
                t.hp -= dmg;
                out_ = c.displayName + " causou " + dmg + " em " + t.name + ". ";
                Log(out_);
                if (t.hp <= 0) Log(t.name + " derrotado!");
                CheckOver();
                ptr++;
            }
            if (ptr >= queue.Count && !over)
            {
                round++;
                Rebuild();
                Log("Round " + round + ".");
            }
            return out_;
        }

        public List<string> DeadIds()
        {
            List<string> ids = new List<string>();
            for (int i = 0; i < party.Count; i++)
            {
                if (!party[i].alive) ids.Add(party[i].id);
            }
            return ids;
        }
    }
}
