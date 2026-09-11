# Review: T-09 — Implementar a Razor Page com formulário, resultado e mensagens de erro

> **Plano de referência:** `docs/plans/PLAN-001-encurtador-url.md`
> **PRD de referência:** `docs/prds/PRD-001-encurtador-url.md`
> **Arquitetura de referência:** `docs/architecture/proposta-arquitetural.md`
> **Reviewer:** Claude (skill `reviewer-leanwork` v1.0)
> **Data:** 2026-08-14
> **Round:** 2
> **Recomendação final:** ✅ Aprovado

---

## Sumário executivo

Round 2 confirma o fechamento do único finding `Importante` do round 1 (R-01) e do ponto de validação humana da seção 9 do plano, que estava em aberto. O código de produção não mudou desde o round 1 — a diferença deste round é inteiramente em rastreabilidade: a nota retroativa na linha de T-02 já estava registrada na seção 11 do plano, o comentário em `EncurtadorDbContext.cs` já cita o risco de T-02 explicitamente, e a validação manual em navegador (formulário, mensagem de erro, botão de cópia) foi confirmada pelo autor em 2026-08-14. Os dois checkboxes que ainda estavam pendentes (seção 9 "Após T-09"; critério de aceite "controle de cópia" na própria tarefa) foram marcados nesta sessão para refletir o estado real. `dotnet test` reexecutado: 29/29 verdes.

**Findings por severidade:**

| Severidade | Quantidade |
|------------|------------|
| Bloqueante | 0 |
| Importante | 0 |
| Sugestão   | 0 |
| **Total**  | **0** |

**Cobertura da tarefa:**

| Item | Esperado | Entregue | Status |
|------|----------|----------|--------|
| Regras implementadas (RN) | RN-14, RN-15 | RN-14, RN-15 | ✅ |
| Cenários validados (CA) | CA-01, CA-11 | CA-01, CA-11 | ✅ |
| Decisões base (ADR) | ADR-001 | ADR-001 respeitada | ✅ |
| Critérios de aceite da tarefa | 3 | 3 atendidos | ✅ |
| Testes prometidos | 2 | 2 | ✅ |
| Ponto de validação humana (seção 9) | 1 | 1 confirmado | ✅ |

---

## Contexto da implementação

### Stack detectada

- Backend: .NET 10 / ASP.NET Core, Razor Pages
- ORM/Banco: EF Core 10 + SQLite
- Testes: xUnit + `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`)
- **Fonte:** `CLAUDE.md` do projeto (seção Stack) e proposta arquitetural §6.2 — inalterada desde o round 1

### Escopo do diff (vs. round 1)

Nenhum arquivo de produção ou teste mudou desde o round 1. Mudanças nesta sessão são só de documentação:

- `docs/plans/PLAN-001-encurtador-url.md` — checkbox da seção 9 marcado, linha de histórico de T-09 e critério de aceite de cópia atualizados para remover a menção a "pendente"
- **Commits:** `2da7b44` (T-08) e `2d63387` (T-09)

---

## Round anterior (Round 1)

| Item anterior | Status | Comentário |
|---------------|--------|------------|
| R-01 (round 1, Importante) — Correção do round-trip de `DateTimeKind` toca arquivo fora do escopo declarado de T-09 | ✅ Resolvido | Nota retroativa já presente na linha de T-02 na seção 11 do plano (`docs/plans/PLAN-001-encurtador-url.md:548`), citando T-09 e R-01 explicitamente. O comentário em `EncurtadorDbContext.cs:21-22` também referencia o risco registrado em T-02. Resta apenas citar `T-09` na mensagem do commit quando o diff for de fato commitado — ver R-01 deste round (rebaixado a Sugestão, é passo de processo, não de código) |
| R-02 (round 1, Sugestão) — BOM UTF-8 removido de `Index.cshtml` | ✅ Sem ação necessária | Mantido como estava — impacto nulo, uniformiza com os demais `.cshtml` do projeto, conforme já registrado no round 1 |
| Ponto de validação humana da seção 9 ("Após T-09") | ✅ Resolvido | Autor confirmou formulário, mensagem de erro e botão de cópia em navegador real em 2026-08-14; checkbox marcado em `docs/plans/PLAN-001-encurtador-url.md:533` |

