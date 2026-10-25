# Plano técnico

## Contexto
Implemneta a especificação da agenda de contatos, respeitando a constituição do projeto.

## Arquitetura
- Implementado em C# .NET 10 com Blazor Server e acesso a banco SQLite.
- API HTTP em Minimal API, um arquivo de endpoints por recurso
- Camada de aplicação com LinkService, dona das regras RN-01 a RN-04
- Camada de dados com EF Core e um repositório por agregado
- Endpoint só valida entrada, chama o serviço e traduz o resultado em resposta

## Decisões
- D-01: SQLite como armazenamento, porque o RF-02 exige persistência dos dados inseridos.
- D-02: Nenhum dos campos podem ser nulos.
- D-03: O Id deve ser gerado automaticamente a cada INSERT;
- D-04: Não deve ser permitido a edição ou alteração do Id;
- D-05: unicidade do código garantida por índice único no banco, além da checagem no serviço, porque a checagem sozinha tem corrida.

## Modelo de dados
- Tabela Contact, campos Id (único), Name e Phone.

## Contratos
- POST /api/cadastro entrada: Name, Phone
- POST /api/cadastro saída: Id, Name, Phone
- GET /api/busca/ entrada
- GET /api/busca/{id} entrada
- GET /api/busca/ saída: Id, Name, Phone

## Riscos
- Concorrência na criação de código próprio, mitigada por D-05.