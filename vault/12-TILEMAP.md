# 12 — Guia de mapeamento de tiles (para o humano preencher)

Objetivo: você olha `../07-REFERENCIAS/tilesheets/{pirates_full,rpg_full}.png`
(números amarelos = índice do tile) e me diz qual tile é o quê.
Eu transformo em autotile de costa de verdade no `IslandRenderer`.

## Como responder

Me mande linhas assim (aqui no chat ou editando este arquivo):

```text
pirates tile_0020 = sand_n (areia com agua em cima)
pirates tile_0048 = house
```

Só corrija/complete — meus chutes já estão nas tabelas.

## Modelo de borda (o que eu preciso)

Para cada célula de AREIA, olho os 8 vizinhos. Se o vizinho é ÁGUA, a célula
usa o tile de borda daquele lado:

```text
            água ao NORTE  →  tile mostra areia embaixo, água em cima
            água a NE+N+E   →  canto externo (curva p/ fora)
            água só na diagonal (ex: só NE) → canto interno (curva p/ dentro)
```

Exemplo da sua ref (ilha com praia contornando a grama):

```text
        ~~~~~~~~~~~~~  água
        ~~~█████████~~  espuma/areia externa
        ~~~██▓▓▓▓███~~  areia + grama dentro
        ~~~██▓⌂⌂▓███~~  grama + casa
```

## Tabela 1 — base (faixas chapadas)

| Papel | Confirmado |
|---|---|
| water (mar) | pirates 0006 (+0007/0008 variantes) + speckles 0001/0002 como overlay |
| sand (areia) | pirates 0017 (fill) |
| grass (grama) | pintada chapada na rampa do pack (sem tile dedicado confirmado) |
| dirt (terra) | rpg 0007 |

## Tabela 2 — bordas de areia (CONFIRMADO via zoom + teste)

Bitmask por água vizinha (N/S/E/W). Implementado no `IslandRenderer`.

| Papel | Tile | Geometria confirmada |
|---|---|---|
| sand fill | pirates 0017 | quadrado chapado claro |
| sand_t (água N) | pirates 0004 | faixa horizontal no topo |
| sand_b (água S) | pirates 0038 | faixa horizontal embaixo |
| sand_l (água W) | pirates 0020 | faixa vertical esquerda |
| sand_r (água E) | pirates 0022 | faixa vertical direita |
| sand_tl | pirates 0003 | curva externa |
| sand_tr | pirates 0005 | curva externa |
| sand_bl | pirates 0037 | curva externa |
| sand_br | pirates 0039 | curva externa |
| cantos internos | sand fill (aprox.) | sem tile dedicado confirmado |

## Tabela 3 — props e construções (já em uso, só corrija se eu errei)

| Papel | Em uso | Correto? |
|---|---|---|
| dock | pirates 0009 | |
| barrel | pirates 0102 | |
| house | rpg 0048 | |
| hut | rpg 0049 | |
| castle | rpg 0033 | |
| cross | rpg 0064 | |
| cave | rpg 0052 | |
| palm | pirates 0104 | |
| tree | rpg 0013 | |
| rock | pirates 0121 | |
| chest | pirates 0103 | |
| skull | pirates 0122 | |
| hill | pirates 0052 | |
| bottle | pirates 0107 | |
| fish | pirates 0119 | |
| anchor | pirates 0086 | |
| planks | pirates 0094 | |

## Tabela 4 — personagens (retratos e inimigos, já em uso)

| Papel | Em uso | Correto? |
|---|---|---|
| navigator | pirates 0126 | |
| cook | rpg 0120 | |
| medic | rpg 0122 | |
| carpenter | rpg 0121 | |
| fighter | pirates 0125 | |
| shooter | pirates 0124 | |
| sailor | rpg 0119 | |
| inimigo Saqueador | pirates 0125 | |
| inimigo Arraia | rpg 0123 | |
| inimigo Corsario | pirates 0127 | |
| inimigo Sirena | pirates 0124 | |
| inimigo Golem | rpg 0124 | |

## Regras do mapeamento

- Um tile por papel; variantes (`_0/_1`, flip) eu gero sozinho.
- Se um papel não existir no pack, escreva `FALTA` — eu improviso (ex.: espuma procedural, já feita).
- Não precisa mapear o pack inteiro: só as 4 tabelas acima.
- Depois que você responder, eu reescrevo o compose (bitmask real) e te mostro o antes/depois no preview.
