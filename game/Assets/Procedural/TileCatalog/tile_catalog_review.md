# Tile Catalog — review manual (2026-09-24)

Gerado por `tools` (script em Temp/opencode/catalog.py — mover p/ repo se virar rotina).
Gerador procedural NÃO tocado.

## Método

1. Scan dos packs em `vault/07-REFERENCIAS/Assets/{monochrome-pirates,monochrome-rpg}`.
2. Tamanho verificado por arquivo: 272/272 são 16×16. Tilesheets: 17×8, 136 tiles/pack.
3. Contact sheets com id + pack + índice (neste diretório).
4. Classificação: allowlist curta verificada (zoom + uso em produção) + regra
   de honestidade — estrutural incerto = `needs_review: true`, `confidence: 0.0`,
   sockets `UNKNOWN`. Nenhuma conexão inventada.

## Contagens

- Assets encontrados (world packs): 272 tiles (136 pirates + 136 rpg)
- Tiles candidatos no catálogo: 272 (todos listados; uso em produção só p/ revisados)
- Excluídos (fora de escopo, outros folders): input-prompts 823 arquivos,
  ui-pack 514 PNGs, interface-sounds 100 OGGs
- Alta confiança (`needs_review: false`): 4 (1 empty + 3 water flats)
- Aguardando revisão humana: 268
- Com rotação liberada: 4 (3 water + 1 terrain simétrico)
- Suspeitos multi-tile (`module_candidate`): 46

## Alta confiança (uso atual em produção)

- `pirates_tile_0006/0007/0008`: water/open_water, sockets WATER×4, sem travessia.
- Empty (tile vazio): categoria `empty`, peso 0.
- Rotação liberada só em flats simétricos verificados por diff de pixels.

## Sugestões aguardando validação humana

- Costa areia (sockets propostos do zoom): 0003/0004/0005/0020/0022/0037/0038/0039 + fill 0017/0021, conf 0.5–0.6.
- Grama fill rpg 0000, conf 0.5.
- Props/construções/personagens (função evidente no zoom), conf 0.6: dock, anchor,
  barrel, chest, palm, bottle, fish, rock, skull, sword, hill, planks, sail,
  tree, castle, hut, house, cross, cave + 12 figures.
- Dirt rpg 0007, plaza, beach, field: SEM tile confirmado — `unknown`, não usar.

## Módulos suspeitos (confirmar antes de montar [X,Y])

- Pirates 0014–0016 (velas?), 0031–0033, 0043–0050 (casco/convés), 0060–0067,
  0094–0101, 0111–0118, 0128–0135 (partes de navio/canhão). Todos `needs_review`.

## Rotações suspeitas

- Somente flats simétricos (verificação por diff, não por palpite).
- Decorações, construções, personagens, placas: `[0]` — nunca rotacionar.

## Para produção (quando o gerador for ligado nisso)

- Só consumir `needs_review: false`. Hoje: water + empty.
- Categorias aptas a topologia: `terrain`, `coast`, `water`. `decoration`, `object`,
  `building`, `landmark`, `character`, `item` nunca definem topologia.
- `UNKNOWN` nunca entra em produção.

## Em aberto

- Confirmar geometria das 8 bordas + fill de areia com o humano.
- Confirmar grass flat rpg + dirt.
- Sem `.asset`/prefab/ScriptableObject de tiles no projeto (verificado) — gerador lê PNG via Resources.
- Paths do projeto: `game/Assets/Resources/Art/kenney/{tiles,Portraits,UI,prompts}` + `Audio/`.
