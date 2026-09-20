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
