# Plano técnico

## Contexto
Implementa a especificação da agenda de contatos, respeitando a constituição do projeto.

## Arquitetura
- A aplicação usa C# .NET 10, Blazor Server para a interface e Minimal API para a API HTTP.
- Blazor Server atende aos formulários e à interação exigidos por RF-01 e RF-03 a RF-06; a Minimal API atende aos contratos HTTP definidos neste plano.
- A API HTTP usa Minimal API, com um arquivo de endpoints por recurso.
- A camada de aplicação contém o `ContactService`, responsável pelos casos de uso e pelas regras RN-01 a RN-06.
- A camada de dados usa EF Core e um repositório para o agregado Contact.
- O endpoint valida a entrada, chama o serviço e traduz o resultado em resposta; não acessa o banco nem contém regras de negócio.

## Decisões
- D-01: SQLite como armazenamento, porque o RF-02 exige persistência dos dados inseridos.
- D-02: Name e Phone são obrigatórios e não nulos, conforme RN-01.
- D-03: O Id é gerado automaticamente a cada INSERT.
- D-04: O Id não pode ser editado nem alterado.
- D-05: A unicidade é garantida para Phone normalizado, por checagem no serviço e índice único no banco, conforme RN-02 e RN-06; a checagem isolada não evita condições de corrida.
- D-06: A estrutura inicial do projeto segue a stack e a arquitetura definidas na Constituição: .NET 10, C#, Minimal API, EF Core, SQLite e xUnit.
- D-07: bUnit é usado nos testes para renderizar o componente Blazor e verificar a habilitação do botão Salvar após sucesso e ao iniciar um novo cadastro, comportamento que não é coberto pelos testes HTTP da API.

## Modelo de dados
- Tabela Contact com Id (chave primária gerada), Name, Phone e NormalizedName interno.
- Phone é armazenado somente com os dígitos, conforme RN-06; o índice único incide sobre esse valor normalizado.
- NormalizedName é armazenado como chave interna derivada do Name aparado e truncado em caixa alta invariável, para permitir consultas completas sem diferenciar maiúsculas de minúsculas; não faz parte das respostas da API.

## Contratos
- POST /api/cadastro entrada: Name, Phone; saída: Id, Name, Phone.
- PUT /api/cadastro/{id} entrada: Name, Phone; saída: Id, Name, Phone. O Id da rota identifica o contato e não pode ser alterado.
- GET /api/busca?nome={nomeCompleto} busca pelo nome completo sem diferenciar maiúsculas de minúsculas e retorna todos os contatos correspondentes.
- GET /api/busca/{id} consulta um contato pelo Id.
- As consultas retornam Id, Name e Phone.

## Riscos
- Concorrência ao cadastrar telefones iguais após normalização, mitigada pelo índice único definido em D-05.