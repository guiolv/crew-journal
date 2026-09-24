// CrewJournal — fluxo de viagem em etapas (refs: detalhes -> iniciar -> evento -> chegada).
using System;
using System.Collections.Generic;

namespace CrewJournal.Logic
{
    public enum TickResult
    {
        Sailing,
        NeedChoice,
        Arrived
    }

    public class VoyageReport
    {
        public bool ok;
        public string message;
        public TickResult tick;
        public int days;
        public string eventText = "";
        public bool needCombat;
        public int combatDanger;
        public List<string> log = new List<string>();
    }

    public static partial class GameSession
    {
        public static WeatherKind RollWeather(int seed, int day, int destIdx, int danger)
        {
            int h = Math.Abs(seed * 31 + day * 17 + destIdx * 13) % 10;
            // Mares de tempestade: destinos perigosos (danger>=4) têm o dobro de chance de Storm.
            if (h <= 5) return WeatherKind.Clear;
            if (h <= 7) return WeatherKind.Cloudy;
            if (h == 8 && danger >= 4) return WeatherKind.Storm;
            if (h <= 8) return WeatherKind.Cloudy;
            return WeatherKind.Storm;
        }

        public static WeatherKind RollWeather(int seed, int day, int destIdx)
        {
            return RollWeather(seed, day, destIdx, 1);
        }

        public static string WeatherName(WeatherKind w)
        {
            if (w == WeatherKind.Clear) return "Limpo";
            if (w == WeatherKind.Cloudy) return "Nublado";
            return "Tempestade";
        }

        public static TravelResult PreviewCalc(IslandData from, IslandData to, ShipData ship, int nav, WeatherKind w)
        {
            TravelResult r = NavigationSystem.Calculate(from, to, ship, nav, to.danger);
            if (w == WeatherKind.Cloudy)
            {
                r.risk = Math.Min(0.85f, r.risk * 1.15f);
                r.eventChance = Math.Min(0.9f, r.eventChance + 0.05f);
            }
            else if (w == WeatherKind.Storm)
            {
                r.days += 1;
                r.risk = Math.Min(0.85f, r.risk * 1.4f);
                r.eventChance = Math.Min(0.9f, r.eventChance + 0.15f);
            }
            return r;
        }

        public static void ApplyStance(TravelResult r, int stance)
        {
            // Regra de design: navegador + ordens REDUZEM o RNG, nunca eliminam.
            // Piso absoluto: risco 0.02, evento 0.05.
            if (stance == 1)
            {
                r.days += 1;
                r.risk = Math.Max(0.02f, r.risk * 0.7f);
                r.eventChance = Math.Max(0.05f, r.eventChance - 0.1f);
            }
            else if (stance == 2)
            {
                r.days = Math.Max(1, r.days - 1);
                r.risk = Math.Min(0.9f, r.risk * 1.3f);
                r.eventChance = Math.Min(0.95f, r.eventChance + 0.1f);
            }
        }

        public static string StanceName(int stance)
        {
            if (stance == 1) return "Cautela";
            if (stance == 2) return "Marcha";
            return "Normal";
        }

        private static int DestIndex(GameData d, string destId)
        {
            for (int i = 0; i < d.world.islands.Count; i++)
            {
                if (d.world.islands[i].id == destId) return i;
            }
            return 0;
        }

        public static bool PreviewTravel(GameData d, string destId, out TravelResult calc, out WeatherKind w, out string err)
        {
            calc = null;
            w = WeatherKind.Clear;
            err = "";
            IslandData from = d.FindIsland(d.currentIslandId);
            IslandData to = d.FindIsland(destId);
            if (to == null) { err = "Destino invalido."; return false; }
            if (from != null && from.id == to.id) { err = "Voce ja esta aqui."; return false; }
            IslandData o = from != null ? from : to;
            w = RollWeather(d.world.seed, d.world.day, DestIndex(d, destId), to.danger);
            calc = PreviewCalc(o, to, d.ship, BestNavigator(d), w);
            ApplyStance(calc, d.sailStance);
            int crewN = Math.Max(1, AliveCrew(d));
            int needFood = (int)Math.Ceiling(calc.foodCost * crewN * ShipModules.FoodFactor(d.ship));
            int needWater = calc.waterCost * crewN;
            if (d.GetResource(ResourceId.Food) < needFood || d.GetResource(ResourceId.Water) < needWater)
            {
                err = "Suprimentos insuficientes (" + needFood + " comida, " + needWater + " agua).";
                return false;
            }
            return true;
        }

