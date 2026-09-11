# Review: T-01 — Criar solution, projeto web e projeto de testes

> **Plano de referência:** `docs/plans/PLAN-001-encurtador-url.md`
> **PRD de referência:** `docs/prds/PRD-001-encurtador-url.md`
> **Arquitetura de referência:** `docs/architecture/proposta-arquitetural.md` (v0.1, ADR-001 a ADR-008)
> **Reviewer:** Claude (skill `reviewer-leanwork` v1.0)
> **Data:** 2026-08-14
> **Round:** 2
> **Recomendação final:** ✅ Aprovado

---

## Sumário executivo

Este é o round 2 da review de T-01, disparado para verificar se as correções prometidas após o round 1 (`REVIEW-T-01-2026-08-14.md`) foram de fato aplicadas. Foram: o commit `2c74937` já nasce incorporando as três correções — a mensagem do próprio commit cita explicitamente "Incorpora as correções da REVIEW-T-01-2026-08-14" e lista R-01 e R-03 por número. O commit `460b30c`, seguinte, registra o hash na tabela de histórico do plano, fechando R-02.

Reexecutei os três critérios de aceite testáveis da tarefa contra o estado atual do código (não apenas inspecionei — rodei os comandos): `dotnet build` compila os dois projetos com 0 avisos e 0 erros; `dotnet test` executa a suíte vazia com exit code 0; `dotnet run --project src/Encurtador.Web` serve a home em HTTP 200, sem o aviso `Failed to determine the https port for redirect` que aparecia no round 1 (confirma R-03). Inspecionei o sistema de arquivos e `_Layout.cshtml`: não há mais `Pages/Privacy.cshtml(.cs)` nem link para ela no menu ou rodapé (confirma R-01). `Program.cs` não contém `UseHttpsRedirection()` nem `UseAuthorization()` (confirma R-03).

Não há finding novo neste round. T-01 está aprovada sem ressalvas.

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
| Regras implementadas (RN) | — | — | ✅ |
| Cenários validados (CA) | — | — | ✅ |
| Decisões base (ADR) | ADR-001, ADR-008 | ADR-001, ADR-008 respeitadas | ✅ |
| Critérios de aceite da tarefa | 3 | 3 atendidos (reexecutados neste round) | ✅ |
| Testes prometidos | 0 ("Não aplicável — tarefa estrutural") | 0 | ✅ |

---

## Contexto da implementação

### Stack detectada

- Backend: ASP.NET Core 10, Razor Pages
- Banco: SQLite via EF Core 10 (pacotes instalados; schema ainda não existe — correto, escopo de T-02)
- Testes: xUnit + `Microsoft.AspNetCore.Mvc.Testing`
- **Fonte:** proposta arquitetural §4 e §6.2, confirmada por `Encurtador.Web.csproj` (`net10.0`, `Sdk.Web`) e `Encurtador.Tests.csproj`. `dotnet --version` no ambiente de review: `10.0.400`.

### Padrões específicos aplicados (lidos do projeto)

Sem mudança em relação ao round 1: não há `CLAUDE.md` próprio de `demos/encurtador-url/`. Aplicados os critérios universais e as decisões técnicas explícitas do plano §3 e as ADRs.

### Escopo do diff (desde o round 1)

- **Commits novos:** 2 — `2c74937` (T-01: solution, projeto web e projeto de testes) e `460b30c` (T-01: registrar hash do commit na tabela de histórico do plano).
- **Arquivos alterados por `2c74937` em relação ao estado revisado no round 1:** remoção de `Pages/Privacy.cshtml`, `Pages/Privacy.cshtml.cs` e do link correspondente em `Pages/Shared/_Layout.cshtml`; remoção de `app.UseHttpsRedirection()` e `app.UseAuthorization()` de `Program.cs`. O restante do projeto (82 arquivos, incluindo os 60 de terceiros vendorizados em `wwwroot/lib/`) permanece como no round 1.
- **Arquivo alterado por `460b30c`:** `docs/plans/PLAN-001-encurtador-url.md`, linha da tabela de histórico de T-01 (coluna Commit).

---

## Findings detalhados

Nenhum finding neste round.

