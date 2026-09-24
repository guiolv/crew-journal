# 04 — Tarefas

## Em progresso
- [x] Áudio (sessão atual): `Treasure Map` como `music_map` em loop + `Music.cs` (mapa/combate) + override de SFX por arquivo; 15/15, build + smoke limpos
- [x] Assets das sheets no jogo: 37 PNGs fatiados + ArtPostprocessor + topbar com chips; build 00:52 empacotado; aguardando confirmação visual/auditiva do usuário
- [ ] Mapa com ilhas desenhadas + nome abaixo (sessão atual): MapIsland no lugar do MapPin — build em curso, falta screenshot do usuário
- [ ] Playtest manual do loop completo no `Build/crew-journal.exe` (usuário)
- [x] DESIGN fixes (skill): stance, em-jogo, ferido grave, vínculos — lógica 36/36, type-check 0, build 22:22, playtest 13/13, smoke limpo
- [x] Game-feel audit itens 1-4: fast-forward, inimigos ritmados (StepEnemy), SFX procedurais + mute, tradeoff da medicina — lógica 38/38, type-check 0, build 23:08, playtest 14/14 (sfx-smoke), smoke limpo
- [ ] Gerador local de sprites (IA): scaffold pronto (`tools/sprite-gen/` + `IPartSource`); falta instalar Python + ComfyUI e gerar os 49 PNGs
- [ ] Biblioteca IA completa (72 PNGs: 49 sprites + 23 UI) — gerador estendido, `UIStyle.cs` fiado no `GameUI` com fallback; downloads ComfyUI AMD + SD1.5 + LoRA em andamento, geração a seguir
- [x] 2026-09-23 — Mapa isométrico 2:1 (`Iso.cs` + `IslandRenderer` diamante com penhascos + pins via `Iso.Pin`); lógica x,y intacta; csc 0 erros

## Backlog
- [ ] 6 eventos marítimos → ~10 (GDD fala ~10; hoje: tempestade, abandonado, desconhecido, criatura, redemoinho, calmaria)
- [ ] Habilidades de combate por função (hoje só golpe pesado genérico)
- [ ] História de personagem em 2-3 linhas (hoje 1 linha via BioFor)

## Backlog
- [ ] Trocar placeholders por arte final; eventos com escolhas (GDD secao 10)
- [ ] Escolhas de evento de viagem (hoje auto-resolvido com log)

## Feito
- [x] 2026-09-19 — Criar vault + configurar OpenCode (AGENTS.md + opencode.jsonc)
- [x] 2026-09-20 — Scaffold v0.1 crew-journal: 10 arquivos Logic + GameManager/GameUI/PlaceholderArt + V1Bootstrap/V1Build; testes 12/12; type-check Unity OK; commit f9081f4
- [x] 2026-09-20 — Unity reparado (reinstall limpo, UPM v9.31.1) + repo https://github.com/guiolv/crew-journal
- [x] 2026-09-20 — v0.1 jogável: manifest uGUI 2.6.0, cena World via CLI, build Windows OK, smoke test sem exceptions
- [x] 2026-09-20 — v0.2 refs: Voyage em etapas + clima, eventos com 3 escolhas, BattleState por turnos, módulos, XP/nível, tema navy+pergaminho; lógica 24/24, playtest engine 13/13, build + smoke limpos
- [x] 2026-09-20 — UI v3 jogável: scroll + layout groups, mapa posicionado, botões grandes (push 59db8d3)
- [x] 2026-09-20 — v0.3 visual procedural: DNA + gramáticas + 3 renderers + integração UI + GDD-31/TDD-36; lógica 30/30, playtest 13/13, arte verificada a olho via PNG
