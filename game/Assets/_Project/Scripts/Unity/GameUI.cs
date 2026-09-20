// CrewJournal — UI por codigo, tema refs: navy profundo + pergaminho + teal + dourado.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrewJournal.Logic;

public class GameUI : MonoBehaviour
{
    static readonly Color Navy = new Color(0.04f, 0.08f, 0.15f, 0.97f);
    static readonly Color Panel = new Color(0.07f, 0.14f, 0.23f, 0.95f);
    static readonly Color Parch = new Color(0.90f, 0.83f, 0.64f);
    static readonly Color Ink = new Color(0.23f, 0.16f, 0.09f);
    static readonly Color Teal = new Color(0.11f, 0.36f, 0.43f);
    static readonly Color Gold = new Color(0.79f, 0.64f, 0.15f);
    static readonly Color Red = new Color(0.55f, 0.16f, 0.16f);

    GameManager gm;
    Font font;
    Text topBar;
    Text logText;
    GameObject content;
    string screen = "map";
    string msg = "";
    string selChar = "";
    int selTarget;
    string journalTab = "Todos";

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

    void Build()
    {
        GameObject canvas = new GameObject("Canvas");
        UnityEngine.Canvas c = canvas.AddComponent<UnityEngine.Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler s = canvas.AddComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1280, 720);
        canvas.AddComponent<GraphicRaycaster>();

        topBar = MkText(canvas.transform, 19);
        topBar.alignment = TextAnchor.MiddleLeft;
        topBar.color = Gold;
        Rect(topBar.gameObject, 0, 1, 1, 1, 10, -38, -10, 0);

        logText = MkText(canvas.transform, 14);
        Rect(logText.gameObject, 0, 0, 1, 0, 10, 56, -10, 150);

        content = new GameObject("Content");
        content.transform.SetParent(canvas.transform, false);
        Rect(content, 0, 0, 1, 1, 10, 158, -10, -46);
        Image bg = content.AddComponent<Image>();
        bg.color = Navy;

