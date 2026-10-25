namespace ContactAgenda.Domain;

public sealed class ContactValidationException(IReadOnlyDictionary<string, string[]> errors)
    : Exception("Os dados do contato são inválidos.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
