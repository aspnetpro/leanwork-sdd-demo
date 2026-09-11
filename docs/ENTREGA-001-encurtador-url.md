# Documento de Entrega — Encurtador de URL

**Cliente:** Leanwork (interno) · **Produto:** Encurtador de URL — demo executável do pipeline SDD
**Referência:** PRD-001 · PLAN-001 · Proposta arquitetural v0.1
**Data da entrega:** 2026-08-15 · **Status:** Aceite registrado em 2026-08-17
**Responsável técnico:** Michel Banagouro

---

## 1. Sumário executivo

O sistema está **completo e entregue**: as 12 tarefas do plano de execução foram implementadas, revisadas e commitadas, sem nenhum finding bloqueante em aberto.

O que foi contratado não era um encurtador de URL — esse problema está resolvido no mercado há vinte anos. O que foi contratado era **prova de que a cadeia `ADR → RN → CA → T → R` fecha de ponta a ponta num repositório real**, e é isso que esta entrega demonstra: cada uma das 16 regras de negócio do PRD tem um critério de aceite, cada um dos 17 critérios tem um teste automatizado que carrega o ID no nome, e cada teste rastreia até a tarefa e a decisão arquitetural que o originaram. A verificação não depende de confiança no documento: é uma busca textual no repositório.

**Estado verificado em 2026-08-15:**

| Indicador | Meta declarada | Entregue |
|---|---|---|
| Regras de negócio implementadas | 16 (RN-01 a RN-16) | 16 |
| Critérios de aceite com teste nomeado pelo ID | 100% dos 17 | 17 de 17 |
| Suíte automatizada | Verde | 29 de 29 aprovados |
| Tarefas do plano concluídas e revisadas | 12 (T-01 a T-12) | 12, com 14 relatórios de review |
| Findings bloqueantes em aberto | 0 | 0 |
| Execução em máquina limpa | `docker build` + `docker run`, sem configuração | Validado em 2026-08-14 (imagem de 118 MB) |

Os três itens de verificação manual que seguiam pendentes foram cumpridos na apresentação de 2026-08-15, com a demo executada em palco. A seção 8 registra o fechamento.

---

## 2. O que foi entregue

| Feature | Entrega | Tarefas | Evidência |
|---|---|---|---|
| Fundação e persistência | Solution .NET 10 com aplicação e suíte de testes; entidade de link, `DbContext` e migration com índice único; schema criado sozinho na inicialização | T-01, T-02, T-03 | `CA_16_PrimeiraExecucao_CriaBancoAutomaticamente` |
| Encurtamento de URL | Validação em quatro regras com ordem determinística; geração de código Base62 não enumerável; tratamento de colisão com retry | T-04, T-05, T-06, T-07 | `CA_02` a `CA_10`, `CA_17` |
| Resolução e redirect | Endpoint `GET /{codigo}` com 302 para código existente e 404 para inexistente, sem efeito colateral de escrita | T-08 | `CA_12` a `CA_15` |
| Interface | Tela única com formulário, exibição da URL curta completa, preservação da entrada em erro, identidade visual Leanwork sem dependência de rede | T-09, T-10 | `CA_01`, `CA_11` |
| Empacotamento | Dockerfile multi-stage; README com instruções de execução e matriz de rastreabilidade | T-11, T-12 | Validação manual registrada no plano §11 |

Cada tarefa gerou um commit próprio com o ID `T-XX` no assunto — o histórico do repositório é navegável tarefa a tarefa, e é o que permite mostrar a evolução amarrada ao plano.

### 2.1 O que deliberadamente não foi entregue

Registrado aqui para que a ausência seja lida como decisão, não como pendência: painel administrativo, listagem de links, métricas e contagem de cliques, autenticação, alias customizado, expiração/edição/remoção de link, QR code, verificação de reputação do destino, rate limiting, deploy em cloud e escala horizontal.

Cada corte tem justificativa individual no PRD §4.2. Nenhum deles é dívida técnica: são fronteiras de escopo acordadas antes da primeira linha de código, e é o que sustenta a promessa de um sistema legível numa sessão.

---

## 3. Evidência de conformidade

Os números abaixo não precisam ser aceitos sob palavra. Três comandos reproduzem a verificação inteira:

```bash
dotnet test                              # 29/29 aprovados
grep -rc "CA_" tests/Encurtador.Tests/   # os 17 IDs de critério aparecem nos nomes dos testes
docker build -t encurtador . && docker run -p 8080:8080 encurtador
```

