using Microsoft.AspNetCore.Http.HttpResults;

namespace REPR.Errors;

public record struct ErrorMessage(string Code, string Message);

public class NormalizeBadRequestErrorsFilter : IEndpointFilter
{
    // Intercepta a execução do endpoint permitindo alterar o resultado antes de retornar ao cliente
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Executa o próximo elemento do pipeline (endpoint ou outro filtro)
        var result = await next(context);

        // Se o retorno for BadRequest<ErrorMessage>, converte para um array padronizando a resposta
        return result is BadRequest<ErrorMessage> badRequest
             ? Results.BadRequest(new[] { badRequest.Value })
             : result;
    }
}
