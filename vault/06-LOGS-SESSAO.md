# 06 — Logs de Sessão

## 2026-09-19 — ses_f441bb278ffeK6WjqT2ncewDl3 — Setup vault
- Criado `vault/` com 00-06 + `AGENTS.md` com protocolo obrigatório + `opencode.jsonc`.
- Motivo: persistir contexto entre sessões; V2 usa AGENTS.md, não `instructions`.
- Próximo: usuário enviar GDD.

## 2026-09-20 — ses_f441bb278ffeK6WjqT2ncewDl3 — Scaffold v0.1 crew-journal
- Recebidos GDD/TDD/GED. Nome repo: crew-journal (público), template 2D Built-in.
- Descoberto: Unity 6000.6.2f1 corrompido (sem UnityPackageManager.exe, sem SceneManagement); `-createProject` falha. Projeto montado manualmente.
- Criados 10 Logic + GameManager/GameUI/PlaceholderArt + V1Bootstrap/V1Build. Verificação: LogicTests 12/12 (csc Framework); CHECK-A/B=0 via Roslyn do SDK + stubs uGUI (pegou ambiguidade real de Canvas, corrigida).
- Commit local f9081f4 (34 arquivos). Pendentes: reparo Unity via Hub, `gh auth login` + push, bootstrap da cena, build Windows.

## 2026-09-20 — ses_f441bb278ffeK6WjqT2ncewDl3 — Repo criado, Unity ainda bloqueado
- Repo https://github.com/guiolv/crew-journal criado (público) + push main OK (2 commits → remote).
- Bootstrap com install novo (`6000.6.2f1-x86_64`) falhou igual: UPM não executa (1392). Evidências: arquivo legível+hash OK, cópia em Temp também não executa, sem Zone.Identifier, só Defender ativo sem detecção Unity.
- Vault atualizado; aguardando usuário corrigir ambiente (exclusão Defender admin + reinstall) para rodar bootstrap + build.

## 2026-09-20 — ses_f441bb278ffeK6WjqT2ncewDl3 — v0.1 jogável verificada
- Usuário refez install (Hub aninha em `6000.6.2f1/6000.6.2f1-x86_64/`); UPM v9.31.1 executa; Editor abre o projeto.
- Achado real: `UnityEngine.UI` não existia — faltava `com.unity.ugui@2.6.0` no manifest (built-in, resolve local). Adicionado, compilação limpa.
- `V1Bootstrap.Build` via CLI criou `Assets/_Project/Scenes/World.unity`; `V1Build.BuildWindows` gerou `<repo>/Build/crew-journal.exe`.
- Smoke test: exe vivo 15s+, Player.log sem exceptions/erros. Push em dia (c4b5907 no remote).

## 2026-09-20 — ses_f441bb278ffeK6WjqT2ncewDl3 — v0.2 refs entregue e verificada
- Voyage.cs + TravelEvents + BattleState + ShipModules + Progression + FinishBattle; GameManager/UI reescritos no fluxo detalhe→viagem→evento→chegada→combate; tema navy+pergaminho.
- Verificação: LogicTests 24/24 (console), V1Playtest no engine 13/13 (recruta, módulo, viagem+evento, trade, missão, turnos, save/load, memorial), build Windows refeito, smoke limpo.
- Critério técnico TDD-35: 12/12. Gaps honestos pós-MVP: 5 tipos de evento (GDD ~10), 1 habilidade genérica, bio 1 linha, clima só em viagem. Multiplayer fora (pós-MVP por definição).

## 2026-09-20 — ses_f441bb278ffeK6WjqT2ncewDl3 — UI v3 (screenshot mostrava tudo colapsado)
- Causa: conteúdo sem layout groups (RectTransforms zerados, tudo empilhado no centro; só a barra inferior renderizava).
- Reescrita: ScrollRect+Viewport+Content(VerticalLayoutGroup+CSF), botões min 52-72px fontes 17-24, mapa com pins posicionados pelas coords do mundo, barra de progresso de viagem, Menu separado (Salvar/Carregar/Novo), trade em linhas, ações de combate em fileira.
- Build 01:46 + smoke limpo, push 59db8d3. Aguardando screenshot do usuário para confirmar jogabilidade.

