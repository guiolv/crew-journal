# 03 — Decisões

Formato: `## YYYY-MM-DD — Título` + Contexto / Decisão / Consequência.

---

## 2026-09-19 — Vault como memória persistente
- Contexto: necessidade de contexto entre sessões.
- Decisão: usar `./vault/` + `AGENTS.md` com protocolo obrigatório de leitura/escrita. `instructions` do `opencode.jsonc` NÃO é usado (V2 não resolve esse campo).
- Consequência: toda sessão deve ler vault primeiro e atualizar no fim.

## 2026-09-20 — Arquitetura v0.1: lógica pura + adaptadores Unity
- Contexto: Editor Unity corrompido impede compilação/build; TDD exige sistemas desacoplados da UI.
- Decisão: `Assets/_Project/Scripts/Logic/` sem nenhuma referência UnityEngine (DTOs + GameSession + Save). `Scripts/Unity/` (GameManager, GameUI) e `Scripts/Editor/` (V1Bootstrap, V1Build) como camada fina.
- Consequência: lógica verificável via csc + testes console (12/12); UI/Editor compilam no Editor após reparo.

## 2026-09-20 — UI construída por código + placeholders procedurais
- Contexto: sem Editor funcional não há como montar cenas/prefabs; sem assets externos.
- Decisão: GameUI.cs monta Canvas uGUI inteiro em runtime; PlaceholderArt.cs gera sprites 16x16 por código; V1Bootstrap cria a cena World via `-executeMethod`.
- Consequência: zero assets binários no repo; trocar por arte final depois sem mexer em lógica.

## 2026-09-20 — Projeto Unity montado manualmente
- Contexto: `Unity -createProject` falha (PackageManager ausente).
- Decisão: criar `game/Assets/`, `game/Packages/manifest.json` mínimo, `game/ProjectSettings/ProjectVersion.txt` (6000.6.2f1) à mão; commit local `f9081f4`.
- Consequência: ao reparar o Editor, abrir `game/` e rodar bootstrap; GitHub push pendente de `gh auth login`.

## 2026-09-20 — Editor novo também com UPM corrompido + repo GitHub criado
- Contexto: usuário reinstalou → `6000.6.2f1-x86_64` tem UPM (96MB, legível, hash OK) mas Windows recusa execução (erro 1392) até em cópia no Temp; sem Mark-of-Web; só Windows Defender ativo, sem detecção Unity. Install antigo sem o exe, novo com exe corrompido = causa fora do Unity.
- Decisão: parar de sondar sem admin; remediation no lado do usuário (exclusão Defender + reinstall limpo). Repo `guiolv/crew-journal` público criado e com push (main).
- Consequência: bootstrap da cena + build Windows ficam para quando o Editor abrir; comandos prontos no README.

## 2026-09-20 — v0.2 conforme refs: viagem em etapas, combate por turnos, módulos, XP
- Contexto: refs "Sail & Survive" exigem preview+clima, eventos com 3 escolhas, combate com ações, módulos, ficha/XP, tema navy+pergaminho.
- Decisão: `Voyage.cs` (Preview/Begin/Tick/Choose/Arrive + clima determinístico), `TravelEvents.cs` (3 opções/evento), `BattleState` interativo (fila por velocidade, atacar/pesado/defender/item/fugir), `ShipModules.cs` (5 tipos com efeito), `Progression.cs` (XP/nível), `FinishBattle` compartilhado, UI reescrita no tema.
- Consequência: testes 24/24; saves antigos continuam lendo (membros novos têm default).
