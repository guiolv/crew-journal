// Harness de evals do SPEC-ISLAND-CJ (RED: espera-se BLOCKED ate a implementacao).
// Separado do LogicTests principal para nao quebrar a suite verde (38/38).
// Uso: csc /t:exe /out:IslandEvals.exe /r:System.Runtime.Serialization.dll <logic> IslandEvals.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using CrewJournal.Logic;

public static class IslandEvals
{
    static int pass = 0;
    static int fail = 0;
    static int blocked = 0;

    static void Eval(bool cond, string name)
    {
        if (cond) { pass++; Console.WriteLine("PASS " + name); }
        else { fail++; Console.WriteLine("FAIL " + name); }
    }

    static void Block(string name, string why)
    {
        blocked++;
        Console.WriteLine("BLOCKED " + name + " (" + why + ")");
    }

    static bool IsLand(IslandTile t) { return t == IslandTile.Sand || t == IslandTile.Grass; }

    public static int Main()
    {
        int[] sizes = new int[] { 8, 16, 32, 64 };
        try
        {
            // EVAL_EdgesOcean (I1)
            bool edgesOk = true;
            foreach (int n in sizes)
            {
                IslandLayout l = IslandLayout.Generate(n, n, 1337);
                for (int i = 0; i < n; i++)
                {
                    if (l.map[i, 0] != IslandTile.Ocean) edgesOk = false;
                    if (l.map[i, n - 1] != IslandTile.Ocean) edgesOk = false;
                    if (l.map[0, i] != IslandTile.Ocean) edgesOk = false;
                    if (l.map[n - 1, i] != IslandTile.Ocean) edgesOk = false;
                }
            }
            Eval(edgesOk, "EVAL_EdgesOcean");

            // EVAL_HasLand (R4)
            bool landOk = true;
            foreach (int n in sizes)
            {
                IslandLayout l = IslandLayout.Generate(n, n, 1337);
                bool any = false;
                for (int x = 0; x < n && !any; x++)
                    for (int y = 0; y < n && !any; y++)
                        if (IsLand(l.map[x, y])) any = true;
                if (!any) landOk = false;
            }
            Eval(landOk, "EVAL_HasLand");

            // EVAL_Adjacency (I2: Grass nunca ortogonal a Ocean)
            bool adjOk = true;
            foreach (int n in sizes)
            {
                IslandLayout l = IslandLayout.Generate(n, n, 1337);
                for (int x = 1; x < n - 1 && adjOk; x++)
                    for (int y = 1; y < n - 1 && adjOk; y++)
                        if (l.map[x, y] == IslandTile.Grass)
                        {
                            if (l.map[x + 1, y] == IslandTile.Ocean) adjOk = false;
                            if (l.map[x - 1, y] == IslandTile.Ocean) adjOk = false;
                            if (l.map[x, y + 1] == IslandTile.Ocean) adjOk = false;
                            if (l.map[x, y - 1] == IslandTile.Ocean) adjOk = false;
                        }
            }
            Eval(adjOk, "EVAL_Adjacency");

            // EVAL_SpawnOnLand (I3)
            bool spawnOk = true;
            foreach (int n in sizes)
            {
                IslandLayout l = IslandLayout.Generate(n, n, 1337);
                if (!IsLand(l.map[l.spawnX, l.spawnY])) spawnOk = false;
            }
            Eval(spawnOk, "EVAL_SpawnOnLand");

            // EVAL_Deterministic (I4)
            IslandLayout a = IslandLayout.Generate(32, 32, 4242);
            IslandLayout b = IslandLayout.Generate(32, 32, 4242);
            bool detOk = a.spawnX == b.spawnX && a.spawnY == b.spawnY;
            for (int x = 0; x < 32 && detOk; x++)
                for (int y = 0; y < 32 && detOk; y++)
                    if (a.map[x, y] != b.map[x, y]) detOk = false;
            Eval(detOk, "EVAL_Deterministic");

            // EVAL_DistinctSeeds
            IslandLayout c = IslandLayout.Generate(32, 32, 1111);
            IslandLayout d = IslandLayout.Generate(32, 32, 2222);
            bool diffOk = false;
            for (int x = 0; x < 32 && !diffOk; x++)
                for (int y = 0; y < 32 && !diffOk; y++)
                    if (c.map[x, y] != d.map[x, y]) diffOk = true;
            Eval(diffOk, "EVAL_DistinctSeeds");

            // EVAL_Perf64 (I5: <50ms)
            Stopwatch sw = Stopwatch.StartNew();
            IslandLayout.Generate(64, 64, 1337);
            sw.Stop();
            Console.WriteLine("perf64_ms=" + sw.ElapsedMilliseconds);
            Eval(sw.ElapsedMilliseconds < 50, "EVAL_Perf64");
        }
        catch (NotImplementedException e)
        {
            Block("SPEC-ISLAND-CJ", e.Message);
        }
        Console.WriteLine("----");
        Console.WriteLine("pass=" + pass + " fail=" + fail + " blocked=" + blocked);
        return (fail == 0 && blocked == 0) ? 0 : 1;
    }
}