---

## Findings detalhados

Nenhum finding aberto.

#### R-01 — Commit de T-08/T-09 ainda pendente; lembrar de citar T-09 na mensagem

- **Status:** ✅ Fechado. Commit `2d63387` ("T-09: Razor Page com formulario, resultado e mensagens de erro") cita `T-09` na mensagem e descreve explicitamente a correção em `EncurtadorDbContext.cs` (round-trip de `DateTimeKind`), fechando o laço de rastreabilidade entre `git log` e o plano.

---

## Cobertura por RN (Implementa)

Sem mudanças desde o round 1 — reconfirmado nesta sessão:

### RN-14 — Tela exibe a URL curta completa (esquema, host, código), pronta para cópia

- **Status:** ✅ Implementada corretamente (`src/Encurtador.Web/Pages/Index.cshtml.cs:33`; `Index.cshtml:22-29`). Validação humana em navegador real confirmada — o botão de cópia funciona como esperado.

### RN-15 — Falha de validação reexibe o formulário com a entrada preservada e a mensagem da regra violada

- **Status:** ✅ Implementada corretamente (`Index.cshtml.cs:23-31`; `Index.cshtml:15-18`).

---

## Cobertura por CA (Valida)

Sem mudanças desde o round 1:

### CA-01 — Encurtamento bem-sucedido de URL válida

- **Teste correspondente:** `tests/Encurtador.Tests/EncurtamentoWebTests.cs::CA_01_UrlValida_CriaLinkEExibeUrlCurtaCompleta`
- **Status:** ✅ (reexecutado neste round — verde)

### CA-11 — Formulário preserva a entrada após rejeição

- **Teste correspondente:** `tests/Encurtador.Tests/EncurtamentoWebTests.cs::CA_11_UrlInvalida_PreservaEntradaEExibeMensagem`
- **Status:** ✅ (reexecutado neste round — verde)

`dotnet test` neste round: 29/29 verdes (nenhuma mudança de contagem desde o round 1).

---

## Verificação da Decisão Arquitetural

### ADR-001 — Formulário como Razor Page

- **Conformidade:** ✅ Inalterada desde o round 1.

---

## Notas ao processo (não-findings)

- **Commit fechado:** T-08 (`2da7b44`) e T-09 (`2d63387`) foram commitados separadamente, cada um citando sua própria `T-XX`. R-01 (acima) está resolvido.
- **Seção 9 do plano agora está com todos os pontos anteriores a T-11 marcados** (T-02, T-07, T-09). Restam "Durante T-11" (docker build/run) e "Antes de fechar" (smoke test no equipamento da apresentação), ambos ainda não aplicáveis nesta fase do plano.

---

## Conclusão

T-09 está de fato fechada: os dois critérios de aceite têm teste passando, a decisão arquitetural (ADR-001) foi respeitada, e o único finding `Importante` do round 1 (R-01, deriva de escopo da correção de `DateTimeKind`) foi resolvido com a nota retroativa já presente na seção 11 do plano e o comentário correspondente no código. O ponto de validação humana da seção 9 — abrir a tela em navegador real e confirmar visualmente formulário, mensagem de erro e cópia — também foi confirmado pelo autor. Não há mais bloqueio para avançar a T-10.

**Próximos passos:**

- ✅ T-08 e T-09 commitados em commits separados (`2da7b44`, `2d63387`), cada um citando sua própria `T-XX` — T-09 cita também a correção em `EncurtadorDbContext.cs`.
- ✅ T-10 (brand guide Leanwork) concluída e aprovada.

Nenhum próximo passo pendente para T-09.
