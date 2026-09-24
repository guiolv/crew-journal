# DESIGN — correções derivadas do Lens Review (2026-09-23)

Modo DESIGN da skill `skills/` aplicado às tensões de `vault/08-LENS-REVIEW.md`.

## Goal

Reduzir o risco #1 (permadeath sem apego/causa legível) e #4 (risco como loteria) e dar agência na viagem, com o menor diff que preserva todo o resto.

## Candidates (por tensão)

**T1 viagem passiva:** (a) ordens de viagem: Cautela/Normal/Marcha; (b) eventos no mapa clicáveis; (c) minigame de navegação. Exame: (b) exige conteúdo novo; (c) trai a fantasia ("jogador não controla o leme", GDD §2). Escolha: **(a)** — 1 campo + 3 botões, mesma fantasia.

**T4 risco-loteria:** (a) linha "em jogo" no detalhe (suprimentos pós-viagem + quem está frágil); (b) mostrar seed/rolls (quebra imersão). Escolha: **(a)** — só UI, zero regra nova.

**T3/T6 morte:** (a) ferido grave (2 etapas); (b) seguro/ressurreição (trai permadeath); (c) só vínculos (não resolve causa distante). Escolha: **(a) + vínculos leves** — (a) dá resposta à má sorte imediata; vínculos dão apego barato.

**T2 farming:** (a) missões com prazo; (b) reputação decai; (c) nada agora (observar no playtest). Escolha: **(c)** — sem evidência de que farming ocorre; não construir contra hipótese.

## Spec escolhida (implementar)

1. **Stance de viagem** `GameData.sailStance` (0 normal, 1 cautela, 2 marcha). Cautela: +1 dia, risco ×0.7, evento −0.1. Marcha: −1 dia (mín 1), risco ×1.3, evento +0.1. Preview e consumo usam valores ajustados.
2. **Linha "em jogo"** na tela de detalhe: suprimentos restantes pós-viagem + 2 tripulantes com menor HP.
3. **Ferido grave** `CharacterData.grave`: golpe letal com `!grave` → HP 1 + flag (1x por vida em combate); novo letal com `grave` → morte. Grave: ATK pela metade; inimigos ignoram graves salvo se todos vivos forem graves. Tratamento na ficha: 1 medicina → limpa + cura 50%. Retrato com bandagem.
4. **Vínculos**: na chegada, 35% de chance de evento de vínculo (par aleatório vivo com relação): afinidade +8..15, confiança +5, linha no diário citando os dois.

## GDD deltas

- Combate/morte: golpe letal → ferido grave (1x); tratamento com medicina.
- Viagem: ordens Cautela/Normal/Marcha com trade-offs acima.
- RNG: navegador + ordens reduzem, nunca eliminam (piso 2% risco / 5% evento).
- Perigos: +Redemoinho (3 escolhas); mares de tempestade (danger≥4 dobra chance de Storm).

## Game-feel (auditoria 10, implementado)

- Fast-forward pós-evento (APRESSAR: tick 1.1s→0.25s); inimigos em passos de 0.35s + Shake + Hit; SFX procedurais + mute; tradeoff da medicina na ficha.