## 2026-09-20 — ses_f441bb278ffeK6WjqT2ncewDl3 — v0.3 visual procedural entregue
- Proposta do usuário implementada: DNA puro + gramáticas de arquétipo + renderers pixel-art em estilo único + GDD-31/TDD-36 no vault.
- Verificação: lógica 30/30, playtest engine 13/13, type-check Unity 0 erros, build refeito, smoke limpo, arte conferida a olho via PNG exportado (retratos/ilhas/navios legíveis e consistentes).
- Cicatriz adquirida em batalha implementada (visual evolui com história). Contrato p/ biblioteca externa PNG definido, assets pendentes.

## 2026-09-22 — ses_f3944e4baffe0k3t7OxM1S180M — Scaffold gerador local de sprites (IA)
- `tools/sprite-gen/`: dna.py (hash emulado do C#), prompts.py (templates por profissão/arquétipo/navio), generate.py (ComfyUI local + --dry-run), post.py (key-out magenta + nearest + paleta), validate.py, comfy_workflow.json, library_plan.json (49 PNGs), README com setup DirectML p/ RX 6600.
- Unity: `IPartSource.cs` (SpriteLib chaves + ResourcesPartSource + fallback), hooks em Portrait/Island/ShipRenderer, `Pixel.ClonePixels`, `Resources/Art/{Portraits,Islands,Ships}/`. TDD-36.1 documenta contrato.
- Verificação: csc Roslyn 0 erros (Logic + 5 Unity); StableHash C# == porta Python em 6/6 strings (incl. overflow int32); LogicTests 30/30; JSONs válidos. Python NÃO executado (sem interpretador na máquina).
- Pendente (usuário): instalar Python 3.10 + ComfyUI --directml, `generate.py`, `post.py`, marcar import Point/No-compression.

## 2026-09-22 — ses_f3944e4baffe0k3t7OxM1S180M — Python instalado + testes do gerador executados
- Instalado Python 3.12.10 user-scope via winget + Pillow/requests. `dna.py` real: hash 6/6 == C#, 49 chaves únicas.
- `validate.py --manifest-only`: 0 erros. `generate.py --dry-run`: 49 entradas em `out/manifest.json`. `post.py` ponta a ponta com raw sintético: 512px magenta → 40x48, fundo transparente (1276px), 1 cor de paleta. Artefatos de teste removidos do repo.

## 2026-09-22 — ses_f3944e4baffe0k3t7OxM1S180M — UI kit + juice + downloads IA
- Gerador 79 entradas (49 sprites + 23 UI + 7 frames sea/sailship); `UIStyle.cs` (botoes/cards/pins/icones/progresso/moldura/compass) + `Juice.cs` (fade/punch/shake/flash/Frames/FillTo) fiados no GameUI com fallback total.
- Verificação: csc 0 erros (dir Unity inteira), LogicTests 30/30, validate 0 erros, dry-run 79.
- Toolchain: Python 3.12 + Pillow, aria2 multi-conexão; checkpoint SD1.5 completo (4.27GB), LoRA PixelArtRedmond OK, portable AMD v0.37.0 retomado com retry infinito após erro transitório 10051 (44%).
- Pendente: extrair portable → ComfyUI --directml → smoke visual → lote 79 → post → validate → build.

## 2026-09-22 — ses_f3944e4baffe0k3t7OxM1S180M — Downloads concluidos, extração
- Checkpoint + portable AMD baixados via aria2; hash SHA256 do portable confere com release (563da246…).
- Extraindo p/ `tools/comfy/` (gitignored). Próximo: modelos p/ checkpoints/loras + `run_amd_gpu` --directml.

## 2026-09-23 — ses_f441bb278ffeK6WjqT2ncewDl3 — DESIGN fixes implementados (skill)
- Rodado modo DESIGN sobre o review: spec em `vault/09-DESIGN-FIXES.md` (stance, em-jogo, ferido grave, vínculos; T2 farming adiado por falta de evidência).
- Implementado: `sailStance` + ApplyStance, `grave` + TreatWound + bandagem no retrato, BondEvent na chegada, linha "em jogo" + seletor de ordens no detalhe, botão Tratar na ficha.
- Curso do usuário: piso RNG explícito (risco ≥0.02, evento ≥0.05 — Cautela não elimina), +Redemoinho (3 escolhas), mares de tempestade (danger≥4 dobra Storm). Piratas/monstros já existiam (UnknownShip/Criatura + combate em ilhas P4+).
- Verificação: LogicTests 36/36, CHECK-B 0, build 22:22, playtest engine 13/13, smoke limpo.
- GDD atualizado (§10 eventos + §18 morte). Pendente: commit/push + playtest manual do usuário.

## 2026-09-23 — ses_f441bb278ffeK6WjqT2ncewDl3 — Mapa com ilhas desenhadas
- Pedido do usuário: desenho da ilha no mapa + nome abaixo. `MapPin` (botão de texto) → `MapIsland`: moldura (dourada = atual), desenho 162x108 clicável, nome + perigo abaixo (clicável).
- Verificação: CHECK-B 0, build 22:46, smoke limpo. Push 7eb221f5. Aguardando screenshot.

## 2026-09-23 — ses_f441bb278ffeK6WjqT2ncewDl3 — Ilhas sem moldura, no mar
- Fundo do sprite passou a transparente (só ilha + auréola rasa); moldura removida do mapa. Atual = nome dourado + `>>`.
- Verificação: preview PNG confere transparência, build + smoke limpos.

## 2026-09-24 — ses_f441bb278ffeK6WjqT2ncewDl3 — Isométrico verificado (outra sessão fez, eu integrei)
- Mudança top-down→isométrico veio da outra sessão (`Iso.cs` + `IslandRenderer` diamante com penhascos + pins `Iso.Pin`); encontrei e reconciliei 1 conflito: o rewrite dela tinha voltado o fundo azul chapado — reapliquei transparente (só ilha + auréola).
- Verificação: CHECK 0 (Runtime+Editor), preview do diamante conferido a olho, build 23:45, smoke limpo.
- Não commitei: arquivos da outra sessão continuam uncommited com ela (só mexi no Fill do IslandRenderer dela).

## 2026-09-23 — ses_f441bb278ffeK6WjqT2ncewDl3 — Lens review da skill `skills/`
- Rodada skill game-design (Schell) em modo REVIEW sobre crew-journal → `vault/08-LENS-REVIEW.md`.
- Resultado: 6 perspectivas (A/C/D/F/G/H); 6 tensões, maior risco = permadeath sem apego/causa legível; 3 alternativas (A capitão ativo, B morte em 2 etapas, C vínculos primeiro); protocolo de playtest de 30 min com 6 perguntas (serve o playtest pendente de 04-TAREFAS); questões abertas (balanceamento GED sem sessão real).
- Nada implementado — review é diagnóstico + plano de validação, não mudança de código.

## 2026-09-24 — ses_f441bb278ffeK6WjqT2ncewDl3 — Game Feel itens 1-4 implementados
- Fast-forward (`sailTick` 1.1→0.25 botão APRESSAR, reseta por viagem); inimigos ritmados (`EnemyTurnPending`/`StepEnemy` 0.35s + Shake + Hit via `OnEnemyHit`; trava input com `stepping`; `StartBattle` drena se inimigo abre); SFX procedurais (`Sfx.cs`: click/coin/hit/victory/defeat + mute PlayerPrefs no Menu; coin por diff de dinheiro); tradeoff da medicina na ficha do grave.
- Achado no caminho: `CurrentCrew` sem auto-drain quebrava callers (inimigo rápido primeiro) — padrão drain-antes-de-agir nos 3 loops + dreno inicial no `StartBattle`. Teste novo `step-single` prova granularidade (1 inimigo = 1 linha de log).
- Verificação: LogicTests 38/38, CHECK-B 0 (+AudioModule no check), build 23:08, playtest engine 14/14, smoke limpo. Desvio doc.: sem flash no retrato (rebuild da UI invalida a ref) — Shake no content + HPs atualizando por passo cobrem o sinal.
- GDD §7: linha do fast-forward. Não tocado: arquivos da outra sessão em curso.

## 2026-09-24 — ses_f441bb278ffeK6WjqT2ncewDl3 — Game Feel audit (skill)
- Rodada skill `skills/Game Feel` em modo AUDITAR sobre crew-journal → `vault/10-GAME-FEEL-AUDIT.md`. Ponte com lentes aplicada (08 preservado como entrada).
- Limitação declarada: sem controle em tempo real — só eixos Resposta/Contexto/Polish/Metáfora/Regras.
- 4 mudanças priorizadas com cadeia causal: fast-forward da viagem, resolução inimiga legível (delay+flash), SFX procedurais mínimos (zero áudio hoje — grep confirma), conflito da medicina visível.
- Nada implementado — auditoria, não código. Menor playtest proposto: fast-forward A/B em 2 viagens longas.