---

## Cobertura por RN (Implementa)

Não aplicável — T-01 declara `Implementa: —`. Confirmado novamente: nenhuma regra de negócio foi concretizada nesta tarefa estrutural.

---

## Cobertura por CA (Valida)

Não aplicável — T-01 declara `Valida: —`. Confirmado: nenhum teste `CA_XX` existe no projeto, o que é correto no estado atual do plano.

---

## Verificação da Decisão Arquitetural

### ADR-001 — Aplicação única em Razor Pages, com o redirect fora do roteamento de páginas

- **Decisão original:** um único projeto web ASP.NET Core, sem separação em API + frontend, sem projetos de camada.
- **Implementação:** inalterada desde o round 1. `Encurtador.Web.csproj` é o único projeto deployável. A remoção de `Privacy.cshtml` reforça a leitura de "uma única tela" que a ADR-001 e o PRD §1 pedem.
- **Conformidade:** ✅

### ADR-008 — Suíte xUnit com testes nomeados pelo ID do critério de aceite

- **Decisão original:** xUnit em projeto separado, com `Microsoft.AspNetCore.Mvc.Testing` disponível desde o início.
- **Implementação:** inalterada desde o round 1. `tests/Encurtador.Tests/Encurtador.Tests.csproj` mantém a `ProjectReference` e o pacote de testing.
- **Conformidade:** ✅

---

## Notas ao processo (não-findings)

- **Orçamento de arquivos da arquitetura (§3.1, ~10 arquivos de produção):** com a remoção de `Privacy.cshtml(.cs)`, o projeto web tem hoje ~9 arquivos de código/markup de produção (`Program.cs`, `Index.cshtml`+`.cs`, `Error.cshtml`+`.cs`, `_Layout.cshtml`+`.css`, `_ValidationScriptsPartial.cshtml`, `_ViewImports.cshtml`, `_ViewStart.cshtml`), a maioria boilerplate de poucas linhas. As Fases 2 e 3 do plano ainda vão adicionar `Dados/`, `Servicos/` e migrations. Vale observar esse orçamento ao revisar T-06/T-07, que são as tarefas que mais adicionam arquivos de uma vez — não é finding agora, só um ponto de atenção para reviews futuras.
- **`launchSettings.json` mantém um profile `https`** apontando para `https://localhost:7003` (certificado de desenvolvimento local, não HTTPS real). Não conflita com a arquitetura (que descarta HTTPS com certificado real em produção/Docker, não o dev cert local do Visual Studio/CLI) e não foi exercitado no smoke test. Não vira finding — é diferente do que R-03 endereçou (middleware ativo no pipeline HTTP).

---

## Round anterior

Comparação com `REVIEW-T-01-2026-08-14.md` (round 1):

| Item anterior | Status | Comentário |
|---------------|--------|------------|
| R-01 (round 1) — `Pages/Privacy.cshtml` sem RN/CA, fora do escopo "uma única tela" | ✅ Resolvido | Arquivos removidos e link retirado de `_Layout.cshtml`, no commit `2c74937` |
| R-02 (round 1) — T-01 marcada concluída sem commit correspondente | ✅ Resolvido | Commits `2c74937` e `460b30c`, ambos citando `T-01` na mensagem; tabela de histórico do plano atualizada |
| R-03 (round 1) — Middleware de HTTPS/autorização sem uso real no pipeline | ✅ Resolvido | `UseHttpsRedirection()` e `UseAuthorization()` removidos de `Program.cs`; smoke test confirma ausência do warning de HTTPS no log de inicialização |

---

## Conclusão

As três ressalvas do round 1 foram resolvidas no mesmo commit que fecha T-01, e a segunda tarefa registrou o hash retroativamente na matriz de rastreabilidade. Reexecutei os critérios de aceite (build, test, run) contra o estado atual, não apenas inspecionei o diff — os três seguem verdes. T-01 está aprovada sem ressalvas.

**Próximos passos sugeridos:**

- Nenhuma ação pendente para T-01.
- Seguir para T-02 (entidade `Link`, `DbContext`, migration inicial com índice único), próxima tarefa pendente do plano sem bloqueio.
