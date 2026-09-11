# CLAUDE.md

Instruções para agentes de IA trabalhando neste projeto.

## Resumo

Encurtador de URL em .NET — demo da palestra "Do PRD ao Deploy" (LondrinaTech Meetup, 15/08/2026). O sistema em si é banal de propósito: o produto real é a **rastreabilidade** `ADR → RN → CA → T → R` entre arquitetura, PRD, plano de execução e código. Ver `docs/architecture/proposta-arquitetural.md` §1.

Não-objetivos declarados (não sugerir nem implementar): painel admin, métricas/analytics, autenticação, alias customizado, expiração/edição/remoção de links, deploy em cloud, escala horizontal.

## Stack

- .NET 10 / ASP.NET Core — Razor Pages (formulário) + Minimal API (`GET /{codigo}` para o redirect)
- EF Core 10 + SQLite (`shortener.db`, arquivo local, migration aplicada na inicialização)
- xUnit + `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`) para testes de integração
- Docker (Dockerfile multi-stage) como único mecanismo de "deploy"
- Sem framework CSS (Bootstrap removido do scaffold padrão — ver ADR e T-10); paleta monocromática, fonte Figtree embarcada em `wwwroot`

## Comandos

```bash
dotnet build                                    # compila os dois projetos
dotnet test                                     # roda a suíte xUnit (raiz, sem argumentos)
dotnet run --project src/Encurtador.Web         # sobe a aplicação localmente
dotnet ef migrations add <Nome> --project src/Encurtador.Web   # nova migration
docker build -t encurtador .                    # build da imagem
docker run -p 8080:8080 encurtador              # sobe via Docker
```

## Convenções

- **Layout de solution**: `src/Encurtador.Web` (aplicação) + `tests/Encurtador.Tests` (xUnit), `Encurtador.sln` na raiz.
- **Nomes em português**: classes, propriedades e regras de negócio usam nomes em PT-BR (`ValidadorDeUrl`, `ServicoDeEncurtamento`, `UrlDestino`, `CriadoEm`). Manter consistência com o restante do código.
- **Métodos assíncronos de serviço levam o sufixo `Async`** (ex.: `ServicoDeEncurtamento.EncurtarAsync`) — convenção .NET padrão, fixada em T-07 (review R-02) antes que T-08/T-09 criassem mais métodos assíncronos com nomes divergentes.
- **Testes carregam o ID do critério de aceite no nome** (ADR-008): `CA_XX_descricao_do_cenario`. É assim que a rastreabilidade se verifica por busca textual — nunca implementar um CA sem o teste nomeado.
- **Sem repositório genérico sobre o `DbContext`**: acesso a dados é direto via EF Core (ADR-002). Não introduzir camada de abstração adicional.
- **Redirect é sempre 302**, nunca 301 (ADR-004) — decisão deliberada para a demo em palco, não trocar sem revisitar a ADR.
- **Migration via `Database.Migrate()`** na inicialização, nunca `EnsureCreated()` (ADR-006).
- Commits referenciam o ID da tarefa do plano (`T-XX`) na mensagem.

## Restrições

- Meta de legibilidade: código de produção cabe em poucos arquivos pequenos (~120 linhas cada). Antes de adicionar uma camada ou abstração, checar se ela paga o custo contra esse atributo (arquitetura §3.1).
- Reprodutibilidade é atributo de qualidade prioritário: nada de dependência de rede em tempo de execução, nada de variável de ambiente obrigatória, nada de serviço externo (arquitetura §3.2). `docker build && docker run` precisa continuar funcionando em máquina limpa.
- Validação de URL segue allowlist de esquema (`http`/`https`), limite de 2048 caracteres e bloqueio de destino no próprio host (ADR-005) — não afrouxar essas regras sem atualizar RN e CA correspondentes no PRD.
- Não adicionar dependência de CDN externo (ex.: Google Fonts) — a fonte precisa carregar offline (T-10).

## Onde procurar antes de implementar

Antes de qualquer mudança de comportamento, siga a cadeia de artefatos — não improvise regra de negócio direto no código:

1. **Plano de execução** — `docs/plans/PLAN-001-encurtador-url.md`. Encontre a próxima tarefa `Pendente` sem bloqueio (campo `Depende de` satisfeito). Leia os campos `Implementa`, `Valida`, `Decisões base` e a seção 9 (pontos de validação humana).
2. **Regras de negócio** — `docs/prds/PRD-001-encurtador-url.md`, seção de regras (RN-XX) e critérios de aceite em Gherkin (CA-XX), referenciados pela tarefa.
3. **Decisões arquiteturais** — `docs/architecture/proposta-arquitetural.md`, seção 5 (ADR-001 a ADR-008), referenciadas pela tarefa e pelo PRD.
4. **Reviews anteriores** — `docs/reviews/REVIEW-T-XX-*.md`, para findings (R-XX) já levantados sobre tarefas concluídas.

Ao concluir uma tarefa: atualizar o `Status` e a tabela "Histórico de execução" no plano (seção 11), e rodar `/leanwork-review` para gerar o relatório antes de seguir para a próxima.

Convenções gerais de IDs e estrutura de pastas do pipeline: `.claude/templates/id-conventions.md` e `.claude/templates/folder-conventions.md`.
