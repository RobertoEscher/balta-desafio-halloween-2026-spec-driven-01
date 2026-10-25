# Constituição do projeto

## Stack
- Testes com xUnit

## Arquitetura
- Endpoint não acessa banco diretamente, sempre via serviço de aplicação.
- Regra de negócio não vive no endpoint nem no handler HTTP.
- Nenhuma dependência nova entra sem justificativa escrita no plano.

## Qualidade
- Toda regra de negócio precisa ter teste unitário.
- Todo endpoint precisa pelo menos ter um teste de integração de caminho feliz.
- Erro de valiação sempre retorna erro 400 com ProblemDetails.

## Convenções
- Nomes de classes e nome de arquivos em inglês, mensagens de erro em português.
- Artefatos de documentção (constitution.md, spec.md, plan.md, tasks.md) com texto gerado e/ou revisado sempre em português.
- Commit no formato: tipo(escopo): descrição, com o identicador do requisito. 

## Governança
- Se o plano contradiz a constituição, quem muda é o plano.
- Se a constituição precisa mudar, muda em commit separado.