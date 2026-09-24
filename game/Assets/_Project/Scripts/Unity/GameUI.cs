// CrewJournal — UI por codigo v2: scroll + layout groups + mapa posicionado.
// Tema refs: navy + pergaminho + teal + dourado. Fontes grandes, botoes min 52px.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrewJournal.Logic;

public class GameUI : MonoBehaviour
{
    static readonly Color Navy = new Color(0.04f, 0.08f, 0.15f, 1f);
    static readonly Color Panel = new Color(0.07f, 0.14f, 0.23f, 1f);
    static readonly Color Sea = new Color(0.10f, 0.32f, 0.46f, 1f);
    static readonly Color Parch = new Color(0.90f, 0.83f, 0.64f);
    static readonly Color Ink = new Color(0.23f, 0.16f, 0.09f);
    static readonly Color Teal = new Color(0.11f, 0.36f, 0.43f);
    static readonly Color TealDark = new Color(0.08f, 0.25f, 0.31f);
    static readonly Color Gold = new Color(0.95f, 0.77f, 0.25f);
    static readonly Color Red = new Color(0.62f, 0.20f, 0.20f);

    GameManager gm;
    Font font;
    Font fontPixel;
    Text topDay, topMoney, topFood, topWater, topWood, topMed, topTrip;
    Text logText;
    RectTransform content;
    CanvasGroup contentFade;
    int animToken;
    string screen = "map";
    string msg = "";
    string selChar = "";
    int selTarget;
    string journalTab = "Todos";
    int prevMoney = -1;

    void Awake()
    {
        gm = GameManager.Instance;
        if (gm == null) gm = FindAnyObjectByType<GameManager>();
        font = Resources.Load<Font>("Fonts/KenneyMini");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        fontPixel = Resources.Load<Font>("Fonts/KenneyPixel");
        if (fontPixel == null) fontPixel = font;
        PlaceholderArt.Init();
        Juice.Ensure();
        Build();
        gm.OnChanged += Refresh;
        gm.OnEnemyHit += delegate { Juice.Shake(content); Sfx.Hit(); };
        Music.PlayMap();
        Refresh();
    }

    // ================= estrutura =================
    void Build()
    {
        GameObject canvas = new GameObject("Canvas");
        UnityEngine.Canvas c = canvas.AddComponent<UnityEngine.Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler s = canvas.AddComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1280, 720);
        canvas.AddComponent<GraphicRaycaster>();

        GameObject top = new GameObject("TopBar");
        top.transform.SetParent(canvas.transform, false);
        Rect(top, 0, 1, 1, 1, 6, -44, -6, -4);
        HorizontalLayoutGroup thg = top.AddComponent<HorizontalLayoutGroup>();
        thg.spacing = 6;
        thg.padding = new RectOffset(8, 8, 4, 4);
        thg.childControlWidth = false;
        thg.childForceExpandWidth = false;
        topDay = TopChip(top.transform, null);
        topMoney = TopChip(top.transform, "icon_coin");
        topFood = TopChip(top.transform, "icon_food");
        topWater = TopChip(top.transform, "icon_water");
        topWood = TopChip(top.transform, "icon_wood");
        topMed = TopChip(top.transform, "icon_medicine");
        topTrip = TopChip(top.transform, "icon_ship");

        logText = MkText(canvas.transform, 15);
        Rect(logText.gameObject, 0, 0, 1, 0, 10, 66, -10, 168);

        // viewport com scroll
        GameObject vp = new GameObject("Viewport");
        vp.transform.SetParent(canvas.transform, false);
        Rect(vp, 0, 0, 1, 1, 8, 176, -8, -50);
        Image vbg = vp.AddComponent<Image>();
        vbg.color = Navy;
        vp.AddComponent<RectMask2D>();

