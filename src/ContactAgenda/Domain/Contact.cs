using System.Globalization;

namespace ContactAgenda.Domain;

public sealed class Contact
{
    private Contact()
    {
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;

    public static Contact Create(string? name, string? phone)
    {
        var details = Normalize(name, phone);

        return new Contact
        {
            Name = details.Name,
            Phone = details.Phone,
            NormalizedName = details.NormalizedName
        };
    }

    public static ContactDetails Normalize(string? name, string? phone)
    {
        var errors = new Dictionary<string, string[]>();
        var normalizedName = NormalizeName(name, errors);
        var normalizedPhone = NormalizePhone(phone, errors);

        if (errors.Count > 0)
        {
            throw new ContactValidationException(errors);
        }

        return new ContactDetails(normalizedName, normalizedPhone, normalizedName.ToUpperInvariant());
    }

    public static string NormalizeNameForSearch(string? name)
    {
        var errors = new Dictionary<string, string[]>();
        var normalizedName = NormalizeName(name, errors);
        if (errors.Count > 0)
        {
            throw new ContactValidationException(errors);
        }

        return normalizedName.ToUpperInvariant();
    }

    public void Update(ContactDetails details)
    {
        Name = details.Name;
        Phone = details.Phone;
        NormalizedName = details.NormalizedName;
    }

    private static string NormalizeName(string? name, IDictionary<string, string[]> errors)
    {
        var trimmedName = name?.Trim() ?? string.Empty;

        if (trimmedName.Length == 0)
        {
            errors[nameof(Name)] = ["O nome é obrigatório."];
            return string.Empty;
        }

        var textElementStarts = StringInfo.ParseCombiningCharacters(trimmedName);
        if (textElementStarts.Length > 50)
        {
            trimmedName = trimmedName[..textElementStarts[50]];
        }

        return trimmedName;
    }

    private static string NormalizePhone(string? phone, IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            errors[nameof(Phone)] = ["O telefone completo é obrigatório."];
            return string.Empty;
        }

        var digits = new System.Text.StringBuilder(phone.Length);
        foreach (var character in phone)
        {
            if (char.IsAsciiDigit(character))
            {
                digits.Append(character);
            }
            else if (!char.IsWhiteSpace(character) && character is not '(' and not ')' and not '-' and not '.')
            {
                errors[nameof(Phone)] = ["O telefone deve conter apenas dígitos e separadores de formatação."];
                return string.Empty;
            }
        }

        if (digits.Length is not 10 and not 11)
        {
            errors[nameof(Phone)] = ["O telefone completo deve conter 10 ou 11 dígitos, incluindo o DDD."];
            return string.Empty;
        }

        return digits.ToString();
    }
}

public sealed record ContactDetails(string Name, string Phone, string NormalizedName);
