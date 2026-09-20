# 07 — Referências visuais (lidas 2026-09-20)

Dois mockups "Sail & Survive" — identidade: navy profundo (#0A1628) + pergaminho (#E8D5A3) + teal (#1B4D5C) + dourado. Fonte serifada nos títulos.

## Telas exigidas (inventário)
1. Mapa principal (HUD): capitão, recursos, reputação, destino selecionado + tempo estimado
2. Navegação (detalhes): destino, navegador + XP, clima, resultado estimado (tempo/risco/consumo/eventos), botão iniciar
3. Evento durante viagem: card pergaminho + 3 escolhas com trade-off explícito
4. Ilha: menu (comércio/missões/recrutamento/info/sair) + painel mercadorias com mults + missões
5. Ficha de personagem: nível/XP, atributos, XP por função, personalidade, relações, história
6. Navio: HP, trip máx/carga/velocidade, módulos (cozinha/dormitório/carga/equip/especial), inventário
7. Combate por turnos: ordem de turnos, inimigos com HP, ações (atacar/defender/habilidade/item/fugir)
8. Diário (tabs: todos/viagens/missões/combate/recrutamento/mortes/descobertas) + Memorial + Missões (objetivos, recompensa, risco, aceitar)

## Gap v0.1 → refs (virou escopo v0.2, esta sessão)
- [x] Preview de viagem + clima determinístico
- [x] Eventos com 3 escolhas (GDD 10) em vez de auto-resolução
- [x] Combate interativo por turnos (ações por ator) em vez de simulação em bloco
- [x] Módulos do navio compráveis com efeito (GDD 22)
- [x] XP/nível por função (GED 15-16)
- [x] Tema navy+pergaminho na UI
- Ficha/relações/história: parcial (gerador já dá traits/jobs; relações só afinidade/confiança; história = 1 linha gerada)

## v0.3 visual procedural (sessão atual)
- [x] DNA personagem/ilhas/navios (VisualDNA.cs) + testes
- [x] Renderers pixel-art estilo único (Portrait/Island/Ship + Pixel.cs)
- [x] Retratos em recrutas/equipe/combate/ficha; ilha nas telas ilha/detalhe; navio na tela navio
- [x] Cicatriz adquirida em batalha (visual evolui com história)
- [x] Arte verificada a olho via export PNG (preview/ + big_): retratos legíveis, estilo consistente
- [ ] Biblioteca externa via PNG (Resources + fallback procedural) — contrato pronto, assets pendentes