        GameObject co = new GameObject("Content");
        co.transform.SetParent(vp.transform, false);
        content = co.AddComponent<RectTransform>();
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0, 0);
        VerticalLayoutGroup vg = co.AddComponent<VerticalLayoutGroup>();
        vg.spacing = 10;
        vg.padding = new RectOffset(12, 12, 12, 12);
        vg.childControlWidth = true;
        vg.childForceExpandWidth = true;
        vg.childForceExpandHeight = false;
        ContentSizeFitter cf = co.AddComponent<ContentSizeFitter>();
        cf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentFade = co.AddComponent<CanvasGroup>();

        ScrollRect scroll = canvas.AddComponent<ScrollRect>();
        scroll.content = content;
        scroll.viewport = vp.GetComponent<RectTransform>();
        scroll.horizontal = false;
        scroll.vertical = true;

        GameObject bar = new GameObject("Bar");
        bar.transform.SetParent(canvas.transform, false);
        Rect(bar, 0, 0, 1, 0, 0, 0, 0, 58);
        HorizontalLayoutGroup hg = bar.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 6;
        hg.padding = new RectOffset(8, 8, 6, 6);
        MkBarButton(bar.transform, "Mapa", delegate { screen = "map"; Refresh(); });
        MkBarButton(bar.transform, "Ilha", delegate { screen = "island"; Refresh(); });
        MkBarButton(bar.transform, "Navio", delegate { screen = "ship"; Refresh(); });
        MkBarButton(bar.transform, "Equipe", delegate { screen = "crew"; Refresh(); });
        MkBarButton(bar.transform, "Diario", delegate { screen = "journal"; Refresh(); });
        MkBarButton(bar.transform, "Menu", delegate { screen = "menu"; Refresh(); });
    }

    void Refresh()
    {
        if (gm == null || gm.Data == null) return;
        GameData d = gm.Data;
        int money = d.GetResource(ResourceId.Money);
        if (prevMoney >= 0 && money > prevMoney) Sfx.Coin();
        prevMoney = money;
        topDay.text = "Dia " + d.world.day;
        topMoney.text = money + "$";
        topFood.text = "Comida " + d.GetResource(ResourceId.Food);
        topWater.text = "Agua " + d.GetResource(ResourceId.Water);
        topWood.text = "Mad " + d.GetResource(ResourceId.Wood);
        topMed.text = "Med " + d.GetResource(ResourceId.Medicine);
        topTrip.text = "Trip " + GameSession.AliveCrew(d) + "/" + ShipModules.EffectiveCrewCap(d.ship);
        int from = Math.Max(0, d.journal.Count - 3);
        string log = "";
        for (int i = from; i < d.journal.Count; i++) log += "[d" + d.journal[i].day + "] " + d.journal[i].text + "\n";
        if (gm.EventChoiceText != "") log += "! " + gm.EventChoiceText + "\n";
        if (msg != "") log += "> " + msg + "\n";
        logText.text = log;
        Clear(content);
        animToken++;
        Juice.Fade(contentFade);
        if (gm.State == GameState.Sailing) { ShowSailing(d); return; }
        if (gm.State == GameState.Event) { ShowEvent(d); return; }
        if (gm.State == GameState.Combat) { ShowCombat(d); return; }
        if (gm.State == GameState.GameOver) { MkHeader("GAME OVER — toda a tripulacao morreu. Va em Menu > Novo."); return; }
        if (screen == "island") ShowIsland(d);
        else if (screen == "ship") ShowShip(d);
        else if (screen == "crew") ShowCrew(d);
        else if (screen == "sheet") ShowSheet(d);
        else if (screen == "journal") ShowJournal(d);
        else if (screen == "detail") ShowDetail(d);
        else if (screen == "menu") ShowMenu(d);
        else ShowMap(d);
    }

    // ================= telas =================
    void ShowMap(GameData d)
    {
        GameObject card = MkCard();
        MkCardText(card, "MAPA DO MUNDO — toque numa ilha para ver a viagem", 20);
        ClickHint(card.transform);
        GameObject sea = new GameObject("Sea");
        sea.transform.SetParent(content, false);
        Image simg = sea.AddComponent<Image>();
        simg.color = Sea;
        Sprite sea0 = UIStyle.Get("sea_0");
        if (sea0 != null)
        {
            simg.sprite = sea0;
            simg.color = Color.white;
            simg.type = Image.Type.Tiled;
            int t = animToken;
            Juice.Frames(simg, new string[] { "sea_0", "sea_1", "sea_2", "sea_3" }, 0.4f, delegate { return animToken == t; });
        }
        LayoutElement sle = sea.AddComponent<LayoutElement>();
        sle.minHeight = 430;
        sle.preferredHeight = 430;
        RectTransform seaRt = sea.GetComponent<RectTransform>();
        UIStyle.Compass(seaRt);
        for (int i = 0; i < d.world.islands.Count; i++)
        {
            IslandData isl = d.world.islands[i];
            float ax, ay;
            Iso.Pin(isl.x, isl.y, out ax, out ay);
            string id = isl.id;
            bool here = isl.id == d.currentIslandId;
            Texture2D itex = IslandRenderer.Render(d.world.seed, isl.id, isl.archetype);
            MapIsland(seaRt, ax, ay, itex, isl.displayName, isl.danger, here, delegate
            {
                if (!here)
                {
                    gm.SelectDestination(id);
                    screen = "detail";
                }
            });
        }
    }

    void ShowDetail(GameData d)
    {
        IslandData to = d.FindIsland(gm.PreviewDest);
        if (to == null || gm.Preview == null)
        {
            MkHeader("Destino invalido: " + gm.PreviewError(gm.PreviewDest));
            MkButton("Voltar ao mapa", 56, delegate { screen = "map"; Refresh(); });
            return;
        }
        GameObject card = MkCard();
        MkCardText(card, "DESTINO: " + to.displayName + "  [" + to.archetype + "]", 22);
        int best = GameSession.BestNavigator(d);
        string navName = "nenhum";
        for (int i = 0; i < d.crew.Count; i++)
        {
            if (d.crew[i].alive && d.crew[i].nav == best) navName = d.crew[i].displayName + " (" + best + ")";
        }
        int crewN = Math.Max(1, GameSession.AliveCrew(d));
        int needFood = (int)Math.Ceiling(gm.Preview.foodCost * crewN * ShipModules.FoodFactor(d.ship));
        int needWater = gm.Preview.waterCost * crewN;
        MkCardText(card, "Navegador: " + navName, 17);
        MkCardText(card, "Clima: " + GameSession.WeatherName(gm.PreviewWeather), 17);
        MkCardText(card, "Ordens do capitão (bônus — o mar continua imprevisível):", 16);
        GameObject stanceRow = new GameObject("Stance");
        stanceRow.transform.SetParent(content, false);
        HorizontalLayoutGroup shg = stanceRow.AddComponent<HorizontalLayoutGroup>();
        shg.spacing = 4;
        shg.childControlWidth = true;
        shg.childForceExpandWidth = true;
        LayoutElement stle = stanceRow.AddComponent<LayoutElement>();
        stle.minHeight = 48;
        stle.preferredHeight = 48;
        string[] snames = new string[] { "Cautela", "Normal", "Marcha" };
        string[] sdesc = new string[] { "+1d, -risco", "equilíbrio", "-1d, +risco" };
        for (int si = 0; si < 3; si++)
        {
            int sidx = si;
            string sid = to.id;
            MkSmallButton(stanceRow.transform, snames[si] + " (" + sdesc[si] + ")", d.sailStance == si, delegate
            {
                d.sailStance = sidx;
                gm.SelectDestination(sid);
                Refresh();
            });
        }
        MkCardText(card, string.Format("Tempo: {0} dias   Risco: {1}%   Eventos: ate ~{2}%", gm.Preview.days, (int)(gm.Preview.risk * 100), (int)(gm.Preview.eventChance * 100)), 17);
        MkCardText(card, string.Format("Consumo: {0} comida / {1} agua   (voce tem {2} / {3})", needFood, needWater, d.GetResource(ResourceId.Food), d.GetResource(ResourceId.Water)), 17);
        MkCardText(card, "Em jogo: restam " + (d.GetResource(ResourceId.Food) - needFood) + " comida / " + (d.GetResource(ResourceId.Water) - needWater) + " agua. Frágeis: " + FragileCrew(d), 16);
        PortraitImage(content, 150, 150, IslandRenderer.Render(d.world.seed, to.id, to.archetype));
        string id = to.id;
        MkBigButton("INICIAR VIAGEM — " + gm.Preview.days + " dias", delegate { screen = "island"; gm.BeginVoyage(id); });
        MkButton("Voltar ao mapa", 52, delegate { screen = "map"; Refresh(); });
    }

    void ShowSailing(GameData d)
    {
        GameObject card = MkCard();
        IslandData to = d.FindIsland(d.sailDestId);
        string name = to != null ? to.displayName : "?";
        MkCardText(card, "NAVEGANDO: " + name, 22);
        MkCardText(card, "Dia " + Math.Min(d.sailDay + 1, d.sailTotal) + " de " + d.sailTotal, 18);
        GameObject barBg = new GameObject("ProgBg");
        barBg.transform.SetParent(content, false);
        Image bg = barBg.AddComponent<Image>();
        bg.color = Panel;
        LayoutElement le = barBg.AddComponent<LayoutElement>();
        le.minHeight = 30;
        le.preferredHeight = 30;
        GameObject fill = new GameObject("ProgFill");
        fill.transform.SetParent(barBg.transform, false);
        Image fi = fill.AddComponent<Image>();
        fi.color = Gold;
        fi.type = Image.Type.Filled;
        fi.fillMethod = Image.FillMethod.Horizontal;
        UIStyle.SkinProgress(bg, fi);
        float frac = d.sailTotal > 0 ? (float)d.sailDay / d.sailTotal : 0f;
        fi.fillAmount = 0f;
        Juice.FillTo(fi, frac);
        GameObject sail = new GameObject("SailShip");
        sail.transform.SetParent(content, false);
        Image sailImg = sail.AddComponent<Image>();
        sailImg.color = Sea;
        LayoutElement sle2 = sail.AddComponent<LayoutElement>();
        sle2.minHeight = 90;
        sle2.preferredHeight = 90;
        Sprite sh0 = UIStyle.Get("sailship_0");
        if (sh0 != null)
        {
            sailImg.sprite = sh0;
            sailImg.color = Color.white;
            int t = animToken;
            Juice.Frames(sailImg, new string[] { "sailship_0", "sailship_1", "sailship_2" }, 0.3f, delegate { return animToken == t; });
        }
        RectTransform frt = fill.GetComponent<RectTransform>();
        frt.anchorMin = new Vector2(0, 0);
        frt.anchorMax = new Vector2(1, 1);
        frt.offsetMin = new Vector2(4, 4);
        frt.offsetMax = new Vector2(-4, -4);
        MkLabel("A tripulacao segue as ordens do navegador...", 16, Color.white);
        if (gm.sailTick > 0.3f)
        {
            MkButton("APRESSAR >>>", 52, delegate { gm.FastForward(); Refresh(); });
        }
        else
        {
            MkLabel("(a todo pano!)", 15, Gold);
        }
    }

    void ShowEvent(GameData d)
    {
        TravelEventKind k = d.pendingEvent;
        GameObject card = MkCard();
        MkCardText(card, TravelEvents.TitleOf(k), 24);
        MkCardText(card, TravelEvents.DescOf(k), 17);
        MkCardText(card, "O que fazer?", 18);
        ClickHint(card.transform);
        EventChoice[] opts = TravelEvents.OptionsOf(k);
        for (int i = 0; i < opts.Length; i++)
        {
            int idx = i;
            MkBigButton(opts[i].label + "\n" + opts[i].desc, delegate { gm.ChooseEventOption(idx); }, true);
        }
    }

    void ShowIsland(GameData d)
    {
        IslandData isl = d.FindIsland(d.currentIslandId);
        if (isl == null) { MkHeader("Em alto-mar."); return; }
        GameObject card = MkCard();
        MkCardText(card, isl.displayName + "  [" + isl.archetype + "]", 22);
        MkCardText(card, "Perigo " + isl.danger + "   Reputacao: " + RepName(d.GetRep(isl.id)) + " (" + d.GetRep(isl.id) + ")", 17);
        PortraitImage(content, 150, 150, IslandRenderer.Render(d.world.seed, isl.id, isl.archetype));
        MkHeader("Comercio");
        ResourceId[] ids = new ResourceId[] { ResourceId.Food, ResourceId.Water, ResourceId.Wood, ResourceId.Metal, ResourceId.Medicine };
        for (int i = 0; i < ids.Length; i++)
        {
            ResourceId r = ids[i];
            int buy = EconomySystem.PriceFor(r, WorldGenerator.IslandMult(isl, r), d.GetRep(isl.id));
            int sell = EconomySystem.SellPriceFor(r, WorldGenerator.IslandMult(isl, r), d.GetRep(isl.id));
            TradeRow(r, r + "  (tem " + d.GetResource(r) + ")", "Comprar " + buy + "$", "Vender " + sell + "$", delegate
            {
                string m; GameSession.Buy(d, isl.id, r, 1, out m); msg = m; Refresh();
            }, delegate
            {
                string m; GameSession.Sell(d, isl.id, r, 1, out m); msg = m; Refresh();
            });
        }
        MkHeader("Recrutamento");
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
            Texture2D tex = PortraitRenderer.Render(d.world.seed, c.id, job, c.traits, c.scar, c.outfitMod, c.grave);
            PortraitButton(string.Format("{0}  [{1} nv{2}]  nav:{3} lut:{4}  —  {5}$", c.displayName, job, c.level, c.nav, c.fighter, c.hireCost), tex, delegate
            {
                string m; if (GameSession.Recruit(d, id, out m)) Sfx.Recruit(); msg = m; Refresh();
            });
        }
        if (shown == 0) MkLabel("(sem recrutas aqui)", 16, Gold);
        MkHeader("Missoes");
        for (int i = 0; i < d.missions.Count; i++)
        {
            MissionData m = d.missions[i];
            if (m.status != MissionStatus.Available && m.status != MissionStatus.Accepted) continue;
            string id = m.id;
            string label = m.title + "  [" + m.type + "]  +" + m.reward + "$  risco " + m.danger + "  (" + m.status + ")";
            if (m.status == MissionStatus.Available)
                MkButton("Aceitar: " + label, 56, delegate
                {
                    string mm; GameSession.AcceptMission(d, id, out mm); msg = mm; Refresh();
                });
            else
                MkLabel(label, 16, Gold);
        }
    }

    void ShowShip(GameData d)
    {
        GameObject card = MkCard();
        MkCardText(card, "NAVIO: " + d.ship.defId + "   HP " + d.ship.hull + "/" + d.ship.maxHull, 22);
        MkCardText(card, string.Format("Trip max {0}   Carga max {1}   Vel {2}   Modulos {3}/{4}",
            ShipModules.EffectiveCrewCap(d.ship), ShipModules.EffectiveCargoCap(d.ship), d.ship.speed,
            ShipModules.UsedSlots(d.ship), d.ship.modules), 17);
        PortraitImage(content, 240, 150, ShipRenderer.Render(d.ship.defId, d.world.seed, ShipModules.CountOf(d.ship, "special")));
        MkButton("Reparar casco", 56, delegate
        {
            string m; GameSession.Repair(d, out m); msg = m; Refresh();
        });
        if (d.ship.defId == "barrel")
            MkButton("Comprar Pequeno Barco (500$)", 56, delegate
            {
                string m; GameSession.BuyShip(d, "boat", out m); msg = m; Refresh();
            });
        else if (d.ship.defId == "boat")
            MkButton("Comprar Navio Medio (2500$)", 56, delegate
            {
                string m; GameSession.BuyShip(d, "medium", out m); msg = m; Refresh();
            });
        MkHeader("Modulos");
        for (int i = 0; i < ShipModules.Types.Length; i++)
        {
            string t = ShipModules.Types[i];
            MkButton(string.Format("{0} x{1}  —  {2}$ + {3}mad + {4}met", ShipModules.NameOf(t), ShipModules.CountOf(d.ship, t),
                ShipModules.MoneyCost(t), ShipModules.WoodCost(t), ShipModules.MetalCost(t)), 56, delegate
            {
                string m; ShipModules.Buy(d, t, out m); msg = m; Refresh();
            });
        }
        MkHeader("Inventario");
        MkLabel(string.Format("Comida {0}   Agua {1}   Madeira {2}   Metal {3}   Medicina {4}   Dinheiro {5}$",
            d.GetResource(ResourceId.Food), d.GetResource(ResourceId.Water), d.GetResource(ResourceId.Wood),
            d.GetResource(ResourceId.Metal), d.GetResource(ResourceId.Medicine), d.GetResource(ResourceId.Money)), 17, Gold);
    }

    void ShowCrew(GameData d)
    {
        GameObject card = MkCard();
        MkCardText(card, "TRIPULACAO — toque para ver a ficha", 20);
        for (int i = 0; i < d.crew.Count; i++)
        {
            CharacterData c = d.crew[i];
            if (!c.alive) continue;
            string job = c.jobs.Count > 0 ? c.jobs[0] : "?";
            string id = c.id;
            Texture2D tex = PortraitRenderer.Render(d.world.seed, c.id, job, c.traits, c.scar, c.outfitMod, c.grave);
            PortraitButton(string.Format("{0}{1}  [{2}]  nv{3}  HP {4}/{5}", c.grave ? "[GRAVE] " : "", c.displayName, job, c.level, c.stats.hp, c.stats.maxHp), tex, delegate
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
        GameObject card = MkCard();
        MkCardText(card, c.displayName + " — " + job, 22);
        PortraitImage(content, 140, 168, PortraitRenderer.Render(d.world.seed, c.id, job, c.traits, c.scar, c.outfitMod, c.grave));
        MkCardText(card, string.Format("Nivel {0}   XP {1}/{2}   HP {3}/{4}", c.level, c.xp, Progression.XpForLevel(c.level), c.stats.hp, c.stats.maxHp), 17);
        MkCardText(card, string.Format("ATK {0}  DEF {1}  VEL {2}   Moral {3}  Lealdade {4}  Salario {5}$/d",
            c.stats.atk, c.stats.def, c.stats.speed, c.morale, c.loyalty, c.wage), 17);
        MkCardText(card, string.Format("Nav {0}  Culin {1}  Med {2}  Carp {3}  Luta {4}  Tiro {5}",
            c.nav, c.cook, c.medic, c.carpenter, c.fighter, c.shooter), 17);
        MkCardText(card, "Traits: " + string.Join(", ", c.traits.ToArray()) + "   Batalhas " + c.battles + "  Abates " + c.kills, 16);
        MkCardText(card, CharacterGenerator.BioFor(c), 16);
        if (c.grave)
        {
            MkCardText(card, "FERIDO GRAVE — ATK pela metade, próximo golpe letal mata. Trate com 1 medicina.", 16);
            MkCardText(card, "Usar 1 medicina aqui ou guardar para a viagem? Eventos e combate também consomem.", 15);
            string cid = c.id;
            MkButton("Tratar (1 medicina, tem " + d.GetResource(ResourceId.Medicine) + ")", 56, delegate
            {
                string m; if (GameSession.TreatWound(d, cid, out m)) Sfx.Heal(); msg = m; Refresh();
            });
        }
        MkHeader("Relacoes");
        int n = 0;
        for (int i = 0; i < d.relations.Count; i++)
        {
            RelationshipData r = d.relations[i];
            string other = r.aId == c.id ? r.bId : (r.bId == c.id ? r.aId : null);
            if (other == null) continue;
            CharacterData o = GameSession.FindChar(d, other);
            string oname = o != null ? o.displayName : other;
            MkLabel(oname + ": " + RelName(r.affinity) + " (afin " + r.affinity + ", conf " + r.trust + ")", 16, Gold);
            n++;
        }
        if (n == 0) MkLabel("(sem relacoes ainda)", 16, Gold);
        MkButton("Voltar", 52, delegate { screen = "crew"; Refresh(); });
    }

    void ShowCombat(GameData d)
    {
        if (gm.Battle == null) { MkHeader("Sem batalha."); return; }
        BattleState b = gm.Battle;
        GameObject card = MkCard();
        MkCardText(card, "COMBATE — Round " + b.round, 22);
        MkLabel("Inimigos (toque para mirar):", 17, Gold);
        for (int i = 0; i < b.enemies.Count; i++)
        {
            EnemyData e = b.enemies[i];
            if (e.hp <= 0) continue;
            int idx = i;
            string mark = idx == selTarget ? ">> " : "";
            Texture2D etex = PortraitRenderer.RenderEnemy(d.world.seed, EnemyIndex(e.name), i);
            PortraitButton(mark + e.name + "   HP " + e.hp + "/" + e.maxHp, etex, delegate { selTarget = idx; Refresh(); });
        }
        CharacterData cur = b.CurrentCrew();
        if (cur != null)
        {
            string cjob = cur.jobs.Count > 0 ? cur.jobs[0] : "?";
            PortraitImage(content, 80, 96, PortraitRenderer.Render(d.world.seed, cur.id, cjob, cur.traits, cur.scar, cur.outfitMod, cur.grave));
            GameObject c2 = MkCard();
            MkCardText(c2, "Vez de " + cur.displayName + "  (HP " + cur.stats.hp + "/" + cur.stats.maxHp + ")", 19);
        }
        if (gm.LastBattleText != "") MkLabel(gm.LastBattleText, 16, Gold);
        ActionRow(
            "ATACAR", delegate { gm.BattleAct(BattleAction.Attack, selTarget); },
            "PESADO", delegate { gm.BattleAct(BattleAction.Heavy, selTarget); },
            "DEFESA", delegate { gm.BattleAct(BattleAction.Defend, 0); },
            "ITEM", delegate { gm.BattleAct(BattleAction.Item, 0); },
            "FUGIR", delegate { gm.BattleAct(BattleAction.Flee, 0); });
        MkLabel("Ultimos lances:", 15, Gold);
        int from = Math.Max(0, b.log.Count - 4);
        for (int i = from; i < b.log.Count; i++) MkLabel(b.log[i], 14, Gold);
    }

    void ShowJournal(GameData d)
    {
        GameObject tabs = new GameObject("Tabs");
        tabs.transform.SetParent(content, false);
        HorizontalLayoutGroup hg = tabs.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 4;
        hg.childControlWidth = true;
        hg.childForceExpandWidth = true;
        LayoutElement tle = tabs.AddComponent<LayoutElement>();
        tle.minHeight = 48;
        tle.preferredHeight = 48;
        string[] names = new string[] { "Todos", "Viagens", "Missoes", "Combate", "Equipe", "Mortes" };
        for (int i = 0; i < names.Length; i++)
        {
            string t = names[i];
            MkSmallButton(tabs.transform, t, t == journalTab, delegate { journalTab = t; Refresh(); });
        }
        if (journalTab == "Mortes")
        {
            GameObject card = MkCard();
            MkCardText(card, "MEMORIAL", 22);
            if (d.memorial.Count == 0) MkCardText(card, "(nenhum morto — por enquanto)", 16);
            for (int i = 0; i < d.memorial.Count; i++)
                MkCardText(card, d.memorial[i].name + " — dia " + d.memorial[i].dayDied + " (" + d.memorial[i].cause + ")", 16);
            return;
        }
        GameObject card2 = MkCard();
        MkCardText(card2, "DIARIO DA TRIPULACAO", 22);
        int shown = 0;
        for (int i = d.journal.Count - 1; i >= 0 && shown < 16; i--)
        {
            if (!JournalMatch(d.journal[i].text)) continue;
            MkCardText(card2, "[d" + d.journal[i].day + "] " + d.journal[i].text, 15);
            shown++;
        }
        if (shown == 0) MkCardText(card2, "(vazio)", 16);
    }

    void ShowMenu(GameData d)
    {
        GameObject card = MkCard();
        MkCardText(card, "SAIL & SURVIVE — menu", 22);
        MkCardText(card, "One ocean, a thousand stories.", 16);
        MkBigButton("SALVAR", delegate { msg = gm.Save(); Refresh(); });
        MkBigButton("CARREGAR", delegate { msg = gm.Load(); Refresh(); });
        MkBigButton("NOVA JORNADA", delegate { gm.NewGame(gm.Seed + 1); screen = "map"; msg = "Nova jornada!"; Refresh(); }, true);
        MkBigButton("SOM: " + (Sfx.muted ? "OFF" : "ON"), delegate { Sfx.muted = !Sfx.muted; Music.ApplyMute(); Refresh(); });
        MkButton("Voltar ao mapa", 52, delegate { screen = "map"; Refresh(); });
    }

    bool JournalMatch(string t)
    {
        string s = t.ToLower();
        if (journalTab == "Todos") return true;
        if (journalTab == "Viagens") return s.Contains("chegada") || s.Contains("partimos") || s.Contains("viagem") || s.Contains("tempestade") || s.Contains("navio");
        if (journalTab == "Missoes") return s.Contains("missao");
        if (journalTab == "Combate") return s.Contains("batalha") || s.Contains("combate") || s.Contains("pirata") || s.Contains("criatura") || s.Contains("vitoria") || s.Contains("derrota");
        if (journalTab == "Equipe") return s.Contains("juntou-se") || s.Contains("recrut") || s.Contains("morreu") || s.Contains("nivel");
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

    string FragileCrew(GameData d)
    {
        CharacterData f1 = null, f2 = null;
        for (int i = 0; i < d.crew.Count; i++)
        {
            CharacterData c = d.crew[i];
            if (!c.alive) continue;
            if (f1 == null || c.stats.hp < f1.stats.hp) { f2 = f1; f1 = c; }
            else if (f2 == null || c.stats.hp < f2.stats.hp) { f2 = c; }
        }
        string s = f1 != null ? f1.displayName + " " + f1.stats.hp + "/" + f1.stats.maxHp : "—";
        if (f2 != null) s += ", " + f2.displayName + " " + f2.stats.hp + "/" + f2.stats.maxHp;
        return s;
    }

    string RelName(int aff)
    {
        if (aff >= 40) return "Amizade";
        if (aff >= 10) return "Confianca";
        if (aff >= -10) return "Neutro";
        return "Rivalidade";
    }

    // ================= widgets =================
    void Clear(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--) Destroy(t.GetChild(i).gameObject);
    }

    Text MkText(Transform parent, int size)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text tx = go.AddComponent<Text>();
        tx.font = size >= 19 ? fontPixel : font;
        tx.fontSize = size;
        tx.color = Color.white;
        return tx;
    }

    void Fit(Text tx)
    {
        ContentSizeFitter f = tx.gameObject.AddComponent<ContentSizeFitter>();
        f.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    void MkHeader(string s)
    {
        Text tx = MkText(content, 21);
        tx.text = s;
        tx.color = Gold;
        Fit(tx);
    }

    void MkLabel(string s, int size, Color col)
    {
        Text tx = MkText(content, size);
        tx.text = s;
        tx.color = col;
        Fit(tx);
    }

    GameObject MkCard()
    {
        GameObject go = new GameObject("Card");
        go.transform.SetParent(content, false);
        Image img = go.AddComponent<Image>();
        img.color = Parch;
        UIStyle.SkinCard(go);
        VerticalLayoutGroup vg = go.AddComponent<VerticalLayoutGroup>();
        vg.spacing = 6;
        vg.padding = new RectOffset(14, 14, 12, 12);
        vg.childControlWidth = true;
        vg.childForceExpandWidth = true;
        vg.childForceExpandHeight = false;
        ContentSizeFitter cf = go.AddComponent<ContentSizeFitter>();
        cf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return go;
    }

    void MkCardText(GameObject card, string s, int size)
    {
        Text tx = MkText(card.transform, size);
        tx.text = s;
        tx.color = Ink;
        Fit(tx);
    }

    GameObject MkBtnBase(Transform parent, float minH, Color bg)
    {
        GameObject go = new GameObject("Btn");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = bg;
        UIStyle.SkinButton(go, bg);
        Button b = go.AddComponent<Button>();
        b.onClick.AddListener(delegate { Juice.Punch(go); Sfx.Click(); });
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = minH;
        le.preferredHeight = minH;
        return go;
    }

    void MkBtnText(GameObject go, string label, int size)
    {
        Text tx = MkText(go.transform, size);
        tx.text = label;
        tx.alignment = TextAnchor.MiddleCenter;
        RectTransform rt = tx.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(8, 4);
        rt.offsetMax = new Vector2(-8, -4);
    }

    void MkButton(string label, float minH, Action onClick)
    {
        GameObject go = MkBtnBase(content, minH, Teal);
        go.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onClick(); });
        MkBtnText(go, label, 19);
    }

    void MkBigButton(string label, Action onClick, bool shake)
    {
        GameObject go = MkBtnBase(content, 72, Teal);
        if (shake) go.GetComponent<Button>().onClick.AddListener(delegate { Juice.Shake(content); });
        go.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onClick(); });
        MkBtnText(go, label, 21);
    }

    void MkBigButton(string label, Action onClick)
    {
        MkBigButton(label, onClick, false);
    }

    void MkBarButton(Transform parent, string label, Action onClick)
    {
        GameObject go = MkBtnBase(parent, 46, TealDark);
        LayoutElement le = go.GetComponent<LayoutElement>();
        le.flexibleWidth = 1;
        go.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onClick(); });
        MkBtnText(go, label, 18);
    }

    void MkSmallButton(Transform parent, string label, bool active, Action onClick)
    {
        GameObject go = MkBtnBase(parent, 44, active ? Gold : TealDark);
        LayoutElement le = go.GetComponent<LayoutElement>();
        le.flexibleWidth = 1;
        go.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onClick(); });
        Text tx = MkText(go.transform, 14);
        tx.text = label;
        tx.color = active ? Ink : Color.white;
        tx.alignment = TextAnchor.MiddleCenter;
        RectTransform rt = tx.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(2, 2);
        rt.offsetMax = new Vector2(-2, -2);
    }

    void TradeRow(ResourceId r, string name, string buyLabel, string sellLabel, Action onBuy, Action onSell)
    {
        GameObject row = new GameObject("Row");
        row.transform.SetParent(content, false);
        UIStyle.Icon(row.transform, "icon_" + r.ToString().ToLower());
        HorizontalLayoutGroup hg = row.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 6;
        hg.childControlWidth = true;
        hg.childForceExpandHeight = false;
        LayoutElement rle = row.AddComponent<LayoutElement>();
        rle.minHeight = 52;
        rle.preferredHeight = 52;
        Text tx = MkText(row.transform, 17);
        tx.text = name;
        tx.color = Gold;
        tx.alignment = TextAnchor.MiddleLeft;
        LayoutElement tle = tx.gameObject.AddComponent<LayoutElement>();
        tle.flexibleWidth = 1;
        GameObject b1 = MkBtnBase(row.transform, 48, Teal);
        LayoutElement l1 = b1.GetComponent<LayoutElement>();
        l1.preferredWidth = 190;
        l1.minWidth = 190;
        b1.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onBuy(); });
        MkBtnText(b1, buyLabel, 16);
        GameObject b2 = MkBtnBase(row.transform, 48, TealDark);
        LayoutElement l2 = b2.GetComponent<LayoutElement>();
        l2.preferredWidth = 170;
        l2.minWidth = 170;
        b2.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onSell(); });
        MkBtnText(b2, sellLabel, 16);
    }

    void ActionRow(string l1, Action a1, string l2, Action a2, string l3, Action a3, string l4, Action a4, string l5, Action a5)
    {
        GameObject row = new GameObject("Actions");
        row.transform.SetParent(content, false);
        HorizontalLayoutGroup hg = row.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 6;
        hg.childControlWidth = true;
        hg.childForceExpandWidth = true;
        LayoutElement rle = row.AddComponent<LayoutElement>();
        rle.minHeight = 64;
        rle.preferredHeight = 64;
        RowAction(row.transform, l1, a1);
        RowAction(row.transform, l2, a2);
        RowAction(row.transform, l3, a3);
        RowAction(row.transform, l4, a4);
        RowAction(row.transform, l5, a5);
    }

    void RowAction(Transform parent, string label, Action onClick)
    {
        GameObject go = MkBtnBase(parent, 60, Teal);
        go.GetComponent<Button>().onClick.AddListener(delegate { Juice.Shake(content); });
        LayoutElement le = go.GetComponent<LayoutElement>();
        le.flexibleWidth = 1;
        go.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onClick(); });
        MkBtnText(go, label, 17);
    }

    void MapIsland(RectTransform sea, float ax, float ay, Texture2D tex, string name, int danger, bool here, Action onClick)
    {
        GameObject go = new GameObject("Isl");
        go.transform.SetParent(sea, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(ax, ay);
        rt.anchorMax = new Vector2(ax, ay);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(170, 152);
        GameObject im = new GameObject("Pic");
        im.transform.SetParent(go.transform, false);
        Image img = im.AddComponent<Image>();
        img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        im.AddComponent<Button>().onClick.AddListener(delegate { msg = ""; Sfx.Click(); onClick(); });
        RectTransform irt = im.GetComponent<RectTransform>();
        irt.anchorMin = new Vector2(0, 1);
        irt.anchorMax = new Vector2(1, 1);
        irt.offsetMin = new Vector2(27, -120);
        irt.offsetMax = new Vector2(-27, -4);
        GameObject lab = new GameObject("Name");
        lab.transform.SetParent(go.transform, false);
        Text tx = lab.AddComponent<Text>();
        tx.font = font;
        tx.fontSize = 14;
        tx.color = here ? Gold : Color.white;
        tx.alignment = TextAnchor.UpperCenter;
        tx.text = (here ? ">> " : "") + name + "\nPerigo " + danger;
        lab.AddComponent<Button>().onClick.AddListener(delegate { msg = ""; Sfx.Click(); onClick(); });
        RectTransform lrt = lab.GetComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0, 0);
        lrt.anchorMax = new Vector2(1, 0);
        lrt.offsetMin = new Vector2(-15, 0);
        lrt.offsetMax = new Vector2(15, 40);
    }

    void PortraitImage(Transform parent, float w, float h, Texture tex)
    {
        GameObject holder = new GameObject("Portrait");
        holder.transform.SetParent(parent, false);
        LayoutElement le = holder.AddComponent<LayoutElement>();
        le.minHeight = h + 8;
        le.preferredHeight = h + 8;
        GameObject go = new GameObject("Img");
        go.transform.SetParent(holder.transform, false);
        Image img = go.AddComponent<Image>();
        img.sprite = Sprite.Create((Texture2D)tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(w, h);
        UIStyle.Frame(holder.transform, w, h);
    }

    void PortraitButton(string label, Texture tex, Action onClick)
    {
        GameObject row = new GameObject("PRow");
        row.transform.SetParent(content, false);
        HorizontalLayoutGroup hg = row.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 8;
        hg.childControlWidth = true;
        hg.childForceExpandHeight = false;
        LayoutElement rle = row.AddComponent<LayoutElement>();
        rle.minHeight = 64;
        rle.preferredHeight = 64;
        GameObject go = new GameObject("PImg");
        go.transform.SetParent(row.transform, false);
        Image img = go.AddComponent<Image>();
        img.sprite = Sprite.Create((Texture2D)tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minWidth = 56;
        le.minHeight = 56;
        le.preferredWidth = 56;
        le.preferredHeight = 56;
        GameObject b = MkBtnBase(row.transform, 60, Teal);
        LayoutElement bl = b.GetComponent<LayoutElement>();
        bl.flexibleWidth = 1;
        b.GetComponent<Button>().onClick.AddListener(delegate { Juice.Flash(img); });
        b.GetComponent<Button>().onClick.AddListener(delegate { msg = ""; onClick(); });
        MkBtnText(b, label, 18);
    }

    int EnemyIndex(string name)
    {
        string[] pool = new string[] { "Saqueador", "Arraia Gigante", "Corsario", "Sirena Hostil", "Golem de Coral" };
        for (int i = 0; i < pool.Length; i++)
        {
            if (name.StartsWith(pool[i])) return i;
        }
        return 0;
    }

    Text TopChip(Transform parent, string iconKey)
    {
        GameObject go = new GameObject("Chip");
        go.transform.SetParent(parent, false);
        HorizontalLayoutGroup hg = go.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 4;
        hg.childControlWidth = false;
        hg.childForceExpandWidth = false;
        ContentSizeFitter cf = go.AddComponent<ContentSizeFitter>();
        cf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (iconKey != null)
        {
            Sprite s = UIStyle.Get(iconKey);
            if (s != null)
            {
                GameObject io = new GameObject("Ic");
                io.transform.SetParent(go.transform, false);
                Image img = io.AddComponent<Image>();
                img.sprite = s;
                img.raycastTarget = false;
                LayoutElement le = io.AddComponent<LayoutElement>();
                le.minWidth = 26;
                le.minHeight = 26;
                le.preferredWidth = 26;
                le.preferredHeight = 26;
            }
        }
        Text tx = MkText(go.transform, 16);
        tx.color = Gold;
        ContentSizeFitter tf = tx.gameObject.AddComponent<ContentSizeFitter>();
        tf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        return tx;
    }

    void ClickHint(Transform parent)
    {
        Sprite s = UIStyle.Get("click_1");
        if (s == null) s = UIStyle.Get("click_0");
        GameObject row = new GameObject("Hint");
        row.transform.SetParent(parent, false);
        HorizontalLayoutGroup hg = row.AddComponent<HorizontalLayoutGroup>();
        hg.spacing = 6;
        hg.childControlWidth = false;
        hg.childForceExpandWidth = false;
        ContentSizeFitter cf = row.AddComponent<ContentSizeFitter>();
        cf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (s != null)
        {
            GameObject io = new GameObject("Ic");
            io.transform.SetParent(row.transform, false);
            Image img = io.AddComponent<Image>();
            img.sprite = s;
            img.raycastTarget = false;
            LayoutElement le = io.AddComponent<LayoutElement>();
            le.minWidth = 28;
            le.minHeight = 28;
            le.preferredWidth = 28;
            le.preferredHeight = 28;
        }
        Text tx = MkText(row.transform, 14);
        tx.text = "clique para escolher";
        tx.color = Ink;
        ContentSizeFitter tf = tx.gameObject.AddComponent<ContentSizeFitter>();
        tf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
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
