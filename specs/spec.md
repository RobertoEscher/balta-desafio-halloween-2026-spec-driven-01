# Agenda de contatos

## Problema
- Manter os telefones de contatos atualizados em uma agenda de papel dificulta a atualização e torna a busca demorada.

## Objetivo
- Permitir que um organizador cadastre contatos e consulte os dados de um contato pelo nome.

## Usuários
- Organizador: pessoa responsável por manter uma agenda única de contatos atualizada.
- A agenda é destinada a um único organizador. Não há cadastro nem login de usuário.

## Histórias
- Como organizador, quero cadastrar os dados de um contato.
- Como organizador, quero consultar os dados de um contato pelo nome completo.

## Requisitos funcionais
RF-01: Formulário de cadastro: o sistema deve exibir os campos "Nome" e "Telefone completo".
RF-02: Salvamento de dados: o sistema deve permitir salvar as informações preenchidas na agenda única em um banco de dados.
RF-03: Feedback visual: após a gravação correta dos dados, o sistema deve exibir a mensagem "Contato salvo com sucesso!".
RF-04: Botão "Salvar": após o salvamento bem-sucedido, o botão deve permanecer desabilitado até o organizador iniciar um novo cadastro. Em caso de falha no salvamento, deve ser possível tentar novamente.
RF-05: Consulta: o sistema deve permitir buscar pelo nome completo, sem diferenciar letras maiúsculas de minúsculas. Se houver mais de um contato com o mesmo nome, deve exibir todos os correspondentes.
Edição: Deve ser permitido editar um determinado contato e salvá-lo.

## Regras de negócio
RN-01: Campos obrigatórios: "Nome" e "Telefone completo" são obrigatórios para criar um contato.
RN-02: Unicidade: não deve ser permitido cadastrar dois contatos com o mesmo número de telefone completo, após a normalização.
RN-03: Nome: remover espaços em branco no início e no fim. Se o nome exceder 50 caracteres, truncá-lo para 50 caracteres. Outros caracteres, inclusive caracteres especiais e emojis, são permitidos.
RN-04: Telefone completo: deve conter o DDD seguido do número de telefone, com 10 dígitos no total para telefones fixos ou 11 dígitos no total para celulares.
RN-05: Validação do DDD: validar somente a quantidade total de dígitos do telefone completo; não verificar se o DDD é válido segundo a ANATEL.
RN-06: Normalização do telefone: aceitar separadores de formatação, como parênteses, espaços e hífen, e considerar apenas os dígitos para validação, salvamento e verificação de unicidade.

## Casos de borda
- Nome com 5.000 caracteres: após remover espaços externos, truncar o nome para os primeiros 50 caracteres.
- Nome com espaços externos: salvar o nome sem os espaços em branco no início e no fim.
- Telefone com DDD inexistente, como 00 ou 99: aceitar se o número completo tiver 10 ou 11 dígitos; a validade do DDD não é verificada.
- Telefone formatado, como "(21) 99999-9999": remover os separadores e tratar o valor normalizado "21999999999" para validação e unicidade.
- Contatos com nomes iguais: a consulta pelo nome completo deve exibir todos os contatos correspondentes.

## Fora de escopo
- Exclusão de contatos.
- Listagem de todos os contatos, incluindo agrupamento alfabético.
- Cadastro ou login do organizador.
- Rotinas de backup.

## Critérios de aceite
- Deve ser possível cadastrar um contato com nome e telefone completo válidos.
- Após salvar corretamente, deve ser exibida a mensagem "Contato salvo com sucesso!" e o botão "Salvar" deve permanecer desabilitado até iniciar um novo cadastro.
- Deve ser possível consultar um contato pelo nome completo, sem diferenciar maiúsculas de minúsculas.
- Se mais de um contato tiver o nome consultado, todos devem ser exibidos.
- Não deve ser possível cadastrar dois contatos com o mesmo telefone completo após a normalização.
- O nome deve ser aparado e truncado para até 50 caracteres, conforme as regras definidas.
- O telefone completo deve ser aceito quando tiver 10 ou 11 dígitos após a normalização, independentemente da validade ANATEL do DDD.
- Deve ser permitido editar um determinado contato cadastrado.