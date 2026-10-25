namespace ContactAgenda.Domain;

public sealed class ContactNotFoundException(int id)
    : Exception($"O contato com identificador {id} não foi encontrado.");
