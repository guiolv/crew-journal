// CrewJournal — Logic pura (sem UnityEngine). Compilavel com csc + testes console.
// TDD secao 5 (GameState), GED recursos/missoes. C# 5 compativel.
using System;

namespace CrewJournal.Logic
{
    public enum GameState
    {
        Boot,
        Map,
        Sailing,
        Island,
        Event,
        Combat,
        Menu,
        GameOver
    }

    public enum IslandArchetype
    {
        FishingVillage,
        TradingPort,
        Capital,
        Uninhabited,
        Dangerous
    }

    public enum ResourceId
    {
        Money = 0,
        Food = 1,
        Water = 2,
        Wood = 3,
        Metal = 4,
        Medicine = 5
    }

    public enum MissionType
    {
        Transport,
        Trade,
        Rescue,
        Combat,
        Explore
    }

    public enum MissionStatus
    {
        Available,
        Accepted,
        InProgress,
        Completed,
        Failed,
        Abandoned
    }

    public enum TravelEventKind
    {
        None,
        AbandonedShip,
        Storm,
        UnknownShip,
        SeaCreature,
        CalmBonus
    }
}
