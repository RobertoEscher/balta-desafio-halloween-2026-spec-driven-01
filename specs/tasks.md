# Tarefas

| ID | Tarefa | Origem | Depende de | Concluída quando |
|---|---|---|---|---|
| T-01 | Criar projeto e estrutura de pastas | Constituição (stack e arquitetura), D-06 | | Projeto compila |
| T-02 | Alinhar o plano técnico à constituição e à especificação | Constituição, Spec, Plano | T-01 | O plano define a arquitetura, as regras de unicidade e normalização, o serviço e os contratos de criação, consulta e edição sem contradições |
| T-03 | Implementar o modelo de contato e as regras de domínio | RN-01 a RN-06 | T-02 | Nome e telefone são tratados conforme as regras acordadas, com testes unitários para regras e casos de borda |
| T-04 | Configurar persistência SQLite para contatos | RF-02, D-01 a D-05 | T-03 | O EF Core persiste contatos e o banco garante a unicidade acordada para o telefone normalizado |
| T-05 | Implementar o serviço de aplicação de contatos | RF-02, RF-05, RF-06; RN-01 a RN-06 | T-03, T-04 | O serviço cria, edita e consulta contatos; regras e erros têm testes unitários |
| T-06 | Implementar os endpoints da API | Contratos do plano; RF-02, RF-05, RF-06 | T-05 | Endpoints de criação, busca por nome, consulta por ID e edição usam o serviço; erros de validação retornam 400 com ProblemDetails e cada endpoint tem teste de integração do caminho feliz |
| T-07 | Implementar a interface de cadastro, consulta e edição | RF-01, RF-03 a RF-06 | T-06 | A interface exibe os campos "Nome" e "Telefone completo", permite cadastrar, consultar e editar; exibe a mensagem de sucesso e respeita o estado habilitado/desabilitado do botão Salvar |
| T-08 | Validar os critérios de aceite da agenda | Critérios de aceite | T-06, T-07 | Os critérios de aceite estão cobertos por testes e a solução compila |
