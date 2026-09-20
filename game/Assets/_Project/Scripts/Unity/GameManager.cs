// CrewJournal — ponte Unity <-> logica pura. MonoBehaviour singleton.
// Cena unica (World). UI em GameUI assina OnChanged.
using System;
using System.Collections;
using UnityEngine;
using CrewJournal.Logic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameData Data;
    public GameState State = GameState.Boot;
    public TravelReport LastReport;
    public string LastBattleText = "";
    public bool PendingCombat;
    public int PendingCombatDanger;
    public int Seed = 12345;

    public event Action OnChanged;

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
        PendingCombat = false;
        LastReport = null;
        LastBattleText = "";
        Notify();
    }

    public void TravelTo(string islandId)
    {
        if (State == GameState.Sailing || State == GameState.Combat) return;
        StartCoroutine(TravelRoutine(islandId));
    }

    IEnumerator TravelRoutine(string islandId)
    {
        State = GameState.Sailing;
        Notify();
        yield return new WaitForSeconds(1.2f);
        TravelReport rep = GameSession.Travel(Data, islandId);
        LastReport = rep;
        if (!rep.ok)
        {
            State = GameState.Island;
            Notify();
            yield break;
        }
        if (rep.needCombat)
        {
            PendingCombat = true;
            PendingCombatDanger = rep.combatDanger;
            State = GameState.Combat;
        }
        else
        {
            State = GameState.Island;
        }
        if (GameSession.IsGameOver(Data)) State = GameState.GameOver;
        Notify();
    }

    public void ResolvePendingBattle(bool flee)
    {
        if (!PendingCombat) return;
        LastBattleText = GameSession.ResolveBattle(Data, PendingCombatDanger, Data.world.day, flee);
        PendingCombat = false;
        State = GameSession.IsGameOver(Data) ? GameState.GameOver : GameState.Island;
        Notify();
    }

    public void BackToMap()
    {
        if (State == GameState.Sailing || State == GameState.Combat) return;
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
            State = GameState.Map;
            PendingCombat = false;
            Notify();
            return "Save carregado (dia " + Data.world.day + ").";
        }
        catch (Exception e)
        {
            return "Falha ao carregar: " + e.Message;
        }
    }
}
