# 04 — Tarefas

## Em progresso
- [ ] Unity AINDA bloqueado: `UnityPackageManager.exe` do install novo não executa (erro 1392, até via cópia em Temp; sem Zone.Identifier; Defender sem detecção Unity) — causa fora do Unity (AV/disco). Usuário: exclusão no Defender (admin) + reinstalar limpo
- [x] `gh auth login` + repo criado: https://github.com/guiolv/crew-journal (push main OK)

## Backlog
- [ ] Abrir `game/` no Editor reparado → rodar `CrewJournal.Editor.V1Bootstrap.Build` via CLI
- [ ] Play na cena World: validar loop mapa→viagem→ilha→combate→diário→save
- [ ] Build Windows via `CrewJournal.Editor.V1Build.BuildWindows`
- [ ] Trocar placeholders por arte final; eventos com escolhas (GDD secao 10)

## Feito
- [x] 2026-09-19 — Criar vault + configurar OpenCode (AGENTS.md + opencode.jsonc)
- [x] 2026-09-20 — Scaffold v0.1 crew-journal: 10 arquivos Logic + GameManager/GameUI/PlaceholderArt + V1Bootstrap/V1Build; testes 12/12; type-check Unity OK; commit f9081f4
