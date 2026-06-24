using FastEndpoints;
using Poc.WorkflowCore.Domain.Orchestration.Features.NewEnrollmentFlow;

namespace Poc.WorkflowCore.Domain.Orchestration.Features.AddNewEnrollment;

public class Endpoint(Handler handler) : Endpoint<Request, Guid>
{
    public override void Configure()
    {
        Post("/api/enrollments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(
       Request req,
       CancellationToken ct)
    {
        var enrollmentId = await handler.HandleAsync(req, ct);

        await Send.OkAsync(enrollmentId, ct);
    }
}
public class Handler
{
    public  Task<Guid> HandleAsync(
        Request request,
        CancellationToken ct)
    {
        // Executa a operação
        return Task.FromResult(Guid.NewGuid());
    }
}