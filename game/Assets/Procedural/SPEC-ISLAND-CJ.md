# SPEC-ISLAND-CJ — Contrato do layout estrutural de ilhas (SDD)

Fonte-metodologia: `vault/13 - LIBRARY/AI System Engineering v2.4.0` (§7 SDD, §11 evals, lab SPEC-ISLAND-001).
Estado: **RED** — spec + evals existem, implementação NÃO. `IslandRenderer.cs` intacto.

## 1. Goal declarativo

Toda ilha possui um layout estrutural determinístico `N×N` (função pura de `seed`)
que garante: borda sempre oceano, pelo menos 1 célula de terra, invariante de
adjacência grama/areia/oceano, e ponto de desembarque (`spawn`) em terra.

## 2. Fora de escopo (explícito)

- `IslandRenderer.cs` NÃO é tocado por este spec (continua blob visual).
- Adoção do layout pelo renderer (pintar a partir do grid) é fase posterior, com spec próprio.
- `dirt`/props (doca, casa, palmeira) são overlay de gameplay, NÃO topologia.
- Catálogo de 272 tiles: só flats verificados entram em produção; resto segue `needs_review`.

## 3. Requisitos funcionais

- R1. Grade `width×height`, `8 ≤ N ≤ 64`, default `8` (casa com as 8×8 células do renderer).
- R2. Biomas por elevação + falloff radial: `Ocean / Sand / Grass` (limiares na implementação).
- R3. Falloff garante borda `Ocean` em todo o perímetro, qualquer seed.
- R4. Se nenhuma célula de terra emergir, forçar núcleo `2×2` de `Sand` no centro.
- R5. `spawn` = célula de terra (`Sand` ou `Grass`) mais próxima do centro; determinístico.
- R6. RNG dedicado com seed (`System.Random(seed)` ou equivalente); zero aleatoriedade global.

## 4. Invariantes (hard constraints)

- I1. Borda: toda célula do perímetro é `Ocean`.
- I2. Adjacência: `Grass` NUNCA é ortogonalmente adjacente a `Ocean` (buffer de `Sand` obrigatório).
- I3. Spawn sempre em `Sand` ou `Grass`, nunca em `Ocean`.
- I4. Determinismo: mesma `(width, height, seed)` → mapa idêntico, bit a bit.
- I5. Performance: `64×64` gera em `< 50ms` (máquina dev).

## 5. Definition of Done

- [ ] `IslandLayout.Generate` compila sem warnings no csc + Unity 6.
- [ ] `IslandEvals.exe`: 7/7 EVALs verdes nos tamanhos {8, 16, 32, 64}.
- [ ] `LogicTests.exe` principal continua verde (sem regressão).
- [ ] `IslandRenderer.cs` inalterado (diff vazio) até a fase de adoção.

## 6. Evals (harness separado, `game/Tools/LogicTests/IslandEvals.cs`)

| Eval | Verifica |
|---|---|
| EVAL_EdgesOcean | I1 em {8,16,32,64} |
| EVAL_HasLand | ≥1 célula `Sand`/`Grass` |
| EVAL_Adjacency | I2 varredura total |
| EVAL_SpawnOnLand | I3 |
| EVAL_Deterministic | I4 (mesma seed 2×) |
| EVAL_DistinctSeeds | seeds distintas → mapas distintos |
| EVAL_Perf64 | I5 |

Falha de geração: NUNCA colocar tile arbitrário; reportar `seed + célula + constraint` (regra da skill).
