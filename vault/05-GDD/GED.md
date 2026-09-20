# GED — GAME ECONOMY & PROGRESSION DESIGN

## Projeto [Nome Provisório]

**Versão:** 0.3 — MVP

---

# 1. Objetivo

O sistema econômico deve criar decisões.

O jogador nunca deve ter recursos suficientes para fazer tudo simultaneamente.

Ele deve escolher entre:

* contratar;
* melhorar;
* comprar;
* reparar;
* viajar;
* guardar;
* arriscar.

---

# 2. Moedas e recursos

## Dinheiro

Principal moeda.

Usado para:

* contratar;
* comprar;
* reparar;
* negociar;
* melhorar.

---

## Comida

Consumida diariamente pela tripulação.

---

## Água

Consumida diariamente.

---

## Madeira

Usada para:

* reparos;
* melhorias.

---

## Metal

Usado para:

* melhorias;
* equipamentos;
* armas.

---

## Medicina

Usada para:

* recuperação;
* eventos;
* combate.

---

# 3. Recursos iniciais

O jogador começa com:

```text
Dinheiro: 100
Comida: 8
Água: 8
Madeira: 2
Metal: 0
Medicina: 1
```

O objetivo é que isso seja suficiente para aproximadamente:

**1–2 viagens curtas.**

---

# 4. Consumo

Cada tripulante consome:

```text
Food: 1 / dia
Water: 1 / dia
```

No início do jogo, como o jogador está sozinho:

```text
1 Food / dia
1 Water / dia
```

Isso cresce conforme a tripulação aumenta.

---

# 5. Barril

O barril não possui capacidade real de tripulação.

Capacidade:

```text
Crew: 1
Cargo: 2
```

Não possui módulos.

É uma ferramenta de início de jogo.

---

# 6. Pequeno Barco

```text
Crew: 4
Cargo: 8
Modules: 4
Hull: 50
Speed: 1.0
```

Preço alvo:

```text
500
```

O jogador deve precisar realizar algumas atividades antes de comprá-lo.

---

# 7. Navio Médio

```text
Crew: 10
Cargo: 20
Modules: 10
Hull: 120
Speed: 1.15
```

Preço alvo:

```text
2.500
```

Esse navio representa o início da verdadeira gestão de tripulação.

---

# 8. Recrutamento

Custo inicial:

```text
50–250
```

O custo depende de:

* habilidade;
* experiência;
* raridade;
* reputação;
* personalidade.

Personagens muito fortes devem ser mais caros.

---

# 9. Salário

No MVP, salários podem ser simplificados.

Cada personagem gera:

```text
5–20 moedas / dia
```

dependendo da experiência.

Isso cria um custo contínuo para manter uma tripulação grande.

---

# 10. Comida e água

Preços base:

```text
Food: 5
Water: 4
Medicine: 20
Wood: 15
Metal: 25
```

Cada ilha modifica esses preços.

---

# 11. Economia de ilhas

Multiplicadores:

```text
0.6x — muito barato
0.8x — barato
1.0x — normal
1.3x — caro
1.8x — muito caro
```

Exemplo:

Vila de Pescadores:

```text
Food = 0.6x
Water = 0.8x
Wood = 1.1x
Metal = 1.4x
```

Capital:

```text
Food = 1.0x
Water = 1.0x
Wood = 1.0x
Metal = 0.9x
```

---

# 12. Missões

As missões devem fornecer recursos, mas não substituir completamente o comércio.

## Transporte

Recompensa:

```text
100–250
```

## Comércio

Recompensa:

```text
50–300
```

dependendo da margem.

## Resgate

Recompensa:

```text
100–400
```

## Combate

Recompensa:

```text
100–500
```

* possíveis itens.

## Exploração

Recompensa variável.

Pode gerar:

* recursos;
* itens;
* mapas;
* dinheiro;
* reputação.

---

# 13. Risco x recompensa

Missões possuem três categorias.

### Baixo risco

Recompensa pequena.

### Médio risco

Recompensa relevante.

### Alto risco

Recompensa grande.

O jogador deve ser capaz de sobreviver fazendo apenas conteúdo de baixo risco, mas o crescimento será mais lento.

---

# 14. Progressão

A progressão inicial:

```text
BARRIL
 ↓
PRIMEIRA ILHA
 ↓
PRIMEIRO RECRUTAMENTO
 ↓
PRIMEIRA MISSÃO
 ↓
PEQUENO BARCO
 ↓
MAIS TRIPULANTES
 ↓
REGIÕES MAIS PERIGOSAS
 ↓
NAVIO MÉDIO
```