        GameObject bar = new GameObject("Bar");
        bar.transform.SetParent(canvas.transform, false);
        Rect(bar, 0, 0, 1, 0, 0, 0, 0, 48);
        HorizontalLayoutGroup hg = bar.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 5;
        hg.padding = new RectOffset(8, 8, 5, 5);
        MkButton(bar.transform, "Mapa", delegate { screen = "map"; Refresh(); });
        MkButton(bar.transform, "Ilha", delegate { screen = "island"; Refresh(); });
        MkButton(bar.transform, "Navio", delegate { screen = "ship"; Refresh(); });
        MkButton(bar.transform, "Tripulacao", delegate { screen = "crew"; Refresh(); });
        MkButton(bar.transform, "Diario", delegate { screen = "journal"; Refresh(); });
        MkButton(bar.transform, "Salvar", delegate { msg = gm.Save(); Refresh(); });
        MkButton(bar.transform, "Carregar", delegate { msg = gm.Load(); Refresh(); });
        MkButton(bar.transform, "Novo", delegate { gm.NewGame(gm.Seed + 1); screen = "map"; msg = "Nova jornada!"; Refresh(); });
    }

    void Refresh()
    {
        if (gm == null || gm.Data == null) return;
        GameData d = gm.Data;
        topBar.text = string.Format("Dia {0} | Notoriedade {1} | {2}$ | Comida {3} | Agua {4} | Mad {5} | Metal {6} | Med {7} | {8} ({9}/{10}) | Trip {11}/{12}",
            d.world.day, d.notoriety, d.GetResource(ResourceId.Money), d.GetResource(ResourceId.Food), d.GetResource(ResourceId.Water),
            d.GetResource(ResourceId.Wood), d.GetResource(ResourceId.Metal), d.GetResource(ResourceId.Medicine),
            d.ship.defId, d.ship.hull, d.ship.maxHull, GameSession.AliveCrew(d), ShipModules.EffectiveCrewCap(d.ship));
        int from = Math.Max(0, d.journal.Count - 4);
        string log = "";
        for (int i = from; i < d.journal.Count; i++) log += "[d" + d.journal[i].day + "] " + d.journal[i].text + "\n";
        if (gm.EventChoiceText != "") log += "! " + gm.EventChoiceText + "\n";
        if (msg != "") log += "> " + msg + "\n";
        logText.text = log;
        Clear(content.transform);
        if (gm.State == GameState.Sailing) { ShowSailing(d); return; }
        if (gm.State == GameState.Event) { ShowEvent(d); return; }
        if (gm.State == GameState.Combat) { ShowCombat(d); return; }
        if (gm.State == GameState.GameOver) { MkLabel(content.transform, "GAME OVER — toda a tripulacao morreu. Clique em Novo.", 20, Gold); return; }
        if (screen == "island") ShowIsland(d);
        else if (screen == "ship") ShowShip(d);
        else if (screen == "crew") ShowCrew(d);
        else if (screen == "sheet") ShowSheet(d);
        else if (screen == "journal") ShowJournal(d);
        else if (screen == "detail") ShowDetail(d);
        else ShowMap(d);
    }

    // ---------- telas ----------
    void ShowMap(GameData d)
    {
        GameObject card = MkCard(content.transform);
        MkCardText(card, "MAPA DO MUNDO — escolha o destino", 19);
        IslandData from = d.FindIsland(d.currentIslandId);
        for (int i = 0; i < d.world.islands.Count; i++)
        {
            IslandData isl = d.world.islands[i];
            string id = isl.id;
            if (isl.id == d.currentIslandId)
            {
                MkLabel(content.transform, "[VOCE ESTA AQUI] " + isl.displayName + " [" + isl.archetype + "] rep " + d.GetRep(isl.id), 16, Gold);
                continue;
            }
            int days = 1;
            if (from != null)
                days = GameSession.PreviewCalc(from, isl, d.ship, GameSession.BestNavigator(d), GameSession.RollWeather(d.world.seed, d.world.day, i)).days;
            MkButton(content.transform, isl.displayName + " [" + isl.archetype + "] perigo " + isl.danger + " ~" + days + "d rep " + d.GetRep(isl.id), delegate
            {
                gm.SelectDestination(id);
                screen = "detail";
            });
        }
    }

    void ShowDetail(GameData d)
    {
        IslandData to = d.FindIsland(gm.PreviewDest);
        if (to == null || gm.Preview == null)
        {
            MkLabel(content.transform, "Destino invalido: " + gm.PreviewError(gm.PreviewDest), 16, Red);
            MkButton(content.transform, "Voltar ao mapa", delegate { screen = "map"; Refresh(); });
            return;
        }
        GameObject card = MkCard(content.transform);
        MkCardText(card, "DESTINO: " + to.displayName + " [" + to.archetype + "]", 19);
        int best = GameSession.BestNavigator(d);
        string navName = "—";
        for (int i = 0; i < d.crew.Count; i++)
        {
            if (d.crew[i].alive && d.crew[i].nav == best) navName = d.crew[i].displayName + " (" + best + ")";
        }
        int crewN = Math.Max(1, GameSession.AliveCrew(d));
        int needFood = (int)Math.Ceiling(gm.Preview.foodCost * crewN * ShipModules.FoodFactor(d.ship));
        MkCardText(card, "Navegador: " + navName + " | Clima: " + GameSession.WeatherName(gm.PreviewWeather), 15);
        MkCardText(card, string.Format("Tempo: {0}d | Risco: {1}% | Consumo: {2} comida / {3} agua | Eventos ate ~{4}%",
            gm.Preview.days, (int)(gm.Preview.risk * 100), needFood, gm.Preview.waterCost * crewN, (int)(gm.Preview.eventChance * 100)), 15);
        string id = to.id;
        MkButton(content.transform, "INICIAR VIAGEM", delegate { screen = "island"; gm.BeginVoyage(id); });
        MkButton(content.transform, "Voltar ao mapa", delegate { screen = "map"; Refresh(); });
    }

    void ShowSailing(GameData d)
    {
        GameObject card = MkCard(content.transform);
        IslandData to = d.FindIsland(d.sailDestId);
        string name = to != null ? to.displayName : "?";
        MkCardText(card, "Viagem em andamento: " + name + " — Dia " + d.sailDay + "/" + d.sailTotal, 18);
    }

    void ShowEvent(GameData d)
    {
        TravelEventKind k = d.pendingEvent;
        GameObject card = MkCard(content.transform);
        MkCardText(card, TravelEvents.TitleOf(k), 20);
        MkCardText(card, TravelEvents.DescOf(k), 15);
        MkCardText(card, "O que fazer?", 16);
        EventChoice[] opts = TravelEvents.OptionsOf(k);
        for (int i = 0; i < opts.Length; i++)
        {
            int idx = i;
            MkButton(content.transform, opts[i].label + " (" + opts[i].desc + ")", delegate { gm.ChooseEventOption(idx); });
        }
    }

    void ShowIsland(GameData d)
    {
        IslandData isl = d.FindIsland(d.currentIslandId);
        if (isl == null) { MkLabel(content.transform, "Em alto-mar.", 16, Gold); return; }
        GameObject card = MkCard(content.transform);
        MkCardText(card, isl.displayName + " [" + isl.archetype + "] perigo " + isl.danger + " | Rep: " + RepName(d.GetRep(isl.id)) + " (" + d.GetRep(isl.id) + ")", 18);
        MkLabel(content.transform, "-- Comercio --", 16, Gold);
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
        MkLabel(content.transform, "-- Recrutamento --", 16, Gold);
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
            MkButton(content.transform, string.Format("Recrutar {0} [{1} nv{2} nav:{3} lut:{4}] ({5}$)", c.displayName, job, c.level, c.nav, c.fighter, c.hireCost), delegate
            {
                string m; GameSession.Recruit(d, id, out m); msg = m; Refresh();
            });
        }
        if (shown == 0) MkLabel(content.transform, "(sem recrutas aqui)", 15, Gold);
        MkLabel(content.transform, "-- Missoes --", 16, Gold);
        for (int i = 0; i < d.missions.Count; i++)
        {
            MissionData m = d.missions[i];
            if (m.status != MissionStatus.Available && m.status != MissionStatus.Accepted) continue;
            string id = m.id;
            string label = m.title + " [" + m.type + "] +" + m.reward + " risco " + m.danger + " (" + m.status + ")";
            if (m.status == MissionStatus.Available)
                MkButton(content.transform, "Aceitar: " + label, delegate
                {
                    string mm; GameSession.AcceptMission(d, id, out mm); msg = mm; Refresh();
                });
            else
                MkLabel(content.transform, label, 15, Gold);
        }
    }

    void ShowShip(GameData d)
    {
        GameObject card = MkCard(content.transform);
        MkCardText(card, "NAVIO: " + d.ship.defId + "  HP " + d.ship.hull + "/" + d.ship.maxHull, 19);
        MkCardText(card, string.Format("Trip max: {0} | Carga max: {1} | Vel: {2} | Modulos: {3}/{4}",
            ShipModules.EffectiveCrewCap(d.ship), ShipModules.EffectiveCargoCap(d.ship), d.ship.speed,
            ShipModules.UsedSlots(d.ship), d.ship.modules), 15);
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
        MkLabel(content.transform, "-- Modulos --", 16, Gold);
        for (int i = 0; i < ShipModules.Types.Length; i++)
        {
            string t = ShipModules.Types[i];
            string label = string.Format("{0} x{1} — {2}$ + {3}mad + {4}met", ShipModules.NameOf(t), ShipModules.CountOf(d.ship, t),
                ShipModules.MoneyCost(t), ShipModules.WoodCost(t), ShipModules.MetalCost(t));
            MkButton(content.transform, "Instalar: " + label, delegate
            {
                string m; ShipModules.Buy(d, t, out m); msg = m; Refresh();
            });
        }
        MkLabel(content.transform, string.Format("Inventario: comida {0} agua {1} madeira {2} metal {3} medicina {4}",
            d.GetResource(ResourceId.Food), d.GetResource(ResourceId.Water), d.GetResource(ResourceId.Wood),
            d.GetResource(ResourceId.Metal), d.GetResource(ResourceId.Medicine)), 15, Gold);
    }

    void ShowCrew(GameData d)
    {
        GameObject card = MkCard(content.transform);
        MkCardText(card, "TRIPULACAO", 19);
        for (int i = 0; i < d.crew.Count; i++)
        {
            CharacterData c = d.crew[i];
            if (!c.alive) continue;
            string job = c.jobs.Count > 0 ? c.jobs[0] : "?";
            string id = c.id;
            MkButton(content.transform, string.Format("{0} [{1}] nv{2} HP {3}/{4}", c.displayName, job, c.level, c.stats.hp, c.stats.maxHp), delegate
            {
                selChar = id;
                screen = "sheet";
                Refresh();
            });
        }
    }

    void ShowSheet(GameData d)
    {
        CharacterData c = GameSession.FindChar(d, selChar);
        if (c == null) { screen = "crew"; ShowCrew(d); return; }
        string job = c.jobs.Count > 0 ? c.jobs[0] : "?";
        GameObject card = MkCard(content.transform);
        MkCardText(card, c.displayName + " — " + job + "  Nv " + c.level + "  XP " + c.xp + "/" + Progression.XpForLevel(c.level), 18);
        MkCardText(card, string.Format("HP {0}/{1} ATK {2} DEF {3} VEL {4} | Moral {5} Lealdade {6} | Salario {7}/d",
            c.stats.hp, c.stats.maxHp, c.stats.atk, c.stats.def, c.stats.speed, c.morale, c.loyalty, c.wage), 14);
        MkCardText(card, string.Format("Nav {0} Culin {1} Med {2} Carp {3} Luta {4} Tiro {5}",
            c.nav, c.cook, c.medic, c.carpenter, c.fighter, c.shooter), 14);
        MkCardText(card, "Traits: " + string.Join(", ", c.traits.ToArray()) + " | Batalhas " + c.battles + " | Abates " + c.kills, 14);
        MkCardText(card, CharacterGenerator.BioFor(c), 14);
        MkLabel(content.transform, "-- Relacoes --", 16, Gold);
        int n = 0;
        for (int i = 0; i < d.relations.Count; i++)
        {
            RelationshipData r = d.relations[i];
            string other = r.aId == c.id ? r.bId : (r.bId == c.id ? r.aId : null);
            if (other == null) continue;
            CharacterData o = GameSession.FindChar(d, other);
            string oname = o != null ? o.displayName : other;
            MkLabel(content.transform, oname + ": " + RelName(r.affinity) + " (afin " + r.affinity + ", conf " + r.trust + ")", 14, Gold);
            n++;
        }
        if (n == 0) MkLabel(content.transform, "(sem relacoes ainda)", 14, Gold);
        MkButton(content.transform, "Voltar", delegate { screen = "crew"; Refresh(); });
    }

    void ShowCombat(GameData d)
    {
        if (gm.Battle == null) { MkLabel(content.transform, "Sem batalha.", 16, Gold); return; }
        BattleState b = gm.Battle;
        GameObject card = MkCard(content.transform);
        MkCardText(card, "COMBATE — Round " + b.round + "  Perigo " + gm.PendingCombatDanger, 19);
        MkLabel(content.transform, "-- Inimigos (clique p/ mirar) --", 15, Gold);
        for (int i = 0; i < b.enemies.Count; i++)
        {
            EnemyData e = b.enemies[i];
            if (e.hp <= 0) continue;
            int idx = i;
            string mark = idx == selTarget ? ">> " : "";
            MkButton(content.transform, mark + e.name + " HP " + e.hp + "/" + e.maxHp, delegate { selTarget = idx; Refresh(); });
        }
        CharacterData cur = b.CurrentCrew();
        if (cur != null)
        {
            GameObject c2 = MkCard(content.transform);
            MkCardText(c2, "Vez de " + cur.displayName + " (HP " + cur.stats.hp + "/" + cur.stats.maxHp + ")", 16);
        }
        if (gm.LastBattleText != "") MkLabel(content.transform, gm.LastBattleText, 14, Gold);
        MkButtonC(content.transform, "ATACAR", delegate { gm.BattleAct(BattleAction.Attack, selTarget); }, Teal);
        MkButtonC(content.transform, "GOLPE PESADO (-2 HP)", delegate { gm.BattleAct(BattleAction.Heavy, selTarget); }, Teal);
        MkButtonC(content.transform, "DEFENDER", delegate { gm.BattleAct(BattleAction.Defend, 0); }, Teal);
        MkButtonC(content.transform, "ITEM (+15 HP, med: " + d.GetResource(ResourceId.Medicine) + ")", delegate { gm.BattleAct(BattleAction.Item, 0); }, Teal);
        MkButtonC(content.transform, "FUGIR (45%)", delegate { gm.BattleAct(BattleAction.Flee, 0); }, Red);
        MkLabel(content.transform, "-- Ultimos lances --", 14, Gold);
        int from = Math.Max(0, b.log.Count - 4);
        for (int i = from; i < b.log.Count; i++) MkLabel(content.transform, b.log[i], 13, Gold);
    }

    void ShowJournal(GameData d)
    {
        GameObject tabs = new GameObject("Tabs");
        tabs.transform.SetParent(content.transform, false);
        HorizontalLayoutGroup hg = tabs.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 4;
        string[] names = new string[] { "Todos", "Viagens", "Missoes", "Combate", "Recrutamento", "Mortes" };
        for (int i = 0; i < names.Length; i++)
        {
            string t = names[i];
            MkButton(tabs.transform, t, delegate { journalTab = t; Refresh(); });
        }
        if (journalTab == "Mortes")
        {
            GameObject card = MkCard(content.transform);
            MkCardText(card, "MEMORIAL", 19);
            if (d.memorial.Count == 0) MkCardText(card, "(nenhum morto — por enquanto)", 14);
            for (int i = 0; i < d.memorial.Count; i++)
                MkCardText(card, d.memorial[i].name + " — d" + d.memorial[i].dayDied + " (" + d.memorial[i].cause + ")", 14);
            return;
        }
        GameObject card2 = MkCard(content.transform);
        MkCardText(card2, "DIARIO DA TRIPULACAO — " + journalTab, 18);
        int shown = 0;
        for (int i = d.journal.Count - 1; i >= 0 && shown < 14; i--)
        {
            if (!JournalMatch(d.journal[i].text)) continue;
            MkCardText(card2, "[d" + d.journal[i].day + "] " + d.journal[i].text, 13);
            shown++;
        }
        if (shown == 0) MkCardText(card2, "(vazio)", 14);
    }

    bool JournalMatch(string t)
    {
        string s = t.ToLower();
        if (journalTab == "Todos") return true;
        if (journalTab == "Viagens") return s.Contains("chegada") || s.Contains("partimos") || s.Contains("viagem") || s.Contains("tempestade") || s.Contains("navio");
        if (journalTab == "Missoes") return s.Contains("missao");
        if (journalTab == "Combate") return s.Contains("batalha") || s.Contains("combate") || s.Contains("pirata") || s.Contains("criatura") || s.Contains("vitoria") || s.Contains("derrota");
        if (journalTab == "Recrutamento") return s.Contains("juntou-se") || s.Contains("recrut");
        return true;
    }

    string RepName(int rep)
    {
        if (rep >= 60) return "Admirado";
        if (rep >= 20) return "Amigavel";
        if (rep <= -60) return "Hostil";
        if (rep <= -20) return "Desconfiado";
        return "Neutro";
    }

    string RelName(int aff)
    {
        if (aff >= 40) return "Amizade";
        if (aff >= 10) return "Confianca";
        if (aff >= -10) return "Neutro";
        return "Rivalidade";
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

    void MkLabel(Transform parent, string s, int size, Color col)
    {
        Text tx = MkText(parent, size);
        tx.text = s;
        tx.color = col;
    }

    GameObject MkCard(Transform parent)
    {
        GameObject go = new GameObject("Card");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = Parch;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 4);
        return go;
    }

    void MkCardText(GameObject card, string s, int size)
    {
        Text tx = MkText(card.transform, size);
        tx.text = "  " + s;
        tx.color = Ink;
    }

    void MkButton(Transform parent, string label, Action onClick)
    {
        MkButtonC(parent, label, onClick, Teal);
    }

    void MkButtonC(Transform parent, string label, Action onClick, Color bg)
    {
        GameObject go = new GameObject("Btn");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = bg;
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
