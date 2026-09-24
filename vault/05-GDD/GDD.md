# GDD — GAME DESIGN DOCUMENT

## Projeto [Nome Provisório]

**Versão:** 0.3 — MVP
**Gênero:** Adventure / Management / RPG
**Perspectiva:** Top-down cartográfica
**Plataforma inicial:** PC
**Modo MVP:** Single-player
**Engine:** Unity

---

# 1. High Concept

Um jogo de aventura e gerenciamento de tripulação inspirado na fantasia de piratas e grandes jornadas marítimas.

O jogador começa sozinho, navegando em um barril.

A partir daí, precisa encontrar recursos, visitar ilhas, recrutar personagens, adquirir um navio e construir uma tripulação capaz de sobreviver a viagens cada vez mais perigosas.

O jogador não controla diretamente a navegação.

Ele escolhe **para onde ir**.

A tripulação determina **como a viagem acontece**.

Um bom navegador pode encontrar uma rota segura.

Um navegador inexperiente pode aumentar o tempo de viagem ou colocar o navio em perigo.

Quando ocorre um confronto, o jogo muda para uma tela de combate inspirada em JRPGs, onde os membros da tripulação lutam individualmente.

Os personagens são únicos, possuem personalidade, atributos, relações e histórias geradas proceduralmente.

**Se morrerem, morrem para sempre.**

A jornada é registrada automaticamente em um **Diário da Tripulação**, transformando os acontecimentos emergentes em uma história única.

---

# 2. Fantasia do jogador

A fantasia central é:

> **"Eu sou o capitão. Minha função é montar uma tripulação capaz de enfrentar o mundo."**

O jogador não precisa controlar cada ação.

Ele precisa:

* escolher destinos;
* avaliar riscos;
* contratar pessoas;
* administrar recursos;
* configurar o navio;
* escolher missões;
* decidir quando lutar ou fugir;
* administrar perdas.

---

# 3. Pilares

## 3.1 Exploração

Descobrir ilhas, rotas, recursos e perigos.

## 3.2 Gerenciamento

Administrar:

* comida;
* água;
* dinheiro;
* carga;
* espaço;
* tripulação;
* funções;
* manutenção.

## 3.3 Personagens

Cada tripulante deve parecer uma pessoa, não apenas uma estatística.

## 3.4 Risco

Toda viagem pode gerar consequências.

## 3.5 História emergente

O jogo cria acontecimentos que formam uma história única.

---

# 4. Loop principal

```text
ESCOLHER DESTINO
       ↓
NAVEGAR
       ↓
EVENTO / PERIGO
       ↓
CHEGAR À ILHA
       ↓
EXPLORAR
       ↓
MISSÃO / COMÉRCIO / RECRUTAMENTO
       ↓
DECISÃO
       ↓
RECOMPENSA / CONSEQUÊNCIA
       ↓
GERENCIAR TRIPULAÇÃO
       ↓
MELHORAR NAVIO
       ↓
ESCOLHER NOVO DESTINO
```

---

# 5. MVP

O MVP deve provar o loop principal.

## Conteúdo

### Ilhas

5:

1. Vila de Pescadores
2. Porto Comercial
3. Capital
4. Ilha Inabitada
5. Ilha Perigosa

### Personagens

10–15 personagens gerados proceduralmente.

### Navios

3 níveis:

1. Barril
2. Pequeno Barco
3. Navio Médio

### Recursos

* dinheiro;
* comida;
* água;
* madeira;
* metal;
* medicina.

### Missões

* transporte;
* comércio;
* resgate;
* combate;
* exploração.

### Eventos

Aproximadamente 10.

### Inimigos

5 tipos.

### Combate

Turn-based.

### Diário

Completo.

### Morte permanente

Ativa.

### Reputação

Local por ilha.

### Notoriedade

Simplificada.

---

# 6. Início da jornada

O jogador começa:

* sozinho;
* em um barril;
* com poucos recursos;
* sem reputação;
* sem tripulação.

Primeiro objetivo implícito:

> Encontrar uma ilha.

O jogo não precisa apresentar uma longa introdução narrativa.

A própria jornada deve ensinar o jogador.

---

# 7. Navegação

A interface principal é um mapa top-down com aparência cartográfica.

O jogador seleciona uma ilha.

O navio navega automaticamente.

A viagem é calculada pelo sistema.

Após o evento da viagem resolvido, o jogador pode apressar o restante (velas a todo pano) — o risco já ocorreu, resta só a espera.

---

# 8. Fatores da navegação

O resultado depende de:

* navegador;
* experiência;
* clima;
* região;
* navio;
* distância;
* rota.

