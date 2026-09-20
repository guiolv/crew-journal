# TDD — TECHNICAL DESIGN DOCUMENT

## Projeto [Nome Provisório]

**Versão:** 0.3 — MVP
**Engine:** Unity
**Linguagem:** C#
**Arquitetura:** Data-driven / Event-driven

---

# 1. Objetivo

Criar uma arquitetura pequena, modular e extensível capaz de suportar o MVP sem antecipar sistemas complexos do jogo completo.

O código deve permitir adicionar posteriormente:

* mais ilhas;
* mais personagens;
* mais navios;
* mais eventos;
* multiplayer;
* novas regiões.

---

# 2. Arquitetura

```text
Core
├── GameState
├── GameLoop
├── Time
├── EventBus
└── Save

World
├── WorldGenerator
├── IslandGenerator
├── IslandRuntime
└── Weather

Navigation
├── RouteSystem
├── NavigationSystem
├── NavigatorSystem
└── TravelEventSystem

Crew
├── CrewSystem
├── CharacterGenerator
├── RelationshipSystem
├── JobSystem
└── DeathSystem

Ship
├── ShipSystem
├── CargoSystem
└── ModuleSystem

Economy
├── EconomySystem
├── TradingSystem
└── ResourceSystem

Mission
├── MissionSystem
└── MissionGenerator

Combat
├── BattleSystem
├── TurnSystem
├── ActionSystem
└── StatusEffectSystem

Narrative
├── JournalSystem
└── MemorialSystem

UI
├── MapUI
├── CrewUI
├── ShipUI
├── IslandUI
├── MissionUI
├── JournalUI
└── CombatUI
```

---

# 3. Dados estáticos

Usar `ScriptableObject`.

```text
CharacterDefinition
JobDefinition
TraitDefinition
SkillDefinition
ShipDefinition
IslandArchetype
ResourceDefinition
ItemDefinition
EnemyDefinition
MissionDefinition
EventDefinition
WeatherDefinition
```

---

# 4. Runtime Data

Usar classes serializáveis.

```text
CharacterData
CrewData
ShipData
IslandData
MissionData
WorldData
JournalEntry
DeathRecord
RelationshipData
```

Nunca armazenar o estado de uma partida diretamente em ScriptableObjects.

---

# 5. Game State

```csharp
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
```

`GameStateManager` controla o estado atual.

---

# 6. Event Bus

Criar um sistema simples de eventos.

Exemplos:

```text
CharacterRecruitedEvent
CharacterDiedEvent
IslandDiscoveredEvent
MissionCompletedEvent
ShipPurchasedEvent
BattleStartedEvent
BattleEndedEvent
ReputationChangedEvent
JournalEvent
```

O objetivo é reduzir dependências entre sistemas.

---

# 7. World Generator

Entrada:

```text
WorldSeed
```

Saída:

```text
WorldData
```

O MVP pode gerar as cinco ilhas iniciais.

Cada ilha recebe:

```text
IslandID
Archetype
Position
Size
Resources
Population
Economy
NPCs
Missions
Seed
```

---

# 8. Island Generator

Pipeline:

```text
Seed
 ↓
Archetype
 ↓
Geography
 ↓
Resources
 ↓
Population
 ↓
Economy
 ↓
NPCs
 ↓
Missions
```

A geração deve ser determinística.

---

# 9. Navigation System

Input:

```text
Origin
Destination
ShipData
NavigatorData
Weather
```

Output:

```text
TravelResult
```

Contendo:

```text
TravelTime
Risk
Consumption
PossibleEvents
RouteQuality
```

---

# 10. Travel Simulation

O MVP não precisa mover o navio fisicamente pelo oceano durante toda a viagem.

O sistema pode:

1. calcular rota;
2. iniciar viagem;
3. mostrar progresso;
4. gerar evento;
5. concluir viagem.

Isso reduz muito o custo de desenvolvimento.

---

# 11. Map

O mapa deve usar:

* câmera ortográfica;
* sistema de coordenadas 2D;
* ilhas como pontos de interesse;
* navio como unidade móvel;
* linhas de rota.

O mapa deve ter aparência cartográfica.

---

# 12. Island Interaction

Ao chegar à ilha:

```text
Map
 ↓
Island State
 ↓
Island UI
```

A interface apresenta:

* nome;
* tipo;
* reputação;
* comércio;
* missões;
* recrutamento;
* recursos;
* saída.

---

# 13. Character Generator

O gerador utiliza:

```text
NamePool
AgeRange
AppearancePool
JobPool
TraitPool
BackgroundPool
StatRanges
```

Cada personagem recebe um `UniqueID`.

A seed pode ser:

```text
WorldSeed + CharacterID
```

Isso torna o personagem reproduzível.

---

# 14. Character Runtime

```csharp
[Serializable]
public class CharacterData
{
    public string id;
    public string name;

    public int age;
    public int level;

    public Stats stats;

    public List<string> jobs;
    public List<string> traits;
    public List<string> skills;

    public bool alive;

    public int morale;
    public int loyalty;
}
```

---

# 15. Jobs

Cada job possui:

```text
JobDefinition
```

Com:

```text
ID
Name
PrimaryStats
PassiveEffects
AvailableActions
```

---

# 16. Relationships

Estrutura:

```text
RelationshipData
{
    CharacterA
    CharacterB
    Affinity
    Trust
    Fear
    Respect
}
```

No MVP, os valores podem ser simples.

---

# 17. Ship System

```csharp
ShipData
{
    id
    definitionId
    hull
    maxHull
    capacity
    crewCapacity
    cargoCapacity
    speed
    modules
    cargo
}
```

---

# 18. Resource System

Recursos devem ser identificados por ID.