---

# 15. Progressão de personagens

Personagens ganham XP por:

* combate;
* viagens;
* missões;
* utilização da função.

Ao subir de nível:

* atributos podem melhorar;
* habilidades podem desbloquear;
* eficiência profissional aumenta.

---

# 16. Experiência de função

Exemplo:

Um navegador participa de uma viagem.

Recebe:

```text
+10 Navigation XP
```

Se encontrar uma tempestade:

```text
+20 Navigation XP
```

Se evitar um perigo:

```text
+30 Navigation XP
```

Isso recompensa o uso do personagem em sua função.

---

# 17. Progressão de reputação

Escala:

```text
-100 ←→ +100
```

Categorias:

```text
-100 a -60 = Hostil
-59 a -20 = Desconfiado
-19 a +19 = Neutro
+20 a +59 = Amigável
+60 a +100 = Admirado
```

Esses nomes são apenas estados sistêmicos, não uma avaliação moral do jogador.

---

# 18. Efeitos de reputação

### Hostil

* preços maiores;
* menos missões;
* maior chance de conflito.

### Desconfiado

* poucas oportunidades;
* preços normais ou elevados.

### Neutro

Estado padrão.

### Amigável

* melhores preços;
* missões adicionais.

### Admirado

* descontos;
* recrutamento especial;
* informações;
* missões exclusivas.

---

# 19. Notoriedade

Escala aberta.

Exemplo inicial:

```text
0–99
Desconhecido

100–499
Conhecido localmente

500–1.999
Conhecido regionalmente

2.000+
Nome conhecido
```

No MVP, notoriedade não precisa possuir dezenas de efeitos.

Ela deve principalmente:

* desbloquear eventos;
* aparecer no perfil;
* servir como base para o futuro ranking.

---

# 20. Death Economy

A morte deve possuir custo real.

Quando um personagem morre:

* perde-se investimento em XP;
* perde-se capacidade profissional;
* equipamentos podem ser perdidos;
* substituição custa dinheiro;
* funções podem ficar indisponíveis.

Mas a morte não deve ser tão frequente que impeça progressão.

---

# 21. Reparos

Navio danificado exige:

```text
Wood
Money
```

Fórmula inicial:

```text
RepairCost =
MissingHull × BaseRepairCost
```

---

# 22. Limites de segurança econômica

O jogador nunca deve ficar permanentemente sem possibilidade de recuperação.

Mesmo após perder quase tudo, deve existir pelo menos uma alternativa:

* missão simples;
* recurso barato;
* ilha segura;
* trabalho de baixo risco.

O jogo deve permitir **recuperação**, não apenas punição.

---

# 23. Progressão de risco

Região inicial:

```text
Danger 1–2
```

Região intermediária:

```text
Danger 3–5
```

Região avançada:

```text
Danger 6–10
```

O MVP trabalha apenas com:

```text
Danger 1–5
```

---

# 24. Filosofia de balanceamento

Nunca balancear apenas pelo ganho de dinheiro.

Uma atividade deve ser avaliada por:

```text
Dinheiro
+
Experiência
+
Recursos
+
Reputação
+
Risco
+
Tempo
```

Uma missão que paga pouco pode ser boa se for rápida e segura.

Uma missão que paga muito pode ser ruim se colocar uma tripulação valiosa em risco.

---

# 25. Objetivo econômico do MVP

Ao final de uma sessão de 30 minutos, um jogador que tomou boas decisões deve estar aproximadamente em:

```text
1 Pequeno Barco
2–4 tripulantes
500–1.500 moedas
Algumas melhorias
Alguma reputação
Alguns eventos registrados
```

Isso é uma referência inicial, não um valor definitivo.

---

# 26. Balanceamento iterativo

Os valores do GED devem ser tratados como parâmetros.

Nada deve ser considerado definitivo antes de testes.

Criar ferramentas internas para alterar:

* preços;
* recompensas;
* consumo;
* risco;
* XP;
* custo de navios;
* custo de recrutamento.

Sem necessidade de alterar código.

---

# 27. Meta do sistema econômico

O jogador deve frequentemente pensar:

> "Eu tenho dinheiro para isso, mas será que deveria gastar?"

E:

> "Posso seguir para uma ilha mais perigosa e ganhar muito mais, mas minha tripulação está preparada?"

Essa tensão deve ser constante.

---

# 28. Regra final

O dinheiro não deve ser o objetivo final.

Ele é uma ferramenta para:

**manter a tripulação viva → melhorar o navio → alcançar lugares mais perigosos → criar histórias maiores.**