| Atributo de qualidade prioritário | Meta da arquitetura | Resultado |
|---|---|---|
| **Legibilidade** | Código de produção em até 10 arquivos, nenhum acima de ~120 linhas | 10 arquivos `.cs` escritos à mão, maior com 48 linhas (`Program.cs`); ~400 linhas de C# e Razor no total, fora a migration gerada |
| **Reprodutibilidade** | Sistema sobe em máquina com apenas Docker, sem configuração, sem rede | Cumprido: imagem multi-stage de 118 MB, migration aplicada na inicialização, fonte embarcada sem CDN |
| **Rastreabilidade** | 100% dos critérios de aceite com teste nomeado pelo ID | Cumprido: 17 de 17, suíte verde |

A suíte tem 525 linhas de teste para ~400 de produção. A proporção é consequência da decisão de cobrir todos os critérios antes de qualquer outra coisa — a suíte existe para provar rastreabilidade, não para maximizar percentual de cobertura.

---

## 4. Rastreabilidade — como auditar

A cadeia completa está em `docs/traceability/MATRIX-encurtador-url.md`, com três tabelas: regra → critério → tarefa → decisão, critério → teste, e estado de execução por tarefa.

Um exemplo, do fim ao começo, para dar o método:

> A decisão **ADR-004** fixa redirect 302 em vez de 301, porque navegadores cacheiam 301 de forma persistente e o link se torna impossível de corrigir. Dela nasce a regra **RN-10**, validada pelo critério **CA-12**, implementado na tarefa **T-08** (commit `2da7b44`), testado por `ResolucaoTests.CA_12_CodigoExistente_RetornaTrezentosEDois` e revisado em `REVIEW-T-08-2026-08-14.md`.

Essa navegação funciona nos dois sentidos e para todos os 17 critérios.

**Lacuna registrada, não corrigida:** ADR-001 (aplicação única), ADR-007 (empacotamento Docker) e ADR-008 (convenção de nomenclatura de teste) não são citadas por nenhuma `RN-XX`. A arquitetura §3.3 declarava como meta que toda ADR fosse referenciada por ao menos uma regra de negócio — lida ao pé da letra, a meta não fecha. São as três decisões de natureza estrutural ou de processo, que moldam a aplicação e a convenção de teste sem gerar regra de negócio. A opção foi registrar a lacuna em vez de inventar vínculo artificial para fechar a estatística.

---

## 5. Roteiro de aceite

Sequência sugerida para validação pelo cliente, em máquina com apenas Docker instalado:

1. `docker build -t encurtador . && docker run -p 8080:8080 encurtador` — a aplicação sobe e cria o banco sozinha, sem nenhum passo de configuração.
2. Abrir `http://localhost:8080`, colar uma URL longa e encurtar — a tela devolve a URL curta completa, pronta para cópia.
3. Abrir o link curto em aba anônima — o navegador é levado ao destino original.
4. Acessar um código inexistente — a resposta é 404, não um redirect para a home.
5. Testar `google.com` (sem esquema) e `javascript:alert(1)` — ambos rejeitados, com a entrada preservada no campo e uma mensagem por vez.
6. `dotnet test` na raiz — 29 testes verdes, com os IDs dos critérios visíveis nos nomes.
7. Abrir a matriz de rastreabilidade e escolher uma regra ao acaso; seguir até o teste correspondente por busca textual.

O passo 7 é o aceite que importa. Os seis primeiros validam um encurtador; o sétimo valida o que foi contratado.

---

## 6. Limitações conhecidas

Nenhuma delas é defeito: são decisões tomadas com o contexto declarado — sistema de demonstração, uma instância, sem exposição pública. Todas estão registradas na arquitetura §9, com custo e forma de pagamento.

| Limitação | Quando vira problema | Como resolver |
|---|---|---|
| Banco dentro do contêiner, sem volume por padrão | Em qualquer uso além de uma sessão de demonstração | Bind mount + `ConnectionStrings__Default` — comando pronto e testado, no README |
| Migration aplicada na inicialização | No momento em que existir mais de uma instância subindo em paralelo | Extrair para passo de deploy dedicado ou init container |
| Sem limite de taxa | No primeiro minuto de exposição pública | Middleware de rate limiting nativo do ASP.NET Core, por IP |
| Sem verificação de reputação do destino | Em exposição pública — um link de phishing legítimo em HTTPS passa na validação | Integração com Google Safe Browsing, aceitando a dependência de rede hoje rejeitada |
| Sem expiração ou remoção de link | Em escala real, ou no primeiro pedido de remoção por abuso | Coluna de expiração e tarefa agendada |
| Observabilidade limitada ao log padrão | Quando alguém precisar operar o sistema sem o terminal aberto na frente | Health check e OpenTelemetry — ambos triviais em ASP.NET Core |
| SQLite em arquivo inviabiliza múltiplas instâncias | Se escala horizontal deixar de ser não-objetivo | Trocar o provider para PostgreSQL; o acesso a dados é direto via EF Core, sem camada a reescrever |

