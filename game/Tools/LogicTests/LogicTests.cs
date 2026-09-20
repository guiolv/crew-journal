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

        Console.WriteLine("----");
        Console.WriteLine("pass=" + pass + " fail=" + fail);
        return fail == 0 ? 0 : 1;
    }
}
