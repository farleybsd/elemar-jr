using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Poc.WorkflowCore.Common;
using Polly;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Poc.WorkflowCore.Domain.Orchestration.Features.NewEnrollmentFlow.Steps;

public class AddEnrollmentStep(IOptions<ServicesSettings> options) : StepBodyAsync
{
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var data = context.Workflow.Data as NewEnrollmentFlowData;

        var request = new AddEnrollmentRequest(
           data!.Name,
           data.Email,
           data.Phone,
           data.Document,
           data.BirthDate,
           data.Gender,
           data.Address
       );

        var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            return await options.Value.SubscriptionsUri
                .AppendPathSegment("enrollments")
                .PostJsonAsync(request);
        });

        if (response.Outcome == OutcomeType.Failure)
            throw response.FinalException;
        if (!response.Result.ResponseMessage.IsSuccessStatusCode)
            throw new InvalidOperationException("Falha ao criar matrícula.");

        var enrollmentResponse = await response.Result.GetJsonAsync<AddEnrollmentResponse>();
        data.EnrollmentId = enrollmentResponse.EnrollmentId;
        data.EnrollmentCreated = true;
        return ExecutionResult.Next();
    }
}
public record AddEnrollmentRequest(
     string Name,
     string Email,
     string Phone,
     string Document,
     DateTime BirthDate,
     string Gender,
     string Address
 );

public record AddEnrollmentResponse(Guid EnrollmentId);