**Este sistema não está pronto para exposição pública na internet.** As quatro primeiras limitações acima, somadas, tornam isso explícito — e a distância entre "correto aqui" e "correto em produção" é ela própria conteúdo da apresentação.

---

## 7. Riscos residuais

| Risco | Situação na entrega |
|---|---|
| Cache de navegador falsear a demonstração | Neutralizado — 302 em vez de 301 (ADR-004); procedimento de palco é aba anônima |
| Máquina alvo sem SDK .NET 10 | Neutralizado — imagem Docker autocontida (ADR-007) |
| Código divergir da especificação sem que ninguém perceba | Endereçado pelo próprio pipeline — testes nomeados pelo ID do critério e review por tarefa. Foi o mecanismo que capturou o bug de `DateTimeKind` em T-09 (risco registrado em T-02 e sem teste que o exercitasse) |
| Falha do equipamento de apresentação | Não mitigado por este projeto — depende do ensaio previsto na seção 8 |

---

## 8. Pendências no momento da entrega — fechadas

Nenhuma pendência era de código. As três verificações manuais previstas no plano foram cumpridas na apresentação de 2026-08-15, com a demo executada em palco:

- [x] **Smoke test no equipamento da apresentação** (plano §6): com a imagem rodando e o navegador em aba anônima, encurtar uma URL real, abrir o link curto, apagar o banco, subir de novo e confirmar 404 no link antigo — prova o comportamento do 302 ao vivo.
- [x] **Execução em máquina sem SDK .NET** (plano §7): o `docker build` + `docker run` foi validado em 2026-08-14 na máquina do autor e reexecutado no equipamento da apresentação em 2026-08-15.
- [x] **Ensaio da demo no notebook e no projetor que serão usados** (plano §7).

As duas inconsistências de documentação apontadas na entrega foram corrigidas em 2026-08-17:

- [x] O cabeçalho do `PLAN-001` passou de **Status: Rascunho** para **Concluído**, refletindo as 12 tarefas executadas e revisadas.
- [x] Os pontos de validação humana de **T-02** e **T-07** (plano §9) foram marcados, com referência ao histórico de execução §11, que já registrava as duas revisões como feitas e aprovadas pelo autor.

---

## 9. Próximos passos sugeridos

Fora do escopo contratado, listados por serem as extensões mais naturais — e boas candidatas a exercício de mentoria, porque cada uma atravessa o pipeline inteiro em vez de ser só código:

1. **Expiração de link** — a extensão mais natural: nova RN, novo CA, migration, tarefa e review. Exercita a cadeia completa numa feature pequena.
2. **Volume persistente por padrão** no comando documentado, encerrando a primeira dívida da seção 6.
3. **Health check e rate limiting** — dois passos que aproximam o sistema de operável sem descaracterizar a simplicidade.
4. **Automação da matriz de rastreabilidade em CI** — hoje a matriz é gerada sob demanda e é um snapshot. Em projeto de vida longa, vale falhar o build quando um `CA-XX` do PRD não tiver teste correspondente.

---

## 10. Anexo — índice de artefatos

| Artefato | Caminho | Conteúdo |
|---|---|---|
| Proposta arquitetural | `docs/architecture/proposta-arquitetural.md` | ADR-001 a ADR-008, atributos de qualidade, trade-offs, dívidas conscientes |
| PRD | `docs/prds/PRD-001-encurtador-url.md` | RN-01 a RN-16, CA-01 a CA-17 em Gherkin, escopo e não-escopo |
| Plano de execução | `docs/plans/PLAN-001-encurtador-url.md` | T-01 a T-12, dependências, pontos de validação humana, histórico de execução |
| Reviews | `docs/reviews/REVIEW-T-*.md` | 14 relatórios com findings R-XX por severidade |
| Matriz de rastreabilidade | `docs/traceability/MATRIX-encurtador-url.md` | Cadeia `ADR → RN → CA → T → R` completa, nos dois sentidos |
| Instruções de uso | `README.md` | Como rodar, como testar, matriz resumida |
| Código-fonte | `src/Encurtador.Web`, `tests/Encurtador.Tests` | Aplicação e suíte |
