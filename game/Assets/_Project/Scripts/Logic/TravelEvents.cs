// CrewJournal — eventos de viagem com escolhas (GDD secao 10, refs tela Evento).
// 3 opcoes por evento, trade-off explicito. Deterministico via rng injetado.
using System;

namespace CrewJournal.Logic
{
    public class EventChoice
    {
        public string label;
        public string desc;
    }

    public static class TravelEvents
    {
        public static bool NeedsChoice(TravelEventKind kind)
        {
            return kind == TravelEventKind.Storm
                || kind == TravelEventKind.AbandonedShip
                || kind == TravelEventKind.UnknownShip
                || kind == TravelEventKind.SeaCreature
                || kind == TravelEventKind.Whirlpool;
        }

        public static string TitleOf(TravelEventKind kind)
        {
            switch (kind)
            {
                case TravelEventKind.Storm: return "Tempestade a frente!";
                case TravelEventKind.AbandonedShip: return "Navio abandonado";
                case TravelEventKind.UnknownShip: return "Navio desconhecido";
                case TravelEventKind.SeaCreature: return "Criatura marinha!";
                case TravelEventKind.Whirlpool: return "Redemoinho à vista!";
                default: return "Mar calmo";
            }
        }

        public static string DescOf(TravelEventKind kind)
        {
            switch (kind)
            {
                case TravelEventKind.Storm: return "Uma forte tempestade foi avistada no horizonte. O mar esta ficando cada vez mais agitado.";
                case TravelEventKind.AbandonedShip: return "Um casco a deriva, silencioso. Pode haver suprimentos — ou problemas.";
                case TravelEventKind.UnknownShip: return "Vela desconhecida a bombordo. Nao responde aos sinais.";
                case TravelEventKind.SeaCreature: return "Algo enorme move-se sob as ondas, circulando o navio.";
                case TravelEventKind.Whirlpool: return "As águas giram com força adiante, puxando tudo para o centro. Passar perto é arriscado.";
                default: return "Nenhum evento.";
            }
        }

        public static EventChoice[] OptionsOf(TravelEventKind kind)
        {
            switch (kind)
            {
                case TravelEventKind.Storm:
                    return new EventChoice[] {
                        Opt("Atravessar a tempestade", "alto risco ao casco, +XP navegacao"),
                        Opt("Desviar da rota", "+1 dia de viagem, seguro"),
                        Opt("Esperar passar", "+2 dias, consome suprimentos") };
                case TravelEventKind.AbandonedShip:
                    return new EventChoice[] {
                        Opt("Saquear", "+moedas e madeira, pequeno risco"),
                        Opt("Investigar", "+medicina, seguro"),
                        Opt("Ignorar", "seguir viagem") };
                case TravelEventKind.UnknownShip:
                    return new EventChoice[] {
                        Opt("Aproximar", "troca ou armadilha"),
                        Opt("Atacar", "combate imediato"),
                        Opt("Fugir", "evitar contato") };
                case TravelEventKind.Whirlpool:
                    return new EventChoice[] {
                        Opt("Contornar", "navegador bom passa ileso, +XP"),
                        Opt("Atravessar a borda", "rapido, dano ao casco, perde carga"),
                        Opt("Esperar dissipar", "+1 dia, consome suprimentos") };
                default:
                    return new EventChoice[] {
                        Opt("Lutar", "combate imediato"),
                        Opt("Fugir", "rapido, risco ao casco"),
                        Opt("Evitar", "navegador bom passa ileso") };
            }
        }

        private static EventChoice Opt(string l, string d)
        {
            EventChoice o = new EventChoice();
            o.label = l;
            o.desc = d;
            return o;
        }

        // Aplica a escolha. Retorna texto; needCombat/combatDanger por out.
        public static string ApplyChoice(GameData d, TravelEventKind kind, int idx, Random rng, out bool needCombat, out int combatDanger)
        {
            needCombat = false;
            combatDanger = 3;
            if (idx < 0) idx = 0;
            if (idx > 2) idx = 2;
            switch (kind)
            {
                case TravelEventKind.Storm: return Storm(d, idx, rng);
                case TravelEventKind.AbandonedShip: return Loot(d, idx, rng);
                case TravelEventKind.UnknownShip: return Ship(d, idx, rng, out needCombat, out combatDanger);
                case TravelEventKind.SeaCreature: return Creature(d, idx, rng, out needCombat, out combatDanger);
                case TravelEventKind.Whirlpool: return Whirlpool(d, idx, rng);
                default: return "Seguimos viagem.";
            }
        }

        private static string Storm(GameData d, int idx, Random rng)
        {
            if (idx == 0)
            {
                int dmg = 4 + rng.Next(7);
                d.ship.hull = Math.Max(0, d.ship.hull - dmg);
                for (int i = 0; i < d.crew.Count; i++)
                {
                    if (d.crew[i].alive && d.crew[i].nav == GameSession.BestNavigator(d))
                    {
                        Progression.AddXp(d.crew[i], 30);
                        break;
                    }
                }
                GameSession.AddJournal(d, "Atravessamos a tempestade! Casco -" + dmg + ".");
                return "Atravessamos! Casco -" + dmg + ", navegador ganhou XP.";
            }
            if (idx == 1)
            {
                d.sailTotal += 1;
                GameSession.AddJournal(d, "Desviamos da tempestade (+1 dia).");
                return "Rota desviada: +1 dia de viagem, sem danos.";
            }
            d.sailTotal += 2;
            d.AddResource(ResourceId.Food, -2);
            d.AddResource(ResourceId.Water, -2);
            GameSession.AddJournal(d, "Esperamos a tempestade passar (+2 dias, -2 comida, -2 agua).");
            return "Esperamos em seguranca: +2 dias, -2 comida, -2 agua.";
        }

