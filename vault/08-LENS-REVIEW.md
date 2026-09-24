# Lens Review — crew-journal (2026-09-23)

Skill: `skills/` (lens-based, síntese de Schell). Modo: REVIEW. Alvo: GDD v0.3 + build v0.3/UI v3.

## Design problem

O MVP prova o loop (tudo implementado e testado tecnicamente), mas há **zero evidência de jogador**: o playtest manual está pendente. A pergunta a decidir: o build atual produz a fantasia do GDD ("Eu sou o capitão") ou só executa o loop mecanicamente?

## Intended experience

- Sentir-se capitão: escolher destinos, avaliar riscos, contratar, gerenciar, decidir lutar/fugir, administrar perdas.
- Sessão de ~30 min terminando em "quero continuar navegando".
- Tripulantes como pessoas; morte permanente como história, não como game over vazio.

## Relevant perspectives

Selecionadas por aplicabilidade (skill §3 — conjunto pequeno, não todas):
- **A. Experience** (intenção vs. experiência real)
- **C. Mechanics** (o loop gera decisões de capitão?)
- **D. Balance** (escolhas significativas, risco, punição)
- **F. Interface/Feedback** (risco legível? consequência previsível?)
- **G. Pacing** (viagem, eventos, curva de 30 min)
- **H. Story/Character** (pessoas, não estatísticas)

Fora de escopo agora: multiplayer (pós-MVP por definição), business, production (exceto nota no fim).

## Analysis

**A. Experience.** Evidência: nenhuma de jogador — só smoke tests e asserts. Assunção atual: completar o loop = sentir-se capitão. Tensão: o jogo *executa* as 9 ações do capitão (GDD §2), mas "avaliar riscos" acontece num único ponto (tela de detalhe: dias/risco%/consumo) e a viagem vira barra de progresso + 1 evento. Risco real: sensação de passageiro, não de comandante. Alternativa: ordens de viagem (forçar marcha / racionar / cautela) para dar agência *durante* a navegação. Teste: observar se o jogador olha a tela durante a viagem ou larga o mouse.

**C. Mechanics.** O que o sistema recompensa: navegar com bom navegador (menor tempo/risco), manter suprimentos, aceitar missões de combate com party forte. Comportamento dominante provável: rota segura repetida entre 2 ilhas baratas (farming de baixo risco) — o GED prevê salários/consumo como freio, mas nada *exige* progressão para ilhas perigosas além de recompensa maior. Assunção: "recompensa maior atrai". Pode falhar se o jogador for avesso a perda (permadeath amplifica aversão). Alternativa: missões com prazo ou reputação que decai, puxando o jogador para fora da zona segura. Teste: em 30 min, quantas ilhas distintas o jogador visita sem ser induzido?

**D. Balance.** Pontos fortes: custo de viagem visível antes de partir; fuga 45% explícita; preços com piso. Riscos: (1) combate mira o mais fraco e morte é permanente — perder um veterano por RNG pode ler como injustiça, não consequência (a escolha que causou a morte aconteceu minutos antes, na decisão de viajar/lutar — distância temporal entre causa e efeito enfraquece o aprendizado); (2) 5 eventos para ~10 prometidos reduz variedade percebida em sessões longas. Alternativa: "ferido grave" como estado intermediário antes da morte (mantém stakes, dá chance de resposta). Teste: após primeira morte, o jogador continua ou fecha o jogo?

**F. Interface/Feedback.** UI v3 reescrita após screenshot "injogável" (tudo colapsado); agora há scroll, mapa posicionado, botões grandes. Risco restante: densidade sem hierarquia de decisão — a tela de detalhe mostra risco% mas não *o que está em jogo* (quem pode morrer? o que perco?). Transparência fraca aqui quebra o pilar Risco: consequência imprevisível vira loteria. Alternativa: linha "em jogo" no detalhe (ex.: suprimentos após viagem, membros abaixo de X HP). Teste: antes de clicar INICIAR, pedir ao jogador para dizer em voz alta o que espera que aconteça.

**G. Pacing.** Viagem = ticks de ~1,1s + 1 evento; picos (evento/combate) existem, mas vales são espera passiva. Curva de 30 min não medida. Assunção: 1 evento/viagem sustenta interesse. Pode falhar em viagens longas (barril lento). Alternativa: escalar ticks com distância e permitir acelerar animação. Teste: medir tempo ocioso vs. tempo decidindo na sessão.

**H. Story/Character.** Pontos fortes: permadeath + memorial + diário automático + cicatriz visual. Gap conhecido: BioFor de 1 linha e relações só afinidade/confiança — "parecer pessoa" depende hoje de nome + retrato + traits. Tensão: o diário registra *fatos*, mas história emergente precisa de *contraste* (amizade→morte dói; estatística→morte irrita). Sem evidência de apego, permadeath pode punir sem pagar. Alternativa: 2-3 eventos de vínculo (ex.: dois tripulantes sobrevivem juntos → traço de relação visível) antes de cobrar mortes. Teste: ao fim da sessão, pedir nomes de tripulantes de cabeça — quantos lembra?

## Tensions and risks (resumo)

1. Agência do capitão vs. viagem automática (A, C).
2. Farming seguro vs. progressão (C).
3. Morte significativa vs. morte por RNG distante da decisão (D, H).
4. Risco legível vs. loteria (F).
5. Ritmo com vales passivos (G).
6. Apego antes da perda (H).

Maior risco: **#3 + #6 combinados** — permadeath sem apego e sem causa legível = frustração, que derruba o critério de sucesso ("quero continuar").

## Alternatives (direções distintas)

- **A — Capitão ativo:** ordens de viagem + linha "em jogo" no detalhe. Custo: 2 telas e 3 parâmetros. Preserva todo o resto.
- **B — Morte em duas etapas:** ferido grave → morte só se não tratado (medicina/médico a bordo ganha função fora do combate). Reduz frustração, dá papel ao médico.
- **C — Vínculos primeiro:** eventos de relação antes de escalar perigo; diário cita relações ("Mariana salvou João"). Barato em código (usa relations existentes), caro em escrita.

## Validation (menor teste útil)

Playtest de 30 min com protocolo por perguntas (serve o playtest pendente de `04-TAREFAS.md`):
1. O que você espera que aconteça? (antes de INICIAR — transparência)
2. Para onde, e por quê? (agência + farming?)
3. O que você está olhando durante a viagem? (pacing)
4. Após 1ª morte: continuar ou parar? Por quê? (punição vs. história)
5. Cite tripulantes de cabeça. (apego)
6. Quando quis parar, o que te fez continuar? (critério de sucesso)

Registrar: hesitações, cliques aleatórios em eventos, ponto de saída. Não perguntar "gostou?".

## Open questions

- Números do GED (preços, salários, risco%) sem nenhuma sessão real — todo balanceamento é hipótese.
- Duração real até "quero continuar" vs. até tédio.
- Se permadeath deve ter rede de segurança além das do GED §22.
- Multiplayer assíncrono (pós-MVP) pode invalidar tuning atual — não decidir agora.

## Production note

`tools/sprite-gen/` + `IPartSource` desacoplam arte de sistema: qualquer mudança acima não exige retrabalho visual. Priorizar mecânica antes de gerar os 72 PNGs finais.
