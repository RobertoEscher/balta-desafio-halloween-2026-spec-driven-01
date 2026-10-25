namespace ContactAgenda.Domain;

public sealed class DuplicatePhoneException()
    : Exception("Já existe um contato com este telefone completo.");