        public static VoyageReport BeginTravel(GameData d, string destId)
        {
            VoyageReport rep = new VoyageReport();
            TravelResult calc;
            WeatherKind w;
            string err;
            if (!PreviewTravel(d, destId, out calc, out w, out err))
            {
                rep.ok = false;
                rep.message = err;
                return rep;
            }
            int crewN = Math.Max(1, AliveCrew(d));
            d.AddResource(ResourceId.Food, -(int)Math.Ceiling(calc.foodCost * crewN * ShipModules.FoodFactor(d.ship)));
            d.AddResource(ResourceId.Water, -calc.waterCost * crewN);
            int wages = 0;
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].alive) wages += d.crew[i].wage * calc.days;
            }
            d.AddResource(ResourceId.Money, -wages);
            if (d.GetResource(ResourceId.Money) < 0) d.SetResource(ResourceId.Money, 0);
            d.sailDestId = destId;
            d.sailDay = 0;
            d.sailTotal = calc.days;
            Random rng = new Random(d.world.seed + d.world.day * 911 + d.journal.Count * 37);
            d.pendingEvent = NavigationSystem.RollEvent(rng, calc.eventChance);
            if (d.pendingEvent == TravelEventKind.CalmBonus)
            {
                d.AddResource(ResourceId.Food, 2);
                AddJournal(d, "Corrente favoravel: pesca extra (+2 comida).");
                rep.eventText = "Corrente favoravel: +2 comida.";
                d.pendingEvent = TravelEventKind.None;
            }
            AddJournal(d, "Partimos para " + d.FindIsland(destId).displayName + " (" + WeatherName(w) + ").");
            rep.ok = true;
            rep.tick = TickResult.Sailing;
            rep.days = calc.days;
            rep.message = "Viagem iniciada (" + calc.days + " dias, " + WeatherName(w) + ").";
            return rep;
        }

        public static VoyageReport TravelTick(GameData d)
        {
            VoyageReport rep = new VoyageReport();
            rep.ok = true;
            rep.tick = TickResult.Sailing;
            if (d.sailDestId == null || d.sailDestId == "")
            {
                rep.ok = false;
                rep.message = "Sem viagem em curso.";
                return rep;
            }
            d.sailDay++;
            d.world.day++;
            Progression.AddXpBestNav(d, 4);
            if (d.sailDay == 1 && TravelEvents.NeedsChoice(d.pendingEvent))
            {
                rep.tick = TickResult.NeedChoice;
                rep.message = TravelEvents.TitleOf(d.pendingEvent);
                return rep;
            }
            if (d.sailDay >= d.sailTotal)
            {
                return Arrive(d);
            }
            rep.message = "Dia " + d.sailDay + "/" + d.sailTotal + ".";
            return rep;
        }

        public static VoyageReport ChooseEvent(GameData d, int idx)
        {
            VoyageReport rep = new VoyageReport();
            rep.ok = true;
            rep.tick = TickResult.Sailing;
            Random rng = new Random(d.world.seed + d.world.day * 911 + d.journal.Count * 37 + 5);
            bool nc;
            int cd;
            rep.eventText = TravelEvents.ApplyChoice(d, d.pendingEvent, idx, rng, out nc, out cd);
            d.pendingEvent = TravelEventKind.None;
            rep.needCombat = nc;
            rep.combatDanger = cd;
            rep.message = rep.eventText;
            return rep;
        }

        public static VoyageReport Arrive(GameData d)
        {
            VoyageReport rep = new VoyageReport();
            rep.ok = true;
            rep.tick = TickResult.Arrived;
            IslandData to = d.FindIsland(d.sailDestId);
            d.sailDestId = "";
            d.pendingEvent = TravelEventKind.None;
            if (to == null)
            {
                rep.message = "Destino perdido.";
                return rep;
            }
            d.currentIslandId = to.id;
            AddJournal(d, "Chegada a " + to.displayName + " no dia " + d.world.day + ".");
            rep.log.Add("Chegada a " + to.displayName + ".");
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
            Random rng = new Random(d.world.seed + d.world.day * 733 + d.journal.Count * 11);
            if (to.danger >= 4 && rng.NextDouble() < 0.5)
            {
                rep.needCombat = true;
                rep.combatDanger = Math.Max(to.danger, 3);
            }
            BondEvent(d, rng, rep);
            rep.days = d.sailTotal;
            rep.message = "Viagem concluida.";
            return rep;
        }

        public static void BondEvent(GameData d, Random rng, VoyageReport rep)
        {
            List<CharacterData> alive = new List<CharacterData>();
            for (int i = 0; i < d.crew.Count; i++)
            {
                if (d.crew[i].alive) alive.Add(d.crew[i]);
            }
            if (alive.Count < 2 || rng.NextDouble() > 0.35) return;
            CharacterData a = alive[rng.Next(alive.Count)];
            CharacterData b = alive[rng.Next(alive.Count)];
            if (a.id == b.id) return;
            RelationshipData rel = null;
            for (int i = 0; i < d.relations.Count; i++)
            {
                RelationshipData r = d.relations[i];
                if ((r.aId == a.id && r.bId == b.id) || (r.aId == b.id && r.bId == a.id)) rel = r;
            }
            if (rel == null)
            {
                rel = new RelationshipData();
                rel.aId = a.id;
                rel.bId = b.id;
                rel.affinity = 10;
                rel.trust = 30;
                d.relations.Add(rel);
            }
            rel.affinity = Math.Min(100, rel.affinity + 8 + rng.Next(8));
            rel.trust = Math.Min(100, rel.trust + 5);
            string[] lines = new string[] {
                " dividiram o turno da noite e conversaram até tarde.",
                " pescaram juntos ao amanhecer e riram pela primeira vez em dias.",
                " consertaram uma vela rasgada lado a lado, em silêncio cúmplice."
            };
            string txt = a.displayName + " e " + b.displayName + lines[rng.Next(lines.Length)];
            AddJournal(d, txt);
            rep.log.Add(txt);
        }

        public static bool TreatWound(GameData d, string charId, out string msg)
        {
            CharacterData c = FindChar(d, charId);
            if (c == null || !c.alive) { msg = "Tripulante indisponível."; return false; }
            if (!c.grave) { msg = "Sem ferimentos graves."; return false; }
            if (d.GetResource(ResourceId.Medicine) < 1) { msg = "Precisa de 1 medicina."; return false; }
            d.AddResource(ResourceId.Medicine, -1);
            c.grave = false;
            c.stats.hp = Math.Max(c.stats.hp, c.stats.maxHp / 2);
            AddJournal(d, c.displayName + " foi tratado e deixou de estar em estado grave.");
            msg = c.displayName + " tratado!";
            return true;
        }

        // Fase final de combate compartilhada (auto ou por turnos).
        public static string FinishBattle(GameData d, List<CharacterData> party, List<string> deadIds, bool victory, bool fled, int danger, List<string> battleLog)
        {
            string out_ = "";
            for (int i = 0; i < battleLog.Count && i < 8; i++) out_ += battleLog[i] + "\n";
            if (fled)
            {
                AddJournal(d, "Batalha evitada com fuga no dia " + d.world.day + ".");
                for (int i = 0; i < deadIds.Count; i++) RegisterDeath(d, deadIds[i], "Ferimentos em combate");
                return out_ + "Fuga bem-sucedida.";
            }
            if (victory)
            {
                int reward = 60 + danger * 40;
                d.AddResource(ResourceId.Money, reward);
                d.notoriety += 20 + danger * 5;
                Random scarr = new Random(d.world.seed + d.world.day * 131);
                for (int i = 0; i < party.Count; i++)
                {
                    if (party[i].alive)
                    {
                        party[i].kills++;
                        int ups = Progression.AddXp(party[i], 25);
                        if (ups > 0) out_ += party[i].displayName + " subiu para o nivel " + party[i].level + "!\n";
                        if (!party[i].scar && scarr.NextDouble() < 0.15)
                        {
                            party[i].scar = true;
                            out_ += party[i].displayName + " ganhou uma cicatriz.\n";
                        }
                    }
                }
                for (int i = 0; i < d.missions.Count; i++)
                {
                    if (d.missions[i].type == MissionType.Combat &&
                        (d.missions[i].status == MissionStatus.Accepted || d.missions[i].status == MissionStatus.InProgress))
                    {
                        d.missions[i].status = MissionStatus.Completed;
                        d.AddResource(ResourceId.Money, d.missions[i].reward);
                        out_ += "Missao de combate cumprida (+" + d.missions[i].reward + ").\n";
                    }
                }
                AddJournal(d, "Vitoria em batalha no dia " + d.world.day + " (+" + reward + " moedas).");
                out_ += "Vitoria! +" + reward + " moedas.";
            }
            else
            {
                d.ship.hull -= 5;
                if (d.ship.hull < 0) d.ship.hull = 0;
                AddJournal(d, "Derrota em batalha no dia " + d.world.day + ". Navio danificado.");
                out_ += "Derrota. Navio -5 casco.";
            }
            for (int i = 0; i < deadIds.Count; i++) RegisterDeath(d, deadIds[i], "Ferimentos em combate");
            return out_;
        }
    }
}
