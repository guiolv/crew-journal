// CrewJournal — orquestrador puro do jogo (GameLoop + sistemas). Sem UnityEngine.
// A UI (Unity) chama estes metodos e redesenha. Toda mutacao gera entrada no diario.
using System;
using System.Collections.Generic;

namespace CrewJournal.Logic
{
    public class TravelReport
    {
        public bool ok;
        public string message;
        public int days;
        public TravelEventKind eventKind;
        public string eventText = "";
        public bool needCombat;
        public int combatDanger;
        public List<string> log = new List<string>();
    }

    public static partial class GameSession
    {
        public static GameData NewGame(int seed)
        {
            GameData d = new GameData();
            d.world = WorldGenerator.Generate(seed);
            d.ship = new ShipData();
            d.ship.defId = "barrel";
            d.ship.maxHull = 10;
            d.ship.hull = 10;
            d.ship.crewCap = 1;
            d.ship.cargoCap = 2;
            d.ship.speed = 0.8f;
            d.ship.modules = 0;
            d.SetResource(ResourceId.Money, Balance.StartMoney);
            d.SetResource(ResourceId.Food, Balance.StartFood);
            d.SetResource(ResourceId.Water, Balance.StartWater);
            d.SetResource(ResourceId.Wood, Balance.StartWood);
            d.SetResource(ResourceId.Metal, Balance.StartMetal);
            d.SetResource(ResourceId.Medicine, Balance.StartMedicine);
            // capitao inicial
            CharacterData cap = CharacterGenerator.Generate(seed, 0);
            cap.hireCost = 0;
            d.crew.Add(cap);
            d.nextCharIndex = 1;
            // recrutas iniciais: 2 por ilha (ids referenciam roster global)
            for (int i = 0; i < d.world.islands.Count; i++)
            {
                for (int k = 0; k < 2; k++)
                {
                    CharacterData r = CharacterGenerator.Generate(seed, d.nextCharIndex);
                    d.nextCharIndex++;
                    d.roster.Add(r);
                    d.world.islands[i].recruitIds.Add(r.id);
                }
            }
            // missoes iniciais: 2
            for (int m = 0; m < 2; m++)
            {
                d.missions.Add(MissionSystem.Generate(seed, m, d.world));
            }
            d.nextMissionIndex = 2;
            d.currentIslandId = d.world.islands[0].id;
            AddJournal(d, "A jornada comeca num barril, sozinho, com poucos recursos.");
            AddJournal(d, "Chegada a " + d.world.islands[0].displayName + ".");
            return d;
        }

        public static void AddJournal(GameData d, string text)
        {
            JournalEntry j = new JournalEntry();
            j.day = d.world.day;
            j.text = text;
            d.journal.Add(j);
        }