        private static string Loot(GameData d, int idx, Random rng)
        {
            if (idx == 0)
            {
                int loot = 20 + rng.Next(60);
                d.AddResource(ResourceId.Money, loot);
                d.AddResource(ResourceId.Wood, 1);
                if (rng.NextDouble() < 0.25)
                {
                    d.ship.hull = Math.Max(0, d.ship.hull - 3);
                    GameSession.AddJournal(d, "Saque com armadilha! +" + loot + " moedas, casco -3.");
                    return "Saque! +" + loot + " moedas, +1 madeira, mas havia armadilha (casco -3).";
                }
                GameSession.AddJournal(d, "Navio abandonado saqueado: +" + loot + " moedas, +1 madeira.");
                return "Saque limpo: +" + loot + " moedas, +1 madeira.";
            }
            if (idx == 1)
            {
                d.AddResource(ResourceId.Medicine, 1);
                GameSession.AddJournal(d, "Investigamos o navio: +1 medicina, sobrevivente agradece (+1 moral).");
                for (int i = 0; i < d.crew.Count; i++) d.crew[i].morale = Math.Min(100, d.crew[i].morale + 1);
                return "Investigacao: +1 medicina, moral +1.";
            }
            return "Ignoramos o navio e seguimos viagem.";
        }

        private static string Ship(GameData d, int idx, Random rng, out bool needCombat, out int combatDanger)
        {
            needCombat = false;
            combatDanger = 3;
            if (idx == 0)
            {
                if (rng.NextDouble() < 0.5)
                {
                    d.AddResource(ResourceId.Money, 30);
                    d.SetRep(d.currentIslandId, d.GetRep(d.currentIslandId) + 1);
                    GameSession.AddJournal(d, "Eram mercadores! Troca rapida: +30 moedas.");
                    return "Eram mercadores: +30 moedas.";
                }
                needCombat = true;
                combatDanger = 3;
                GameSession.AddJournal(d, "Armadilha! Piratas a bordo — preparar combate.");
                return "Armadilha! Piratas — combate!";
            }
            if (idx == 1)
            {
                needCombat = true;
                combatDanger = 4;
                GameSession.AddJournal(d, "Atacamos o navio desconhecido!");
                return "Ataque! Combate imediato.";
            }
            GameSession.AddJournal(d, "Evitamos o navio desconhecido.");
            return "Fugimos sem ser notados.";
        }

        private static string Creature(GameData d, int idx, Random rng, out bool needCombat, out int combatDanger)
        {
            needCombat = false;
            combatDanger = 4;
            if (idx == 0)
            {
                needCombat = true;
                GameSession.AddJournal(d, "Enfrentamos a criatura marinha!");
                return "Lutar! Combate imediato.";
            }
            if (idx == 1)
            {
                if (rng.NextDouble() < 0.7)
                {
                    GameSession.AddJournal(d, "Fugimos da criatura por pouco.");
                    return "Fuga bem-sucedida!";
                }
                d.ship.hull = Math.Max(0, d.ship.hull - 3);
                needCombat = true;
                GameSession.AddJournal(d, "A criatura alcancou o navio (casco -3)! Combate!");
                return "Ela nos alcancou (casco -3)! Combate!";
            }
            if (GameSession.BestNavigator(d) >= 50)
            {
                Progression.AddXpBestNav(d, 15);
                GameSession.AddJournal(d, "Navegador contornou a criatura sem ser visto.");
                return "Manobra perfeita: evitamos a criatura.";
            }
            needCombat = true;
            GameSession.AddJournal(d, "Falha ao evitar a criatura! Combate!");
            return "Navegador inexperiente: fomos vistos! Combate!";
        }

        private static string Whirlpool(GameData d, int idx, Random rng)
        {
            if (idx == 0)
            {
                if (GameSession.BestNavigator(d) >= 60)
                {
                    Progression.AddXpBestNav(d, 15);
                    GameSession.AddJournal(d, "Navegador contornou o redemoinho com precisão.");
                    return "Contornado sem danos. Navegador ganhou XP.";
                }
                d.ship.hull = Math.Max(0, d.ship.hull - 4);
                GameSession.AddJournal(d, "A borda do redemoinho atingiu o casco (-4).");
                return "Manobra parcial: casco -4. Contrate um navegador melhor.";
            }
            if (idx == 1)
            {
                int dmg = 5 + rng.Next(5);
                d.ship.hull = Math.Max(0, d.ship.hull - dmg);
                d.AddResource(ResourceId.Food, -2);
                GameSession.AddJournal(d, "Atravessamos a borda do redemoinho! Casco -" + dmg + ", -2 comida ao mar.");
                return "Atravessamos! Casco -" + dmg + ", -2 comida perdida.";
            }
            d.sailTotal += 1;
            d.AddResource(ResourceId.Food, -1);
            d.AddResource(ResourceId.Water, -1);
            GameSession.AddJournal(d, "Esperamos o redemoinho dissipar (+1 dia, -1 comida, -1 agua).");
            return "Esperamos com segurança: +1 dia, -1 comida, -1 agua.";
        }
    }
}
