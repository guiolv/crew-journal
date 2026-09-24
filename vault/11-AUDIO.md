# 11 — Áudio (contrato de arquivos)

Música e SFX por arquivo com fallback procedural. Formato: MP3/OGG/WAV, nomes exatos, em `game/Assets/Resources/Audio/`. Sem arquivo = procedural/silêncio, nada quebra. Toggle único SOM ON/OFF (PlayerPrefs `cj_mute`) vale para tudo.

## Música (loop)

| Arquivo | Toca quando | Status |
|---|---|---|
| `music_map.mp3` | mapa/ilha/navio (tema principal) | OK (Treasure Map) |
| `music_combat.mp3` | combate (volta ao tema ao sair) | FALTA |

## SFX (one-shot)

| Arquivo | Toca quando | Hoje |
|---|---|---|
| `sfx_click` | qualquer botão | procedural |
| `sfx_coin` | dinheiro aumenta (missão, venda, saque) | procedural |
| `sfx_hit` | tripulante atingido no combate | procedural |
| `sfx_victory` | vitória em batalha | procedural |
| `sfx_defeat` | derrota em batalha | procedural |
| `sfx_sail` | início de viagem | NÃO FIADO — trazer p/ fiar |
| `sfx_event` | evento durante a viagem | NÃO FIADO — trazer p/ fiar |
| `sfx_recruit` | novo tripulante | NÃO FIADO — trazer p/ fiar |
| `sfx_death` | morte permanente | NÃO FIADO — trazer p/ fiar |
| `sfx_heal` | tratar ferido grave | NÃO FIADO — trazer p/ fiar |

## Regras

- Nomes exatos, minúsculos, sem espaço. MP3 ou OGG.
- Música: loop de 1–3 min, volume moderado (o jogo toca a 0.35).
- SFX: curtos (<1s, exceto victory/defeat <2s).
- Direitos: só usar áudio com licença (próprio, CC0, ou comprado).