        public static CharacterData FindChar(GameData d, string id)
        {
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].id == id) return d.crew[i];
            }
            for (int i = 0; i < d.roster.Count; i++)
            {
                if (d.roster[i].id == id) return d.roster[i];
            }
            return null;
        }

        public static int BestNavigator(GameData d)
        {
            int best = 0;
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].alive && d.crew[i].nav > best) best = d.crew[i].nav;
            }
            return best;
        }

        public static int AliveCrew(GameData d)
        {
            int n = 0;
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].alive) n++;
            }
            return n;
        }

        public static TravelReport Travel(GameData d, string destId)
        {
            TravelReport rep = new TravelReport();
            IslandData from = d.FindIsland(d.currentIslandId);
            IslandData to = d.FindIsland(destId);
            if (to == null) { rep.ok = false; rep.message = "Destino invalido."; return rep; }
            if (from != null && from.id == to.id) { rep.ok = false; rep.message = "Voce ja esta aqui."; return rep; }
            int crewN = Math.Max(1, AliveCrew(d));
            TravelResult calc;
            if (from == null)
            {
                ShipData tmp = d.ship;
                calc = NavigationSystem.Calculate(to, to, tmp, BestNavigator(d), to.danger);
                calc.days = 1;
            }
            else
            {
                calc = NavigationSystem.Calculate(from, to, d.ship, BestNavigator(d), to.danger);
            }
            int needFood = calc.foodCost * crewN;
            int needWater = calc.waterCost * crewN;
            if (d.GetResource(ResourceId.Food) < needFood || d.GetResource(ResourceId.Water) < needWater)
            {
                rep.ok = false;
                rep.message = "Suprimentos insuficientes para a viagem (" + needFood + " comida, " + needWater + " agua).";
                return rep;
            }
            d.AddResource(ResourceId.Food, -needFood);
            d.AddResource(ResourceId.Water, -needWater);
            // salarios
            int wages = 0;
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].alive) wages += d.crew[i].wage * calc.days;
            }
            d.AddResource(ResourceId.Money, -wages);
            if (d.GetResource(ResourceId.Money) < 0) d.SetResource(ResourceId.Money, 0);
            d.world.day += calc.days;
            rep.days = calc.days;
            rep.log.Add("Viagem de " + calc.days + " dias. Consumo: " + needFood + " comida, " + needWater + " agua. Salarios: " + wages + ".");
            // XP de navegacao para o melhor navegador
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].alive && d.crew[i].nav == BestNavigator(d))
                {
                    d.crew[i].nav = Math.Min(99, d.crew[i].nav + 2);
                    break;
                }
            }
            // evento
            Random rng = new Random(d.world.seed + d.world.day * 911 + d.journal.Count * 37);
            rep.eventKind = NavigationSystem.RollEvent(rng, calc.eventChance);
            rep.eventText = ResolveEvent(d, rep.eventKind, rng, to);
            // chegada
            d.currentIslandId = to.id;
            AddJournal(d, "Chegada a " + to.displayName + " no dia " + d.world.day + ".");
            rep.log.Add("Chegada a " + to.displayName + ".");
            // missoes de chegada
            for (int i = 0; i < d.missions.Count; i++)
            {
                MissionData m = d.missions[i];
                if (MissionSystem.CheckArrivalComplete(m, to.id))
                {
                    m.status = MissionStatus.Completed;
                    d.AddResource(ResourceId.Money, m.reward);
                    d.notoriety += 10;
                    d.SetRep(to.id, d.GetRep(to.id) + 5);
                    AddJournal(d, "Missao cumprida: " + m.title + " (+" + m.reward + ").");
                    rep.log.Add("Missao cumprida: " + m.title + " (+" + m.reward + ").");
                }
            }
            // combate: ilha perigosa ou evento puxou combate
            if (rep.needCombat || (to.danger >= 4 && rng.NextDouble() < 0.5))
            {
                rep.needCombat = true;
                rep.combatDanger = Math.Max(to.danger, 3);
            }
            rep.ok = true;
            rep.message = "Viagem concluida em " + calc.days + " dias.";
            return rep;
        }

        private static string ResolveEvent(GameData d, TravelEventKind kind, Random rng, IslandData dest)
        {
            switch (kind)
            {
                case TravelEventKind.None:
                    return "Mar calmo. Nenhum evento.";
                case TravelEventKind.CalmBonus:
                    d.AddResource(ResourceId.Food, 2);
                    AddJournal(d, "Corrente favoravel: pesca extra (+2 comida).");
                    return "Corrente favoravel: +2 comida.";
                case TravelEventKind.Storm:
                    {
                        int dmg = 2 + rng.Next(6);
                        d.ship.hull -= dmg;
                        if (d.ship.hull < 0) d.ship.hull = 0;
                        AddJournal(d, "Tempestade! Casco sofreu " + dmg + " de dano.");
                        return "Tempestade! Casco -" + dmg + ".";
                    }
                case TravelEventKind.AbandonedShip:
                    {
                        int loot = 20 + rng.Next(60);
                        d.AddResource(ResourceId.Money, loot);
                        d.AddResource(ResourceId.Wood, 1);
                        AddJournal(d, "Navio abandonado saqueado: +" + loot + " moedas, +1 madeira.");
                        return "Navio abandonado: +" + loot + " moedas, +1 madeira.";
                    }
                case TravelEventKind.UnknownShip:
                    AddJournal(d, "Navio desconhecido avistado proximo a " + dest.displayName + ".");
                    return "Navio desconhecido avistado. A tripulacao segue em alerta.";
                default:
                    AddJournal(d, "Criatura marinha avistada! Preparar para combate.");
                    return "Criatura marinha! Combate iminente.";
            }
        }

        public static bool Buy(GameData d, string islandId, ResourceId id, int qty, out string msg)
        {
            IslandData isl = d.FindIsland(islandId);
            if (isl == null) { msg = "Ilha invalida."; return false; }
            int price = EconomySystem.PriceFor(id, WorldGenerator.IslandMult(isl, id), d.GetRep(islandId));
            int total = price * qty;
            if (d.GetResource(ResourceId.Money) < total) { msg = "Dinheiro insuficiente (" + total + ")."; return false; }
            d.AddResource(ResourceId.Money, -total);
            d.AddResource(id, qty);
            msg = "Comprou " + qty + "x " + id.ToString() + " por " + total + ".";
            return true;
        }

        public static bool Sell(GameData d, string islandId, ResourceId id, int qty, out string msg)
        {
            IslandData isl = d.FindIsland(islandId);
            if (isl == null) { msg = "Ilha invalida."; return false; }
            if (d.GetResource(id) < qty) { msg = "Voce nao tem " + qty + "x " + id.ToString() + "."; return false; }
            int price = EconomySystem.SellPriceFor(id, WorldGenerator.IslandMult(isl, id), d.GetRep(islandId));
            d.AddResource(id, -qty);
            d.AddResource(ResourceId.Money, price * qty);
            msg = "Vendeu " + qty + "x " + id.ToString() + " por " + (price * qty) + ".";
            return true;
        }

        public static bool Recruit(GameData d, string charId, out string msg)
        {
            CharacterData c = null;
            for (int i = 0; i < d.roster.Count; i++)
            {
                if (d.roster[i].id == charId) c = d.roster[i];
            }
            if (c == null) { msg = "Personagem indisponivel."; return false; }
            if (AliveCrew(d) >= ShipModules.EffectiveCrewCap(d.ship)) { msg = "Navio lotado (cap " + ShipModules.EffectiveCrewCap(d.ship) + "). Compre um navio maior ou módulos."; return false; }
            if (d.GetResource(ResourceId.Money) < c.hireCost) { msg = "Dinheiro insuficiente (" + c.hireCost + ")."; return false; }
            d.AddResource(ResourceId.Money, -c.hireCost);
            d.roster.Remove(c);
            d.crew.Add(c);
            AddJournal(d, c.displayName + " juntou-se a tripulacao no dia " + d.world.day + ".");
            d.SetRep(d.currentIslandId, d.GetRep(d.currentIslandId) + 2);
            Random rr = new Random(c.id.Length * 131 + d.world.day * 17 + d.crew.Count * 101);
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].id == c.id) continue;
                RelationshipData rel = new RelationshipData();
                rel.aId = c.id;
                rel.bId = d.crew[i].id;
                rel.affinity = rr.Next(-20, 61);
                rel.trust = rr.Next(20, 81);
                d.relations.Add(rel);
            }
            msg = c.displayName + " recrutado!";
            return true;
        }

        public static bool AcceptMission(GameData d, string missionId, out string msg)
        {
            for (int i = 0; i < d.missions.Count; i++)
            {
                if (d.missions[i].id == missionId && d.missions[i].status == MissionStatus.Available)
                {
                    d.missions[i].status = MissionStatus.Accepted;
                    AddJournal(d, "Missao aceita: " + d.missions[i].title + ".");
                    msg = "Missao aceita: " + d.missions[i].title;
                    return true;
                }
            }
            msg = "Missao indisponivel.";
            return false;
        }

        public static string ResolveBattle(GameData d, int danger, int salt, bool flee)
        {
            List<CharacterData> party = new List<CharacterData>();
            for (int i = 0; i < d.crew.Count && party.Count < 4; i++)
            {
                if (d.crew[i].alive) party.Add(d.crew[i]);
            }
            if (party.Count == 0) return "Ninguem vivo para lutar.";
            List<EnemyData> enemies = CombatSystem.GenerateEnemies(d.world.seed + salt, danger, d.world.day);
            BattleResult r = CombatSystem.Simulate(party, enemies, d.world.seed + salt * 3 + d.world.day, flee);
            return FinishBattle(d, party, r.deadCrewIds, r.victory, r.fled, danger, r.log);
        }

        public static void RegisterDeath(GameData d, string charId, string cause)
        {
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].id == charId)
                {
                    CharacterData c = d.crew[i];
                    DeathRecord dr = new DeathRecord();
                    dr.name = c.displayName;
                    dr.dayJoined = 1;
                    dr.dayDied = d.world.day;
                    dr.job = c.jobs.Count > 0 ? c.jobs[0] : "?";
                    dr.battles = c.battles;
                    dr.kills = c.kills;
                    dr.cause = cause;
                    d.memorial.Add(dr);
                    d.crew.RemoveAt(i);
                    AddJournal(d, c.displayName + " morreu no dia " + d.world.day + " (" + cause + ").");
                    for (int k = 0; k < d.crew.Count; k++)
                    {
                        d.crew[k].morale = Math.Max(0, d.crew[k].morale - 10);
                    }
                    return;
                }
            }
        }

        public static bool Repair(GameData d, out string msg)
        {
            int missing = d.ship.maxHull - d.ship.hull;
            if (missing <= 0) { msg = "Casco intacto."; return false; }
            int cost = EconomySystem.RepairCost(missing);
            int wood = (missing + 9) / 10;
            if (d.GetResource(ResourceId.Money) < cost) { msg = "Reparo custa " + cost + " moedas."; return false; }
            if (d.GetResource(ResourceId.Wood) < wood) { msg = "Reparo exige " + wood + " madeira."; return false; }
            d.AddResource(ResourceId.Money, -cost);
            d.AddResource(ResourceId.Wood, -wood);
            d.ship.hull = d.ship.maxHull;
            msg = "Navio reparado.";
            AddJournal(d, "Navio reparado no dia " + d.world.day + ".");
            return true;
        }

        public static bool BuyShip(GameData d, string defId, out string msg)
        {
            int price = 0;
            ShipData s = new ShipData();
            s.defId = defId;
            if (defId == "boat") { price = Balance.BoatPrice; s.maxHull = 50; s.crewCap = 4; s.cargoCap = 8; s.speed = 1.0f; s.modules = 4; }
            else if (defId == "medium") { price = Balance.MediumPrice; s.maxHull = 120; s.crewCap = 10; s.cargoCap = 20; s.speed = 1.15f; s.modules = 10; }
            else { msg = "Navio invalido."; return false; }
            if (d.GetResource(ResourceId.Money) < price) { msg = "Custa " + price + " moedas."; return false; }
            d.AddResource(ResourceId.Money, -price);
            s.hull = s.maxHull;
            d.ship = s;
            AddJournal(d, "Novo navio adquirido: " + defId + " no dia " + d.world.day + ".");
            msg = "Navio " + defId + " adquirido!";
            return true;
        }

        public static bool IsGameOver(GameData d)
        {
            return AliveCrew(d) == 0;
        }
    }
}
