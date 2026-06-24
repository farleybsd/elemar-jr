using FastEndpoints;
using WorkflowCore.Interface;

namespace Poc.WorkflowCore.Domain.Orchestration.Features.NewEnrollmentFlow;

public class NewEnrollmentEndpoint(IWorkflowHost workflowHost) : Endpoint<Request, NewEnrollmentResponse>
{
    public override void Configure()
    {
        Post("/api/enrollmentsWorkflow");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var workflowData = new NewEnrollmentFlowData
        {
            ClientId = request.ClientId,
            PlanId = request.PlanId
        };

        var workflowId = await workflowHost.StartWorkflow("new-enrollment-workflow", workflowData);
        await Send.OkAsync(new NewEnrollmentResponse(workflowId), cancellation: ct);
    }
}

public record NewEnrollmentResponse(string WorkflowId);
