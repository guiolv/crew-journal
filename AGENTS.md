# AGENTS.md — Instruções raiz do projeto

> Este arquivo é carregado automaticamente pelo OpenCode em toda nova sessão.
> Não remover. Regra principal: CONSULTAR O VAULT ANTES DE QUALQUER AÇÃO.

## 1. Protocolo obrigatório de início de sessão

Antes de responder ou executar qualquer tool, SEMPRE:

1. Ler `vault/00-INDEX.md`
2. Ler `vault/01-VISAO-GERAL.md`
3. Ler `vault/02-CONTEXTO-TECNICO.md`
4. Ler `vault/03-DECISOES.md`
5. Ler `vault/04-TAREFAS.md`
6. Ler `vault/06-LOGS-SESSAO.md` (últimas 20 linhas)
7. Se tarefa envolver game design, ler `vault/05-GDD/` inteiro

Não pular. Não presumir contexto. Se algum arquivo faltar, informar e recriar.

## 2. Protocolo de fim de sessão / fim de tarefa

Após concluir trabalho, SEMPRE atualizar:

- `vault/04-TAREFAS.md` — marcar feito / próximo passo
- `vault/03-DECISOES.md` — se houve decisão arquitetural
- `vault/06-LOGS-SESSAO.md` — adicionar entrada com data UTC, resumo, arquivos alterados

## 3. Projeto atual

- Tipo: game em Unity, implementado via Unity CLI
- Vault de memória: `./vault/`
- GDD: será fornecido pelo usuário em `vault/05-GDD/`
- Idioma de comunicação: português (pt-BR)

## 4. Convenções

- Respostas curtas, diretas, técnicas, sem emoji salvo pedido
- Ao referenciar código: `caminho:linha`
- Verificar solução via execução quando possível (build, teste, sanity check)
- Preferir `edit` a criar arquivo novo. Não criar .md fora de `vault/` sem necessidade
- Unity CLI: inspecionar projeto local antes de usar comando genérico
