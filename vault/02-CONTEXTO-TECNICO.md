# 02 — Contexto Técnico

## Stack
- Engine: Unity (versão a definir após GDD — checar `unity --version` / `Unity -version` / Hub instalado)
- Interface: Unity CLI (batchmode, -nographics, -executeMethod, -buildTarget)
- Linguagem: C# (Unity)
- SO dev: Windows (win32) — comandos via powershell
- Memória: vault local em `./vault/`, config OpenCode em `AGENTS.md` + `opencode.jsonc`

## Estrutura prevista
```
OPENCODE/
├── AGENTS.md
├── opencode.jsonc
├── vault/
│   ├── 00-INDEX.md
│   ├── 01-VISAO-GERAL.md
│   ├── 02-CONTEXTO-TECNICO.md
│   ├── 03-DECISOES.md
│   ├── 04-TAREFAS.md
│   ├── 05-GDD/
│   └── 06-LOGS-SESSAO.md
└── game/               # futuro projeto Unity (a criar)
    ├── Assets/
    ├── Packages/
    └── ProjectSettings/
```

## Unity CLI — verificar antes de usar
Antes de assumir comando, inspecionar localmente:
- `where unity` / `where Unity` / Unity Hub path
- Versão instalada e módulos (Android, Windows, etc.)
- Em sessão futura: rodar descoberta e registrar comandos reais aqui.

Padrões esperados (confirmar):
- `Unity -batchmode -nographics -projectPath ./game -executeMethod <Classe.Metodo> -quit -logFile -`
- `Unity -batchmode -buildTarget StandaloneWindows64 ... -quit`

## Regras
- Sempre inspecionarworkspace antes de comando genérico.
- Preferir comandos verificáveis e logs em arquivo.
- Não commitar Library/, Temp/, Logs/ do Unity.
