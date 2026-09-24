// Harness console — compila com csc do .NET Framework + executa testes TDD secao 34.
// Uso: csc /t:exe /out:LogicTests.exe /r:System.Runtime.Serialization.dll <logic> LogicTests.cs
using System;
using System.Collections.Generic;
using CrewJournal.Logic;

public static class LogicTests
{
    static int pass = 0;
    static int fail = 0;

    static void Check(bool cond, string name)
    {
        if (cond) { pass++; Console.WriteLine("PASS " + name); }
        else { fail++; Console.WriteLine("FAIL " + name); }
    }

    public static int Main()
    {
        // 1. World deterministico
        WorldData w1 = WorldGenerator.Generate(1234);
        WorldData w2 = WorldGenerator.Generate(1234);
        Check(w1.islands.Count == 5, "world-5-islands");
        Check(w1.islands[0].displayName == w2.islands[0].displayName, "world-deterministic-name");
        Check(Math.Abs(w1.islands[3].x - w2.islands[3].x) < 0.001f, "world-deterministic-pos");

        // 2. Navigation deterministica
        GameData g1 = GameSession.NewGame(42);
        IslandData a = g1.world.islands[0];
        IslandData b = g1.world.islands[4];
        TravelResult t1 = NavigationSystem.Calculate(a, b, g1.ship, 50, b.danger);
        TravelResult t2 = NavigationSystem.Calculate(a, b, g1.ship, 50, b.danger);
        Check(t1.days == t2.days && Math.Abs(t1.risk - t2.risk) < 0.0001f, "nav-deterministic");
        Check(t1.days >= 1 && t1.risk >= 0.02f && t1.risk <= 0.85f, "nav-bounds");
        // Regra de design: navegador 99 + cautela REDUZEM, nunca eliminam o RNG.
        TravelResult tmax = NavigationSystem.Calculate(a, b, g1.ship, 99, b.danger);
        GameSession.ApplyStance(tmax, 1);
        Check(tmax.risk >= 0.02f && tmax.eventChance >= 0.05f, "nav-floor");
        TravelResult tmarch = NavigationSystem.Calculate(a, b, g1.ship, 10, b.danger);
        GameSession.ApplyStance(tmarch, 2);
        Check(tmarch.risk > tmax.risk, "stance-tradeoff");

        // 3. Character valido
        CharacterData c = CharacterGenerator.Generate(42, 5);
        Check(c.alive && c.stats.hp > 0 && c.jobs.Count > 0 && c.hireCost >= 50 && c.hireCost <= 250, "char-valid");

        // 4. Economy: preco nunca abaixo do piso
        bool floorOk = true;
        foreach (IslandArchetype arch in Enum.GetValues(typeof(IslandArchetype)))
        {
            foreach (ResourceId rid in new ResourceId[] { ResourceId.Food, ResourceId.Water, ResourceId.Wood, ResourceId.Metal, ResourceId.Medicine })
            {
                int p = EconomySystem.PriceFor(rid, 0.1f, 100);
                if (p < 1 || p < Balance.BasePriceOf(rid) / 2) floorOk = false;
            }
        }
        Check(floorOk, "economy-floor");

        // 5. Combat deterministico
        GameData g2 = GameSession.NewGame(7);
        GameData g3 = GameSession.NewGame(7);
        string r1 = GameSession.ResolveBattle(g2, 3, 99, false);
        string r2 = GameSession.ResolveBattle(g3, 3, 99, false);
        Check(r1 == r2, "combat-deterministic");

        // 6. Save round-trip
        GameData g4 = GameSession.NewGame(11);
        TravelReport tr = GameSession.Travel(g4, g4.world.islands[2].id);
        string json = SaveSystem.Serialize(g4);
        GameData g5 = SaveSystem.Deserialize(json);
        Check(g5.world.day == g4.world.day && g5.crew.Count == g4.crew.Count
            && g5.GetResource(ResourceId.Money) == g4.GetResource(ResourceId.Money)
            && g5.currentIslandId == g4.currentIslandId, "save-roundtrip");

        // 7. Death permanente
        GameData g6 = GameSession.NewGame(13);
        string deadId = g6.crew[0].id;
        GameSession.RegisterDeath(g6, deadId, "teste");
        bool stillThere = false;
        for (int i = 0; i < g6.crew.Count; i++) if (g6.crew[i].id == deadId) stillThere = true;
        Check(!stillThere && g6.memorial.Count == 1, "death-permanent");

        // 8. NewGame respeita GED (recursos iniciais)
        GameData g7 = GameSession.NewGame(1);
        Check(g7.GetResource(ResourceId.Money) == 100 && g7.GetResource(ResourceId.Food) == 8
            && g7.GetResource(ResourceId.Water) == 8 && g7.ship.defId == "barrel", "ged-start");

        // 9. Travel consome e avanca dia
        GameData g8 = GameSession.NewGame(21);
        int day0 = g8.world.day;
        TravelReport rep = GameSession.Travel(g8, g8.world.islands[1].id);
        Check(rep.ok && g8.world.day > day0 && g8.currentIslandId == g8.world.islands[1].id, "travel-advances");

        // 10. Weather deterministico
        WeatherKind wa = GameSession.RollWeather(42, 5, 2);
        WeatherKind wb = GameSession.RollWeather(42, 5, 2);
        Check(wa == wb, "weather-deterministic");
        int stormCalm = 0, stormWild = 0;
        for (int i = 0; i < 200; i++)
        {
            if (GameSession.RollWeather(i, i * 3, i * 7, 1) == WeatherKind.Storm) stormCalm++;
            if (GameSession.RollWeather(i, i * 3, i * 7, 5) == WeatherKind.Storm) stormWild++;
        }
        Check(stormWild > stormCalm, "weather-storm-bias");

        // 11. Voyage completo: preview -> begin -> ticks -> chegada
        GameData g9 = GameSession.NewGame(31);
        g9.AddResource(ResourceId.Food, 50);
        g9.AddResource(ResourceId.Water, 50);
        string dest = g9.world.islands[1].id;
        TravelResult pv;
        WeatherKind pw;
        string perr;
        bool pok = GameSession.PreviewTravel(g9, dest, out pv, out pw, out perr);
        Check(pok, "voyage-preview");
        VoyageReport br = GameSession.BeginTravel(g9, dest);
        Check(br.ok && g9.sailDestId == dest, "voyage-begin");
        int guard = 0;
        VoyageReport last = br;
        while (guard < 20)
        {
            guard++;
            if (g9.sailDestId == "") break;
            VoyageReport t = GameSession.TravelTick(g9);
            last = t;
            if (t.tick == TickResult.NeedChoice)
            {
                VoyageReport ch = GameSession.ChooseEvent(g9, 1);
                if (ch.needCombat)
                {
                    string bt = GameSession.ResolveBattle(g9, ch.combatDanger, 500 + guard, false);
                    Check(bt.Length > 0, "voyage-event-combat");
                }
            }
            if (t.tick == TickResult.Arrived) break;
        }
        Check(g9.currentIslandId == dest, "voyage-arrived");

        // 12. Escolhas de evento executam sem erro
        GameData g10 = GameSession.NewGame(77);
        bool chOk = true;
        TravelEventKind[] kinds = new TravelEventKind[] { TravelEventKind.Storm, TravelEventKind.AbandonedShip, TravelEventKind.UnknownShip, TravelEventKind.SeaCreature, TravelEventKind.Whirlpool };
        for (int k = 0; k < kinds.Length; k++)
        {
            for (int o = 0; o < 3; o++)
            {
                bool nc;
                int cd;
                string txt = TravelEvents.ApplyChoice(g10, kinds[k], o, new Random(1 + k * 3 + o), out nc, out cd);
                if (txt == null || txt.Length == 0) chOk = false;
            }
        }
        Check(chOk, "event-choices");

        // 13. BattleState deterministico + termina
        GameData ga = GameSession.NewGame(91);
        GameData gb = GameSession.NewGame(91);
        List<CharacterData> pa = new List<CharacterData>();
        List<CharacterData> pb = new List<CharacterData>();
        for (int i = 0; i < ga.crew.Count; i++) pa.Add(ga.crew[i]);
        for (int i = 0; i < gb.crew.Count; i++) pb.Add(gb.crew[i]);
        List<EnemyData> ea = CombatSystem.GenerateEnemies(1000, 2, 1);
        List<EnemyData> eb = CombatSystem.GenerateEnemies(1000, 2, 1);
        BattleState sa = BattleState.Start(pa, ea, 0, 1, 555);
        BattleState sb = BattleState.Start(pb, eb, 0, 1, 555);
        string ta = sa.Act(BattleAction.Attack, 0);
        string tb = sb.Act(BattleAction.Attack, 0);
        Check(ta == tb, "battle-deterministic");
        int rounds = 0;
        while (!sa.over && rounds < 200)
        {
            rounds++;
            CharacterData cur = sa.CurrentCrew();
            if (cur == null) break;
            sa.Act(BattleAction.Attack, 0);
        }
        Check(sa.over, "battle-terminates");

        // 14. Modulos: barril recusa, barco aceita, slots respeitados
        GameData g11 = GameSession.NewGame(55);
        g11.AddResource(ResourceId.Money, 5000);
        g11.AddResource(ResourceId.Wood, 50);
        g11.AddResource(ResourceId.Metal, 20);
        string mm;
        Check(!ShipModules.Buy(g11, "kitchen", out mm), "modules-barrel-refused");
        string bm;
        GameSession.BuyShip(g11, "boat", out bm);
        Check(ShipModules.Buy(g11, "kitchen", out mm) && ShipModules.UsedSlots(g11.ship) == 1, "modules-buy");
        Check(ShipModules.EffectiveCrewCap(g11.ship) == 4, "modules-cap-base");
        ShipModules.Buy(g11, "dorm", out mm);
        Check(ShipModules.EffectiveCrewCap(g11.ship) == 6, "modules-dorm-effect");

        // 15. XP sobe nivel
        CharacterData cx = CharacterGenerator.Generate(5, 3);
        int ups = Progression.AddXp(cx, 250);
        Check(ups >= 1 && cx.level >= 2, "xp-levelup");

        // Ferido grave: primeiro golpe letal derruba p/ 1 HP, não mata.
        GameData g12 = GameSession.NewGame(200);
        CharacterData hero = g12.crew[0];
        hero.stats.hp = 3;
        List<CharacterData> gp = new List<CharacterData>();
        gp.Add(hero);
        List<EnemyData> ge = CombatSystem.GenerateEnemies(999, 5, 1);
        BattleState bst = BattleState.Start(gp, ge, 0, 0, 4242);
        int guardb = 0;
        while (!bst.over && guardb < 200)
        {
            guardb++;
            if (bst.CurrentCrew() == null) break;
            bst.Act(BattleAction.Defend, 0);
        }
        Check(bst.over && hero.grave, "grave-first");
        g12.AddResource(ResourceId.Medicine, 1);
        string tmsg;
        bool treated = hero.alive ? GameSession.TreatWound(g12, hero.id, out tmsg) : false;
        Check(!hero.alive || (treated && !hero.grave), "grave-treat");

        // Vínculos: evento de chegada não quebra e pode criar/aprofundar relação.
        GameData g13 = GameSession.NewGame(300);
        g13.AddResource(ResourceId.Money, 5000);
        string rc, bs2;
        GameSession.BuyShip(g13, "boat", out bs2);
        GameSession.Recruit(g13, g13.roster[0].id, out rc);
        VoyageReport dummy = new VoyageReport();
        int rel0 = g13.relations.Count;
        int j0 = g13.journal.Count;
        GameSession.BondEvent(g13, new Random(7), dummy);
        Check(g13.journal.Count >= j0, "bond-safe");

        // 16. DNA visual deterministico
        List<string> traits = new List<string>();
        traits.Add("corajoso");
        CharacterVisual va = VisualDNA.Character(42, "char_7", "Medico", traits, false, 0);
        CharacterVisual vb = VisualDNA.Character(42, "char_7", "Medico", traits, false, 0);
        Check(va.hair == vb.hair && va.outfit == vb.outfit && va.face == vb.face, "dna-deterministic");
        CharacterVisual vc = VisualDNA.Character(43, "char_7", "Medico", traits, false, 0);
        Check(va.hair != vc.hair || va.face != vc.face || va.skinTone != vc.skinTone, "dna-varies");

        // 17. Distribuicao por profissao (medico tende a oculos)
        int glasses = 0;
        for (int i = 0; i < 20; i++)
        {
            CharacterVisual m = VisualDNA.Character(1000 + i, "char_" + i, "Medico", traits, false, 0);
            if (m.accessory == 3) glasses++;
        }
        Check(glasses >= 5, "dna-job-distribution");

        // 18. Gramatica de ilha: capital tem castelo, perigosa tem boss
        IslandVisual cap = VisualDNA.Island(9, "isl_2", IslandArchetype.Capital);
        IslandVisual dan = VisualDNA.Island(9, "isl_4", IslandArchetype.Dangerous);
        bool hasCastle = false, hasBoss = false;
        for (int i = 0; i < cap.pieces.Count; i++) if (cap.pieces[i].kind == "castle") hasCastle = true;
        for (int i = 0; i < dan.pieces.Count; i++) if (dan.pieces[i].kind == "boss") hasBoss = true;
        Check(hasCastle && hasBoss, "island-grammar");
        IslandVisual cap2 = VisualDNA.Island(9, "isl_2", IslandArchetype.Capital);
        Check(cap.pieces.Count == cap2.pieces.Count && cap.biome == cap2.biome, "island-deterministic");

        // 19. DNA de navio por def
        ShipVisual sboat = VisualDNA.Ship("boat", 5, 0);
        ShipVisual smed = VisualDNA.Ship("medium", 5, 0);
        ShipVisual sbar = VisualDNA.Ship("barrel", 5, 0);
        Check(sboat.masts == 1 && smed.masts == 2 && sbar.masts == 0, "ship-dna");

        Console.WriteLine("----");
        Console.WriteLine("pass=" + pass + " fail=" + fail);
        return fail == 0 ? 0 : 1;
    }
}
