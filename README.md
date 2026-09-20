# crew-journal

MVP jogável v0.1 — adventure/management/RPG top-down cartográfico (Unity 6000.6.2f1, 2D Built-in).

Baseado em `vault/05-GDD/{GDD,TDD,GED}.md`.

## Loop v0.1
Escolher destino no mapa → navegar (progresso + consumo + risco) → evento → chegar na ilha → comércio / missão / recrutamento → gerenciar tripulação → combater (turn-based simplificado) → diário + memorial → salvar/carregar JSON.

## Estrutura
- `game/` — projeto Unity (`Assets/_Project/...`)
- `vault/` — memória persistente OpenCode (ler primeiro em toda sessão)
- `AGENTS.md` — protocolo obrigatório de sessão

## Rodar
1. Abrir `game/` no Unity Hub (6000.6.2f1)
2. Abrir cena `Assets/_Project/Scenes/World.unity`
3. Play

Ou via CLI:
```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.6.2f1\Editor\Unity.exe" -batchmode -projectPath ./game -openScene Assets/_Project/Scenes/World.unity -logFile -
```

## Build Windows
```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.6.2f1\Editor\Unity.exe" -batchmode -nographics -quit -projectPath ./game -executeMethod CrewJournal.Editor.V1Build.BuildWindows -logFile ./unity-build.log
```

## Save
`Application.persistentDataPath/save_01.json`

## Placeholders v0.1
Sprites procedurais gerados em runtime (sem assets externos, sem licença). Trocar por arte final depois.
