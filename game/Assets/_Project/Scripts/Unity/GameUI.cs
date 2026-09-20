// CrewJournal — UI inteira construida por codigo (uGUI). Sem prefabs.
// Padrao: rebuild completo a cada OnChanged. Listas truncadas (v0.1, sem scroll).
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrewJournal.Logic;

public class GameUI : MonoBehaviour
{
    GameManager gm;
    Font font;
    Text topBar;
    Text logText;
    GameObject content;
    string screen = "map";
    string msg = "";

    void Awake()
    {
        gm = GameManager.Instance;
        if (gm == null) gm = FindAnyObjectByType<GameManager>();
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        PlaceholderArt.Init();
        Build();
        gm.OnChanged += Refresh;
        Refresh();
    }

    // ---------- construcao ----------
    void Build()
    {
        GameObject canvas = new GameObject("Canvas");
        UnityEngine.Canvas c = canvas.AddComponent<UnityEngine.Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler s = canvas.AddComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1280, 720);
        canvas.AddComponent<GraphicRaycaster>();

        topBar = MkText(canvas.transform, 20);
        topBar.alignment = TextAnchor.MiddleLeft;
        Rect(topBar.gameObject, 0, 1, 1, 1, 10, -40, -10, 0);

        logText = MkText(canvas.transform, 15);
        Rect(logText.gameObject, 0, 0, 1, 0, 10, 60, -10, 170);

