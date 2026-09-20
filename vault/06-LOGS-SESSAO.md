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
