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
- Editor: `C:\Program Files\Unity\Hub\Editor\6000.6.2f1\6000.6.2f1-x86_64\Editor\Unity.exe` (versão 6000.6.2f1, UPM v9.31.1 OK desde 2026-09-20; Hub aninha installs em subpastas)
- Roslyn moderno: `.../Editor/Data/DotNetSdk/dotnet.exe` + `sdk/8.0.318/Roslyn/bincore/csc.dll`
- Ref netstandard: `.../Data/DotNetSdk/packs/NETStandard.Library.Ref/2.1.0/ref/netstandard2.1/netstandard.dll`
- Engine DLLs: `.../Editor/Data/Managed/UnityEngine/` (CoreModule, UIModule, TextRenderingModule)
- ATENÇÃO 2026-09-20: instalação corrompida — falta `Data/Resources/PackageManager/Server/UnityPackageManager.exe` e módulos SceneManagement. `-createProject` falha. Projeto foi montado manualmente (Assets/Packages/ProjectSettings). Reparar via Unity Hub (reinstalar 6000.6.2f1) antes de abrir/compilar no Editor.
- Verificação sem Editor: `game/Tools/LogicTests/` (12 testes, csc Framework) + `game/Tools/CompileCheck/` (Roslyn + stubs uGUI, CHECK-A/B=0).
- Pacote uGUI: `com.unity.ugui@2.6.0` (built-in, resolve local sem rede) — obrigatório no manifest, sem ele `UnityEngine.UI` não existe.
- Build Windows: `V1Build` grava em `<repo>/Build/crew-journal.exe` (dataPath + `/../../Build`). Smoke test 2026-09-20: exe abre, vivo 15s+, Player.log sem exceptions.

Padrões esperados (confirmar):
- `Unity -batchmode -nographics -projectPath ./game -executeMethod <Classe.Metodo> -quit -logFile -`
- `Unity -batchmode -buildTarget StandaloneWindows64 ... -quit`

## Regras
- Sempre inspecionarworkspace antes de comando genérico.
- Preferir comandos verificáveis e logs em arquivo.
- Não commitar Library/, Temp/, Logs/ do Unity.
