# GAME FEEL AUDIT — crew-journal (2026-09-24)

Skill: `skills/Game Feel` (síntese de Swink). Modo: **AUDITAR**. Entrada de lentes preservada: `vault/08-LENS-REVIEW.md` (tensões T1–T6) + correções já implementadas (`vault/09-DESIGN-FIXES.md`: stance, "em jogo", ferido grave, vínculos).

## Limitação declarada (skill §14)

O jogo **não tem controle direto em tempo real**: viagem por menu + ticks, combate por turnos, UI por cliques. Não há análise de Input contínuo, câmera ou movimento — não se aplica. Eixos usados: Resposta, Contexto, Polish, Metáfora, Regras.

## Contrato de experiência

- **Sensação-alvo:** comando com peso — "sou o capitão"; risco legível, perda com significado.
- **Verbos:** escolher destino / escolher ordem / escolher opção de evento / escolher ação de combate / gerenciar (comprar, recrutar, tratar).
- **Plataforma:** PC, mouse, tela 1280x720, sem áudio, sem háptica.
- **Sucesso:** sessão de ~30 min terminando em "quero continuar". **Falha:** tédio nos vales ou morte lida como injustiça. **Recuperação:** GED §22 (missões simples, ilha segura) + ferido grave antes da morte.

## Mapa de feeling/loops

- **Momento:** clique → rebuild imediato da UI + Juice (fade/flash) → log registra → próxima escolha. Exceção: ticks de viagem (1,1s/dia, só barra de progresso).
- **Encontro:** preview (risco/consumo/em jogo) → viagem → evento com 3 escolhas → chegada/combate por turnos → desfecho no diário.
- **Sessão:** acumular tripulação/navio/reputação rumo a ilhas P4+.
- **Progressão:** XP/nível, navios, módulos, cicatrizes/bandagem visuais.

## Achados por eixo

### Resposta
- **Observado (código):** toda ação de menu reconstrói a UI no mesmo frame + `Juice.Fade/Flash` — aceitação do input é imediata e sinalizada. Combate: `BattleState.Act` resolve ação + inimigos (`AdvanceAuto`) no mesmo clique; o jogador lê o resultado no log tail e nos HPs.
- **Inferido:** múltiplas ações inimigas por clique podem borrar a atribuição causal ("quem me bateu?") em lutas longas — o log tail de 4 linhas é o único rastro.
- **Desconhecido:** latência real percebida no build (nunca medida).

### Contexto
- **Observado:** risco/consumo/"em jogo" no detalhe; pins de ilha posicionados com nome+perigo; retratos com bandagem comunicam estado sem texto.
- **Inferido:** "em jogo" cobre suprimentos e frágeis, mas não o *custo de oportunidade* (o que deixo de fazer viajando).
- **Desconhecido:** sobreposição dos pins de 170px no mapa real; nunca visto em screenshot após MapIsland.

### Polish
- **Observado:** `Juice.cs` tem Fade, Punch, Shake, Flash, mar animado (`Frames` sea_0–3), `FillTo` na barra de viagem.
- **Observado:** **zero áudio** em todo o projeto (grep: nenhum AudioSource/Clip). Todo "impacto" é visual e silencioso.
- **Inferido:** sem som, confirmação de clique e dano dependem 100% de sinais visuais que competem com texto denso — risco de cliques duplos e de dano não percebido.
- **Desconhecido:** se Shake/Punch disparam nos momentos certos (só leitura de código parcial).

### Metáfora
- **Observado:** fantasia GDD = capitão que não pilota o leme. Stance (Cautela/Marcha) devolveu *ordens*, coerente com a metáfora.
- **Inferido:** o vale da viagem (ticks passivos) contradiz "comando": capitão sem nada para comandar vira passageiro da própria barra de progresso.

### Regras
- **Observado:** escada de consequência existe — dano → grave (1 HP, ATK/2, inimigos ignoram) → morte → memorial/diário; fuga 45% explícita; tratamento custa 1 medicina.
- **Inferido:** a escada é boa para atribuição (a 2ª morte é sempre "culpa" de não tratar/não fugir). Ponto cego: medicina também é consumível de eventos — conflito de uso interessante, mas invisível (nenhuma tela mostra essa tensão).

## Mudanças priorizadas (cadeia causal da skill)

1. **Fast-forward da viagem** — Evidência: ticks de 1,1s/dia sem decisão; viagens longas (Storm+Cautela) = espera passiva. Causa: cadência fixa sem agência. Mudança: botão "Apressar" que acelera ticks após o evento resolvido (ou pula para a chegada). Benefício: remove vale sem remover risco (o evento já ocorreu). Tensão/parâmetros: velocidade do tick (1,1s → 0,25s?) vs. perda da antecipação; manter tick normal até o evento. Teste: medir tempo ocioso e se o jogador usa o botão sempre (sinal de que o vale não tem valor) ou às vezes (sinal de antecipação real).
2. **Resolução inimiga legível no combate** — Evidência: vários inimigos agem por clique, rastro só no log. Causa: compressão temporal. Mudança: atraso curto encadeado por ação inimiga (ex.: 0,35s entre atores) + flash no alvo atingido. Benefício: atribuição causal ("o Corsário me matou"). Tensão: ritmo do combate × clareza; parâmetro = delay por ator. Teste (clareza, framework §Teste de clareza): após derrota, pedir "por que você perdeu?" antes de mostrar o log.
3. **Som mínimo como confirmação** — Evidência: zero áudio; cliques e dano só visuais. Causa: canal único saturado. Mudança: 3–5 SFX procedurais (clique, moeda, dano, vitória, derrota) via `AudioClip` gerado em runtime (sem assets). Benefício: confirmação fora da área visual + impacto. Tensão: qualidade vs. custo; manter volume baixo e mutável. Teste: taxa de cliques duplos e relato de "senti o dano" com/sem som.
4. **Conflito da medicina visível** — Evidência: medicina serve a eventos, item de combate e tratamento, mas nenhuma tela mostra o trade-off. Causa: informação distribuída. Mudança: na ficha de ferido grave, linha "usar aqui ou guardar para a próxima viagem". Benefício: transforma gasto em decisão (T4 do review). Tensão: texto extra × clareza. Teste: jogadores hesitam antes de tratar? (hesitação = decisão real).

Não fazer agora: minigame de navegação (trai a fantasia), screen shake pesado (resposta automática proibida pela skill §60 sem percepção definida).

## Menor playtest (refuta a hipótese central)

Hipótese: os vales da viagem não sustentam antecipação. Protocolo: 2 viagens longas, uma com fast-forward liberado e outra sem (ordem alternada). Medir: uso do botão, olhar na tela vs. idle, relato de tensão antes do evento. Se o botão for usado sempre e o relato for igual, o vale não tem valor — cortar ou preencher (ordens mid-voyage). Se houver diferença, calibrar o timing.

## Questões em aberto

- Latência percebida no build real (rubrica §2: medir no hardware, não assumir).
- Sobreposição dos pins no mapa (aguarda screenshot).
- Se delay encadeado no combate irrita em lutas longas (A/B de parâmetro).
- Tudo de `08-LENS-REVIEW.md` §Open questions (balanceamento sem sessão real).
