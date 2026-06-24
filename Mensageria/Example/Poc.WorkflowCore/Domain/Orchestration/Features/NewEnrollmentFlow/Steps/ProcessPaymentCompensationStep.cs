using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Poc.WorkflowCore.Common;
using Polly;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Poc.WorkflowCore.Domain.Orchestration.Features.NewEnrollmentFlow.Steps
{
    public class ProcessPaymentCompensationStep(IOptions<ServicesSettings> options) : StepBodyAsync
    {
        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var data = context.Workflow.Data as NewEnrollmentFlowData;

            if (data?.PaymentProcessed == true)
            {
                var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
                {
                    return await options.Value.ProcessPaymentUri
                        .AppendPathSegment($"refund/{data.EnrollmentId}")
                        .PostAsync();
                });

                if (response.Outcome == OutcomeType.Failure)
                    throw response.FinalException;
                if (!response.Result.ResponseMessage.IsSuccessStatusCode)
                    throw new InvalidOperationException("Falha ao compensar pagamento.");
            }

            return ExecutionResult.Next();
        }
    }
}