        content = new GameObject("Content");
        content.transform.SetParent(canvas.transform, false);
        Rect(content, 0, 0, 1, 1, 10, 180, -10, -50);
        Image bg = content.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.08f, 0.12f, 0.9f);

        GameObject bar = new GameObject("Bar");
        bar.transform.SetParent(canvas.transform, false);
        Rect(bar, 0, 0, 1, 0, 0, 0, 0, 50);
        HorizontalLayoutGroup hg = bar.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 6;
        hg.padding = new RectOffset(10, 10, 6, 6);
        MkButton(bar.transform, "Mapa", delegate { screen = "map"; Refresh(); });
        MkButton(bar.transform, "Ilha", delegate { screen = "island"; Refresh(); });
        MkButton(bar.transform, "Tripulacao", delegate { screen = "crew"; Refresh(); });
        MkButton(bar.transform, "Diario", delegate { screen = "journal"; Refresh(); });
        MkButton(bar.transform, "Salvar", delegate { msg = gm.Save(); Refresh(); });
        MkButton(bar.transform, "Carregar", delegate { msg = gm.Load(); Refresh(); });
        MkButton(bar.transform, "Novo", delegate { gm.NewGame(gm.Seed + 1); screen = "map"; msg = "Nova jornada!"; Refresh(); });
    }

    // ---------- refresh ----------
    void Refresh()
    {
        if (gm == null || gm.Data == null) return;
        GameData d = gm.Data;
        topBar.text = string.Format("Dia {0} | {1}$ | Comida {2} | Agua {3} | Madeira {4} | Metal {5} | Med {6} | Navio {7} ({8}/{9}) | Trip {10}/{11}",
            d.world.day, d.GetResource(ResourceId.Money), d.GetResource(ResourceId.Food), d.GetResource(ResourceId.Water),
            d.GetResource(ResourceId.Wood), d.GetResource(ResourceId.Metal), d.GetResource(ResourceId.Medicine),
            d.ship.defId, d.ship.hull, d.ship.maxHull, GameSession.AliveCrew(d), d.ship.crewCap);
        int from = Math.Max(0, d.journal.Count - 6);
        string log = "";
        for (int i = from; i < d.journal.Count; i++) log += "[d" + d.journal[i].day + "] " + d.journal[i].text + "\n";
        if (msg != "") log += "> " + msg + "\n";
        if (gm.LastReport != null && gm.LastReport.ok)
        {
            for (int i = 0; i < gm.LastReport.log.Count; i++) log += "* " + gm.LastReport.log[i] + "\n";
            if (gm.LastReport.eventText != "") log += "! " + gm.LastReport.eventText + "\n";
        }
        logText.text = log;
        Clear(content.transform);
        if (gm.State == GameState.Sailing) { MkLabel(content.transform, "Navegando..."); return; }
        if (gm.State == GameState.GameOver) { MkLabel(content.transform, "GAME OVER — toda a tripulacao morreu. Clique em Novo."); return; }
        if (gm.State == GameState.Combat || gm.PendingCombat) { ShowCombat(d); return; }
        if (screen == "island") ShowIsland(d);
        else if (screen == "crew") ShowCrew(d);
        else if (screen == "journal") ShowJournal(d);
        else ShowMap(d);
    }

    void ShowMap(GameData d)
    {
        MkLabel(content.transform, "MAPA — escolha o destino (clique para navegar):");
        IslandData from = d.FindIsland(d.currentIslandId);
        for (int i = 0; i < d.world.islands.Count; i++)
        {
            IslandData isl = d.world.islands[i];
            string here = (isl.id == d.currentIslandId) ? " [VOCE ESTA AQUI]" : "";
            int days = 1;
            if (from != null && from.id != isl.id)
                days = NavigationSystem.Calculate(from, isl, d.ship, GameSession.BestNavigator(d), isl.danger).days;
            string label = isl.displayName + " [" + isl.archetype.ToString() + "] perigo " + isl.danger + " ~" + days + "d" + here;
            string id = isl.id;
            if (isl.id == d.currentIslandId)
                MkLabel(content.transform, label);
            else
                MkButton(content.transform, "Viajar: " + label, delegate
                {
                    screen = "island";
                    gm.TravelTo(id);
                });
        }
    }

    void ShowIsland(GameData d)
    {
        IslandData isl = d.FindIsland(d.currentIslandId);
        if (isl == null) { MkLabel(content.transform, "Em alto-mar."); return; }
        MkLabel(content.transform, isl.displayName + " [" + isl.archetype.ToString() + "] perigo " + isl.danger + " | Rep: " + d.GetRep(isl.id));
        MkLabel(content.transform, "-- Comercio (preco unidade) --");
        ResourceId[] ids = new ResourceId[] { ResourceId.Food, ResourceId.Water, ResourceId.Wood, ResourceId.Metal, ResourceId.Medicine };
        for (int i = 0; i < ids.Length; i++)
        {
            ResourceId r = ids[i];
            int buy = EconomySystem.PriceFor(r, WorldGenerator.IslandMult(isl, r), d.GetRep(isl.id));
            int sell = EconomySystem.SellPriceFor(r, WorldGenerator.IslandMult(isl, r), d.GetRep(isl.id));
            MkButton(content.transform, string.Format("Comprar {0} ({1}$) [tem {2}]", r, buy, d.GetResource(r)), delegate
            {
                string m; GameSession.Buy(d, isl.id, r, 1, out m); msg = m; Refresh();
            });
            MkButton(content.transform, string.Format("Vender {0} ({1}$)", r, sell), delegate
            {
                string m; GameSession.Sell(d, isl.id, r, 1, out m); msg = m; Refresh();
            });
        }
        MkLabel(content.transform, "-- Recrutamento --");
        int shown = 0;
        for (int i = 0; i < isl.recruitIds.Count && shown < 4; i++)
        {
            CharacterData c = GameSession.FindChar(d, isl.recruitIds[i]);
            if (c == null) continue;
            bool inRoster = false;
            for (int k = 0; k < d.roster.Count; k++) if (d.roster[k].id == c.id) inRoster = true;
            if (!inRoster) continue;
            shown++;
            string job = c.jobs.Count > 0 ? c.jobs[0] : "?";
            string id = c.id;
            MkButton(content.transform, string.Format("Recrutar {0} [{1} nav:{2} lut:{3}] ({4}$)", c.displayName, job, c.nav, c.fighter, c.hireCost), delegate
            {
                string m; GameSession.Recruit(d, id, out m); msg = m; Refresh();
            });
        }
        if (shown == 0) MkLabel(content.transform, "(sem recrutas aqui)");
        MkLabel(content.transform, "-- Missoes --");
        for (int i = 0; i < d.missions.Count; i++)
        {
            MissionData m = d.missions[i];
            if (m.status != MissionStatus.Available && m.status != MissionStatus.Accepted) continue;
            string id = m.id;
            string label = m.title + " [" + m.type + "] recompensa " + m.reward + " (" + m.status + ")";
            if (m.status == MissionStatus.Available)
                MkButton(content.transform, "Aceitar: " + label, delegate
                {
                    string mm; GameSession.AcceptMission(d, id, out mm); msg = mm; Refresh();
                });
            else
                MkLabel(content.transform, label);
        }
        MkLabel(content.transform, "-- Navio --");
        MkButton(content.transform, "Reparar casco", delegate
        {
            string m; GameSession.Repair(d, out m); msg = m; Refresh();
        });
        if (d.ship.defId == "barrel")
            MkButton(content.transform, "Comprar Pequeno Barco (500$)", delegate
            {
                string m; GameSession.BuyShip(d, "boat", out m); msg = m; Refresh();
            });
        else if (d.ship.defId == "boat")
            MkButton(content.transform, "Comprar Navio Medio (2500$)", delegate
            {
                string m; GameSession.BuyShip(d, "medium", out m); msg = m; Refresh();
            });
    }

    void ShowCombat(GameData d)
    {
        MkLabel(content.transform, "COMBATE! Perigo " + gm.PendingCombatDanger);
        List<EnemyData> en = CombatSystem.GenerateEnemies(d.world.seed + d.world.day, gm.PendingCombatDanger, d.world.day);
        for (int i = 0; i < en.Count; i++)
            MkLabel(content.transform, en[i].name + " HP " + en[i].hp + "/" + en[i].maxHp + " ATK " + en[i].atk);
        if (gm.LastBattleText != "") MkLabel(content.transform, gm.LastBattleText);
        MkButton(content.transform, "LUTAR", delegate { gm.ResolvePendingBattle(false); screen = "island"; });
        MkButton(content.transform, "FUGIR (45%)", delegate { gm.ResolvePendingBattle(true); screen = "island"; });
    }

    void ShowCrew(GameData d)
    {
        MkLabel(content.transform, "-- Tripulacao --");
        for (int i = 0; i < d.crew.Count; i++)
        {
            CharacterData c = d.crew[i];
            if (!c.alive) continue;
            string job = c.jobs.Count > 0 ? c.jobs[0] : "?";
            MkLabel(content.transform, string.Format("{0} [{1}] HP {2}/{3} nav:{4} lut:{5} moral:{6} salario:{7}/d",
                c.displayName, job, c.stats.hp, c.stats.maxHp, c.nav, c.fighter, c.morale, c.wage));
        }
    }

    void ShowJournal(GameData d)
    {
        MkLabel(content.transform, "-- Diario da Tripulacao --");
        int from = Math.Max(0, d.journal.Count - 14);
        for (int i = from; i < d.journal.Count; i++)
            MkLabel(content.transform, "[d" + d.journal[i].day + "] " + d.journal[i].text);
        MkLabel(content.transform, "-- Memorial --");
        if (d.memorial.Count == 0) MkLabel(content.transform, "(nenhum morto — por enquanto)");
        for (int i = 0; i < d.memorial.Count; i++)
            MkLabel(content.transform, d.memorial[i].name + " d" + d.memorial[i].dayDied + " (" + d.memorial[i].cause + ")");
    }

    // ---------- helpers ----------
    void Clear(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--) Destroy(t.GetChild(i).gameObject);
    }

    Text MkText(Transform parent, int size)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text tx = go.AddComponent<Text>();
        tx.font = font;
        tx.fontSize = size;
        tx.color = Color.white;
        return tx;
    }

    void MkLabel(Transform parent, string s)
    {
        Text tx = MkText(parent, 16);
        tx.text = s;
    }

    void MkButton(Transform parent, string label, Action onClick)
    {
        GameObject go = new GameObject("Btn");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.25f, 0.4f);
        Button b = go.AddComponent<Button>();
        b.onClick.AddListener(delegate { msg = ""; onClick(); });
        Text tx = MkText(go.transform, 15);
        tx.text = label;
        tx.alignment = TextAnchor.MiddleCenter;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 30);
    }

    void Rect(GameObject go, float ax0, float ay0, float ax1, float ay1, float ox0, float oy0, float ox1, float oy1)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(ax0, ay0);
        rt.anchorMax = new Vector2(ax1, ay1);
        rt.offsetMin = new Vector2(ox0, oy0);
        rt.offsetMax = new Vector2(ox1, oy1);
    }
}
