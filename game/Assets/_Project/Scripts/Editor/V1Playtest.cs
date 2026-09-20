// CrewJournal — playthrough automatizado no Editor (valida loop completo no engine).
// Uso: Unity -batchmode -nographics -quit -projectPath ./game -executeMethod CrewJournal.Editor.V1Playtest.FullLoop -logFile -
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CrewJournal.Logic;

namespace CrewJournal.Editor
{
#if UNITY_EDITOR
    public static class V1Playtest
    {
        static int pass;
        static int fail;

        static void Check(bool c, string name)
        {
            if (c) { pass++; Debug.Log("PLAYTEST PASS " + name); }
            else { fail++; Debug.LogError("PLAYTEST FAIL " + name); }
        }

        public static void FullLoop()
        {
            pass = 0;
            fail = 0;
            GameData d = GameSession.NewGame(2024);
            Check(d.crew.Count == 1 && d.world.islands.Count == 5, "newgame");
            d.AddResource(ResourceId.Food, 100);
            d.AddResource(ResourceId.Water, 100);
            d.AddResource(ResourceId.Money, 5000);
            d.AddResource(ResourceId.Wood, 60);
            d.AddResource(ResourceId.Metal, 30);
            d.AddResource(ResourceId.Medicine, 10);

            // recruta 1 + relacoes
            string rid = null;
            for (int i = 0; i < d.roster.Count; i++) { rid = d.roster[i].id; break; }
            string m;
            GameSession.BuyShip(d, "boat", out m);
            Check(GameSession.Recruit(d, rid, out m) && d.relations.Count > 0, "recruit-relations");

            // modulo
            Check(ShipModules.Buy(d, "kitchen", out m), "module");

            // viagem completa com evento
            string dest = d.world.islands[3].id;
            VoyageReport b = GameSession.BeginTravel(d, dest);
            Check(b.ok, "begin");
            int guard = 0;
            bool arrived = false;
            while (guard < 20 && d.sailDestId != "")
            {
                guard++;
                VoyageReport t = GameSession.TravelTick(d);
                if (t.tick == TickResult.NeedChoice)
                {
                    VoyageReport c = GameSession.ChooseEvent(d, 0);
                    if (c.needCombat)
                    {
                        string bt = GameSession.ResolveBattle(d, c.combatDanger, 900, false);
                        Check(bt.Length > 0, "event-combat");
                    }
                }
                if (t.tick == TickResult.Arrived) arrived = true;
            }
            Check(arrived && d.currentIslandId == dest, "arrive");

            // comercio + missao
            Check(GameSession.Buy(d, dest, ResourceId.Food, 2, out m), "buy");
            Check(GameSession.Sell(d, dest, ResourceId.Food, 1, out m), "sell");
            string mid = null;
            for (int i = 0; i < d.missions.Count; i++)
            {
                if (d.missions[i].status == MissionStatus.Available) { mid = d.missions[i].id; break; }
            }
            Check(mid != null && GameSession.AcceptMission(d, mid, out m), "mission-accept");

            // combate por turnos ate o fim
            List<CharacterData> party = new List<CharacterData>();
            for (int i = 0; i < d.crew.Count && party.Count < 4; i++)
            {
                if (d.crew[i].alive) party.Add(d.crew[i]);
            }
            List<EnemyData> enemies = CombatSystem.GenerateEnemies(4242, 3, d.world.day);
            BattleState bs = BattleState.Start(party, enemies, 1, d.GetResource(ResourceId.Medicine), 777);
            int rounds = 0;
            while (!bs.over && rounds < 300)
            {
                rounds++;
                if (bs.CurrentCrew() == null) break;
                bs.Act(BattleAction.Attack, 0);
            }
            Check(bs.over, "battle-over");
            d.AddResource(ResourceId.Medicine, -bs.medicinesUsed);
            string end = GameSession.FinishBattle(d, party, bs.DeadIds(), bs.victory, bs.fled, 3, bs.log);
            Check(end.Length > 0, "battle-finish");

            // save/load no engine (DataContract + arquivo)
            string path = System.IO.Path.GetTempPath() + "/cj_playtest.json";
            SaveSystem.SaveToFile(d, path);
            GameData l = SaveSystem.LoadFromFile(path);
            Check(l.world.day == d.world.day && l.crew.Count == d.crew.Count && l.journal.Count > 3, "save-load");
            Check(l.memorial.Count == d.memorial.Count, "memorial");

            // diario + reputacao moveram
            Check(d.journal.Count > 5 && d.notoriety >= 0, "journal");

            Debug.Log("PLAYTEST RESULT pass=" + pass + " fail=" + fail);
            if (fail > 0) throw new System.Exception("playtest falhou: " + fail);
        }
    }
#endif
}
