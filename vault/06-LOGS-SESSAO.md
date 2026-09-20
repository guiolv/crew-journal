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