O jogador não controla o leme.

Isso transforma o navegador em um personagem estrategicamente importante.

---

# 9. Sistema de navegação

Resultado de uma viagem:

```text
Tempo
Risco
Consumo
Eventos
```

Um navegador melhor:

* reduz tempo;
* reduz risco;
* detecta perigos;
* encontra rotas melhores.

Nunca elimina completamente o risco.

---

# 10. Eventos marítimos

Exemplos:

### Navio abandonado

* ignorar;
* investigar;
* saquear;
* resgatar.

### Tempestade

* atravessar;
* desviar;
* esperar.

### Navio desconhecido

* aproximar;
* fugir;
* observar;
* atacar.

### Criatura marinha

* fugir;
* lutar;
* evitar.

### Redemoinho

* contornar (navegador ≥60 passa ileso, +XP);
* atravessar a borda (rápido, dano ao casco, perde comida);
* esperar dissipar (+1 dia, consome suprimentos).

### Mares de tempestade

Destinos perigosos (perigo ≥4) têm o dobro de chance de clima Tempestade na viagem.

### Regra do RNG (navegação)

Navegador experiente + ordens do capitão (Cautela/Normal/Marcha) REDUZEM risco e chance de evento, nunca eliminam: piso de 2% de risco e 5% de evento por viagem.

---

# 11. Ilhas

Cada ilha possui um arquétipo.

## Vila de Pescadores

Foco:

* comida;
* pesca;
* pequenas missões;
* recrutamento.

## Porto Comercial

Foco:

* comércio;
* carga;
* preços;
* mercadores.

## Capital

Foco:

* grande quantidade de NPCs;
* missões;
* facções;
* comércio;
* recrutamento.

## Ilha Inabitada

Foco:

* exploração;
* recursos;
* descobertas.

## Ilha Perigosa

Foco:

* combate;
* recursos raros;
* alto risco.

---

# 12. Geração procedural

No MVP, as ilhas terão arquétipos fixos, mas seus detalhes serão proceduralmente gerados.

Podem variar:

* nome;
* tamanho;
* recursos;
* população;
* preços;
* NPCs;
* missões;
* eventos;
* história.

A geração será determinística através de uma seed.

---

# 13. Tripulação

Os personagens são indivíduos.

Cada personagem possui:

* nome;
* idade;
* aparência;
* profissão;
* atributos;
* personalidade;
* traits;
* experiência;
* história;
* relações;
* moral;
* lealdade.

---

# 14. Funções

MVP:

* Navegador
* Cozinheiro
* Médico
* Carpinteiro
* Combatente
* Atirador

Um personagem pode possuir mais de uma competência.

---

# 15. Experiência

Experiência é específica por função.

Exemplo:

```text
Carlos

Navegação: 78
Combate: 32
Culinária: 5
```

Isso permite evolução natural.

---

# 16. Personalidade

Traits podem incluir:

* corajoso;
* covarde;
* ambicioso;
* generoso;
* leal;
* impulsivo;
* inteligente;
* preguiçoso;
* desconfiado.

Traits afetam gameplay e narrativa.

---

# 17. Relações

Personagens podem desenvolver:

* amizade;
* rivalidade;
* confiança;
* medo;
* lealdade.

No MVP, relações terão impacto principalmente em:

* moral;
* eventos;
* pequenas modificações de desempenho.

---

# 18. Morte permanente

O primeiro golpe letal em combate deixa o tripulante em **estado grave** (HP 1, ATK pela metade, bandagem no retrato) em vez de matar. Um novo golpe letal enquanto grave **mata**. Feridos graves são ignorados pelos inimigos salvo se forem os únicos de pé. Tratamento: 1 medicina na ficha do personagem (cura até 50% do HP).

Quando um personagem morre:

**ele não retorna.**

Sua morte:

* é registrada;
* afeta a tripulação;
* pode afetar relacionamentos;
* aparece no diário;
* aparece no memorial.

---

# 19. Combate

O combate muda para uma tela separada.

Estilo:

**JRPG por turnos.**

Cada personagem possui:

* HP;
* ataque;
* defesa;
* velocidade;
* habilidades;
* função.

Ações:

* atacar;
* defender;
* habilidade;
* item;
* fugir.

---

# 20. Consequência do combate

Vitória pode gerar:

* dinheiro;
* recursos;
* experiência;
* reputação.

Derrota pode gerar:

* perda de recursos;
* dano ao navio;
* ferimentos;
* morte;
* perda da missão.

---

# 21. Navio

## Barril

Capacidade mínima.

Serve apenas para iniciar a jornada.

