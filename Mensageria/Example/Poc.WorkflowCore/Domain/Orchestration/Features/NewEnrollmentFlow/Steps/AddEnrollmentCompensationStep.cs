using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Poc.WorkflowCore.Common;
using Polly;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Poc.WorkflowCore.Domain.Orchestration.Features.NewEnrollmentFlow.Steps;

public class AddEnrollmentCompensationStep(IOptions<ServicesSettings> options) : StepBodyAsync
{
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var data = context.Workflow.Data as NewEnrollmentFlowData;

        if (data.EnrollmentCreated == true)
        {
            var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                return await options.Value.SubscriptionsUri
                    .AppendPathSegment($"enrollments/{data.EnrollmentId}")
                    .DeleteAsync();
            });

            if (response.Outcome == OutcomeType.Failure)
                throw response.FinalException;
            if (!response.Result.ResponseMessage.IsSuccessStatusCode)
                throw new InvalidOperationException("Falha ao compensar matrícula.");
        }

        return ExecutionResult.Next();
    }
}
