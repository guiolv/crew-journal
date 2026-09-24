// CrewJournal — ponte Unity <-> logica pura. Fluxo: detalhe -> viagem -> evento -> chegada -> combate.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrewJournal.Logic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameData Data;
    public GameState State = GameState.Boot;
    public int Seed = 12345;

    public string PreviewDest = "";
    public TravelResult Preview;
    public WeatherKind PreviewWeather;

    public BattleState Battle;
    public int PendingCombatDanger;
    public string EventChoiceText = "";
    public string LastBattleText = "";
    public VoyageReport LastVoyage;
    public float sailTick = 1.1f;
    public bool stepping;

    public event Action OnChanged;
    public event Action OnEnemyHit;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        NewGame(Seed);
    }

    public void Notify()
    {
        if (OnChanged != null) OnChanged();
    }

    public void NewGame(int seed)
    {
        Seed = seed;
        Data = GameSession.NewGame(seed);
        State = GameState.Map;
        Battle = null;
        Preview = null;
        PreviewDest = "";
        EventChoiceText = "";
        LastBattleText = "";
        LastVoyage = null;
        sailTick = 1.1f;
        stepping = false;
        Notify();
    }

    public void SelectDestination(string islandId)
    {
        PreviewDest = islandId;
        TravelResult calc;
        WeatherKind w;
        string err;
        if (GameSession.PreviewTravel(Data, islandId, out calc, out w, out err))
        {
            Preview = calc;
            PreviewWeather = w;
        }
        else
        {
            Preview = null;
        }
        Notify();
    }

    public string PreviewError(string islandId)
    {
        TravelResult calc;
        WeatherKind w;
        string err;
        GameSession.PreviewTravel(Data, islandId, out calc, out w, out err);
        return err;
    }

    public void BeginVoyage(string islandId)
    {
        if (State == GameState.Sailing || State == GameState.Combat || State == GameState.Event) return;
        VoyageReport rep = GameSession.BeginTravel(Data, islandId);
        LastVoyage = rep;
        if (!rep.ok)
        {
            Notify();
            return;
        }
        EventChoiceText = rep.eventText;
        sailTick = 1.1f;
        StartCoroutine(SailRoutine());
    }

    public void FastForward()
    {
        if (State == GameState.Sailing) sailTick = 0.25f;
    }

    IEnumerator SailRoutine()
    {
        State = GameState.Sailing;
        Notify();
        while (true)
        {
            yield return new WaitForSeconds(sailTick);
            VoyageReport t = GameSession.TravelTick(Data);
            LastVoyage = t;
            if (t.tick == TickResult.NeedChoice)
            {
                State = GameState.Event;
                Notify();
                yield break;
            }
            if (t.tick == TickResult.Arrived)
            {
                AfterArrival(t);
                yield break;
            }
            Notify();
        }
    }

    public void ChooseEventOption(int idx)
    {
        if (State != GameState.Event) return;
        VoyageReport c = GameSession.ChooseEvent(Data, idx);
        EventChoiceText = c.message;
        if (c.needCombat)
        {
            StartBattle(c.combatDanger);
            return;
        }
        StartCoroutine(SailRoutine());
    }

    void AfterArrival(VoyageReport rep)
    {
        if (rep.needCombat)
        {
            StartBattle(rep.combatDanger);
            return;
        }
        State = GameSession.IsGameOver(Data) ? GameState.GameOver : GameState.Island;
        Notify();
    }

    public void StartBattle(int danger)
    {
        List<CharacterData> party = new List<CharacterData>();
        for (int i = 0; i < Data.crew.Count && party.Count < 4; i++)
        {
            if (Data.crew[i].alive) party.Add(Data.crew[i]);
        }
        if (party.Count == 0)
        {
            State = GameState.GameOver;
            Notify();
            return;
        }
        List<EnemyData> enemies = CombatSystem.GenerateEnemies(Data.world.seed + Data.world.day, danger, Data.world.day);
        Battle = BattleState.Start(party, enemies, ShipModules.EquipBonus(Data.ship), Data.GetResource(ResourceId.Medicine), Data.world.seed + Data.world.day * 3 + danger);
        PendingCombatDanger = danger;
        State = GameState.Combat;
        Notify();
        if (Battle.CurrentCrew() == null && !Battle.over) StartCoroutine(EnemySteps());
    }

    public void BattleAct(BattleAction a, int target)
    {
        if (Battle == null || State != GameState.Combat || stepping) return;
        LastBattleText = Battle.Act(a, target);
        Data.SetResource(ResourceId.Medicine, Math.Max(0, Battle.medicines - Battle.medicinesUsed));
        if (Battle.over)
        {
            EndBattle();
            return;
        }
        Notify();
        StartCoroutine(EnemySteps());
    }

    IEnumerator EnemySteps()
    {
        stepping = true;
        while (Battle != null && !Battle.over && Battle.EnemyTurnPending())
        {
            yield return new WaitForSeconds(0.35f);
            if (Battle == null || Battle.over) break;
            Battle.StepEnemy();
            if (OnEnemyHit != null) OnEnemyHit();
            Notify();
        }
        stepping = false;
        if (Battle != null && Battle.over) EndBattle();
        else Notify();
    }

    void EndBattle()
    {
        if (Battle.victory) Sfx.Victory();
        else if (!Battle.fled) Sfx.Defeat();
        LastBattleText = GameSession.FinishBattle(Data, Battle.party, Battle.DeadIds(), Battle.victory, Battle.fled, PendingCombatDanger, Battle.log);
        Battle = null;
        State = GameSession.IsGameOver(Data) ? GameState.GameOver : GameState.Island;
        Notify();
    }

    public void BackToMap()
    {
        if (State == GameState.Sailing || State == GameState.Combat || State == GameState.Event) return;
        State = GameState.Map;
        Notify();
    }

    public string SavePath()
    {
        return Application.persistentDataPath + "/save_01.json";
    }

    public string Save()
    {
        try
        {
            SaveSystem.SaveToFile(Data, SavePath());
            GameSession.AddJournal(Data, "Progresso salvo no dia " + Data.world.day + ".");
            Notify();
            return "Jogo salvo.";
        }
        catch (Exception e)
        {
            return "Falha ao salvar: " + e.Message;
        }
    }

    public string Load()
    {
        try
        {
            Data = SaveSystem.LoadFromFile(SavePath());
            // estado volatil nao sobrevive ao load: cancela viagem/batalha em curso
            Data.sailDestId = "";
            Data.sailDay = 0;
            Data.sailTotal = 0;
            Data.pendingEvent = TravelEventKind.None;
            Battle = null;
            Preview = null;
            PreviewDest = "";
            sailTick = 1.1f;
            stepping = false;
            State = GameState.Map;
            Notify();
            return "Save carregado (dia " + Data.world.day + ").";
        }
        catch (Exception e)
        {
            return "Falha ao carregar: " + e.Message;
        }
    }
}