## Pequeno Barco

Primeiro navio real.

Permite pequena tripulação e carga.

## Navio Médio

Permite:

* mais tripulantes;
* mais carga;
* mais módulos;
* maior autonomia.

---

# 22. Espaço

O navio possui slots.

O jogador decide como utilizá-los.

Exemplo:

```text
10 slots

Cozinha: 2
Dormitório: 2
Carga: 3
Equipamento: 2
Especial: 1
```

Essa configuração influencia a capacidade da tripulação.

---

# 23. Economia

Fontes:

* missões;
* comércio;
* exploração;
* saque;
* recompensas.

Despesas:

* comida;
* água;
* manutenção;
* recrutamento;
* equipamentos;
* reparos.

---

# 24. Reputação

Cada ilha possui reputação independente.

Exemplo:

```text
Ilha A: +50
Ilha B: -20
Ilha C: 0
```

Reputação influencia:

* preços;
* missões;
* recrutamento;
* comportamento de NPCs.

---

# 25. Notoriedade

Representa o quanto o mundo conhece a tripulação.

No MVP possui apenas efeitos simples:

* desbloqueia alguns eventos;
* influencia encontros;
* aparece no perfil da tripulação.

---

# 26. Diário da Tripulação

O diário registra automaticamente acontecimentos importantes.

Exemplos:

* recrutamento;
* morte;
* primeira visita;
* batalha;
* missão;
* descoberta;
* aquisição de navio;
* grande decisão.

---

# 27. Memorial

Todos os mortos permanecem registrados.

Exemplo:

> **Carlos "Mão de Ferro"**
>
> Entrou: Dia 17
> Morreu: Dia 84
> Função: Carpinteiro
> Batalhas: 12
> Inimigos derrotados: 23
> Causa: Ferimentos em combate

---

# 28. Direção visual

Estética:

**cartunesca + aventureira + fantástica + levemente spooky + engraçada.**

Referência conceitual:

* anime de aventura;
* mapas cartográficos;
* fantasia;
* piratas;
* personagens exagerados;
* ilhas visualmente distintas.

Evitar realismo excessivo.

---

# 29. Multiplayer — Pós-MVP

Não implementado inicialmente.

Planejado:

* encontros assíncronos;
* tripulações de outros jogadores;
* rankings;
* comércio;
* batalhas;
* histórico de encontros.

O MVP deve provar o jogo single-player primeiro.

---

# 30. Critério de sucesso

O jogador deve terminar uma sessão de aproximadamente 30 minutos pensando:

> **"Quero continuar navegando para descobrir o que vai acontecer."**

Esse é o principal indicador de sucesso do MVP.

---

# 31. Visual procedural modular (v0.3)

A IA **não** roda dentro do jogo. Ela produz a biblioteca; o Unity monta via seed.

## 31.1 Princípio

> **Regras + arquétipo + seed — nunca randomização pura.**

Todo visual segue o mesmo estilo: mesma perspectiva, iluminação, proporção, paleta, traço e pontos de encaixe.

## 31.2 Ilhas

Biblioteca: terrenos (areia, grama, floresta, montanha, penhasco, vulcão, pântano, rocha, praia), construções (casa, taverna, mercado, porto, igreja, prisão, torre, armazém, moinho), elementos (árvores, pedras, barcos, docas, estradas, plantações, pontes, ruínas, tesouros).

Geração em 3 etapas: **forma** (máscara + erosão → praias/falésias/montanhas/rios) → **bioma** (temperatura+umidade+altitude) → **personalidade** (gramática do arquétipo: vila pesqueira = porto→mercado→casas→plantações→floresta; capital = castelo→praça→mercado/igreja/governo→porto→subúrbio; perigosa = praia→floresta→ruínas→caverna→boss).

## 31.3 Personagens

Camadas: corpo, rosto, cabelo, barba, roupa (por profissão), acessórios, arma — montadas no mesmo rig. DNA procedural (seed → sempre o mesmo visual; save/load preserva). Distribuição por profissão (médico tende a jaleco/óculos; pirata a bandana/cicatriz) sem regra absoluta. O visual evolui com a história: cicatriz após batalha, roupa acompanha função.

## 31.4 Navios

Casco + mastro + vela + canhões + decoração + figura de proa, combinados por seed/definição.

## 31.5 Biblioteca inicial do MVP

Ilhas: 5 arquétipos, ~30 terrenos, ~40 construções, ~50 props. Personagens: 8 corpos, 12 rostos, 15 cabelos, 15 roupas, 20 acessórios, 10 armas. Milhares de combinações; a biblioteca cresce sem alterar o sistema.