```text
food
water
wood
metal
medicine
money
```

O inventário usa:

```text
Dictionary<ResourceID, Amount>
```

---

# 19. Economy System

Cada ilha possui preços locais.

```text
BasePrice
×
SupplyModifier
×
DemandModifier
×
ReputationModifier
```

O resultado é o preço final.

---

# 20. Mission System

Uma missão possui:

```text
MissionDefinition
MissionRuntimeData
```

Estados:

```text
Available
Accepted
InProgress
Completed
Failed
Abandoned
```

---

# 21. Combat System

O combate inicia através de:

```text
BattleRequest
```

Contendo:

```text
PlayerParty
EnemyParty
BattleContext
```

---

# 22. Turn Manager

Responsabilidades:

* determinar iniciativa;
* controlar turno;
* aceitar ação;
* executar ação;
* processar efeitos;
* verificar vitória/derrota.

Não deve conhecer UI.

---

# 23. Combat Action

Interface:

```csharp
public interface ICombatAction
{
    bool CanExecute();
    void Execute();
}
```

Implementações:

```text
AttackAction
DefendAction
SkillAction
ItemAction
FleeAction
```

---

# 24. Death System

O `DeathSystem` recebe:

```text
CharacterID
BattleID
Cause
Location
```

Gera:

```text
DeathRecord
```

E dispara:

```text
CharacterDiedEvent
```

---

# 25. Journal System

Escuta eventos.

Exemplo:

```text
CharacterDiedEvent
        ↓
JournalSystem
        ↓
JournalEntry
```

O sistema não deve saber detalhes de combate.

---

# 26. Save System

Formato inicial:

**JSON**

Arquivo:

```text
save_01.json
```

Salvar:

```text
World
Crew
Ship
Resources
Reputation
Missions
Journal
Memorial
Time
```

---

# 27. Seed

Todas as gerações devem utilizar uma seed central.

```csharp
WorldSeed
```

Subseeds:

```text
IslandSeed
CharacterSeed
MissionSeed
EventSeed
```

Nunca utilizar `Random` global sem controle para conteúdo que precise ser reproduzível.

---

# 28. UI Architecture

A UI não deve alterar diretamente os dados.

Fluxo:

```text
UI
 ↓
Controller
 ↓
System
 ↓
Runtime Data
 ↓
Event
 ↓
UI Update
```

---

# 29. Cena Map

Objetos principais:

```text
World
├── WorldManager
├── Camera
├── Ocean
├── Islands
├── PlayerShip
├── MapUI
└── EventSystem
```

---

# 30. Cena Combat

```text
Combat
├── BattleManager
├── PlayerParty
├── EnemyParty
├── BattleCamera
├── CombatUI
└── Effects
```

---

# 31. MVP Scene Flow

```text
Boot
 ↓
Map
 ↓
Sailing
 ↓
Event
 ↓
Island
 ↓
Map
 ↓
Sailing
 ↓
Combat
 ↓
Map
```

---

# 32. Estrutura de projeto

```text
Assets/_Project/

Scripts/
    Core/
    World/
    Navigation/
    Crew/
    Ship/
    Economy/
    Mission/
    Combat/
    Narrative/
    UI/
    Save/

Data/
    Characters/
    Jobs/
    Traits/
    Skills/
    Ships/
    Islands/
    Resources/
    Missions/
    Events/
    Enemies/

Scenes/
    Boot
    World
    Combat

Prefabs/
    Ships/
    Islands/
    Characters/
    UI/
```

---

# 33. Ferramentas do MVP

Preferir sistemas nativos da Unity inicialmente.

Evitar adicionar dependências externas sem necessidade.

O objetivo é reduzir riscos e acelerar o protótipo.

---

# 34. Testes prioritários

Automatizar principalmente:

### Navigation

Mesma seed + mesmos inputs = mesmo resultado.

### Character Generation

Seed válida sempre gera personagem válido.

### Economy

Preço nunca pode ficar abaixo do limite definido.

### Combat

HP, dano, turnos e morte precisam ser determinísticos.

### Save

Salvar e carregar deve preservar o estado.

### Death

Personagem morto nunca pode retornar ao estado vivo.

---

# 35. Critério técnico do MVP

O protótipo estará tecnicamente pronto quando:

* uma jornada puder ser iniciada;
* o jogador puder navegar;
* eventos puderem acontecer;
* ilhas puderem ser visitadas;
* personagens puderem ser recrutados;
* recursos puderem ser administrados;
* missões puderem ser concluídas;
* combate puder ocorrer;
* personagens puderem morrer;
* o diário registrar acontecimentos;
* o jogo puder ser salvo;
* o save puder ser carregado.

Multiplayer não faz parte do critério.

---

# 36. Visual procedural modular

```text
Visual
├── Dna (puro: CharacterVisual, IslandVisual, ShipVisual)
├── Grammar (zonas por arquetipo, distribuicoes por profissao)
├── Render (Unity: PortraitRenderer, IslandRenderer, ShipRenderer -> Texture2D)
└── Cache (por id; texturas 32-96px, point filter)
```

Contrato: DNA é função pura de `(worldSeed, id)` — nunca serializado, sempre idêntico. Estado visual mutável mínimo e serializado: `CharacterData.scar`, `CharacterData.outfitMod` (0 = profissão, cicatriz adquirida em batalha). Renderers desggiados por `IPartSource`: hoje procedural por código; amanhã PNGs `hair_12.png` etc. via Resources com fallback procedural — sem mudar chamadas.

Retratos 40x48 em grade fixa (cabeca y30-42, corpo y16-30). Ilhas 96x64 (silhueta + bioma + peças por zona). Navios 64x40 (casco/mastro/vela por defId+seed).
