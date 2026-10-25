using ContactAgenda.Application;
using ContactAgenda.Domain;

namespace ContactAgenda.Endpoints;

public static class ContactEndpoints
{
    public static IEndpointRouteBuilder MapContactEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var contacts = endpoints.MapGroup("/api");

        contacts.MapPost("/cadastro", CreateAsync);
        contacts.MapPut("/cadastro/{id:int}", UpdateAsync);
        contacts.MapGet("/busca", SearchByNameAsync);
        contacts.MapGet("/busca/{id:int}", GetByIdAsync);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        ContactRequest request,
        ContactService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await service.CreateAsync(request.Name, request.Phone, cancellationToken);
            return Results.Created($"/api/busca/{contact.Id}", ToResponse(contact));
        }
        catch (ContactValidationException exception)
        {
            return ValidationProblem(exception);
        }
        catch (DuplicatePhoneException exception)
        {
            return Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status400BadRequest, title: "Dados inválidos");
        }
    }

    private static async Task<IResult> UpdateAsync(
        int id,
        ContactRequest request,
        ContactService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await service.UpdateAsync(id, request.Name, request.Phone, cancellationToken);
            return Results.Ok(ToResponse(contact));
        }
        catch (ContactValidationException exception)
        {
            return ValidationProblem(exception);
        }
        catch (DuplicatePhoneException exception)
        {
            return Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status400BadRequest, title: "Dados inválidos");
        }
        catch (ContactNotFoundException exception)
        {
            return Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status404NotFound, title: "Contato não encontrado");
        }
    }

    private static async Task<IResult> SearchByNameAsync(
        string? nome,
        ContactService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var contacts = await service.SearchByNameAsync(nome, cancellationToken);
            return Results.Ok(contacts.Select(ToResponse));
        }
        catch (ContactValidationException exception)
        {
            return ValidationProblem(exception);
        }
    }

    private static async Task<IResult> GetByIdAsync(
        int id,
        ContactService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await service.GetByIdAsync(id, cancellationToken);
            return Results.Ok(ToResponse(contact));
        }
        catch (ContactNotFoundException exception)
        {
            return Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status404NotFound, title: "Contato não encontrado");
        }
    }

    private static IResult ValidationProblem(ContactValidationException exception)
    {
        return Results.Problem(
            detail: exception.Message,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Dados inválidos",
            extensions: new Dictionary<string, object?> { ["errors"] = exception.Errors });
    }

    private static ContactResponse ToResponse(Contact contact)
    {
        return new ContactResponse(contact.Id, contact.Name, contact.Phone);
    }
}
