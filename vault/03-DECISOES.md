# 03 — Decisões

Formato: `## YYYY-MM-DD — Título` + Contexto / Decisão / Consequência.

---

## 2026-09-19 — Vault como memória persistente
- Contexto: necessidade de contexto entre sessões.
- Decisão: usar `./vault/` + `AGENTS.md` com protocolo obrigatório de leitura/escrita. `instructions` do `opencode.jsonc` NÃO é usado (V2 não resolve esse campo).
- Consequência: toda sessão deve ler vault primeiro e atualizar no fim.
