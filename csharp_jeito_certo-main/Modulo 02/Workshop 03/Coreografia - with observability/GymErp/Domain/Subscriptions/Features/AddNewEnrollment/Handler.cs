using CSharpFunctionalExtensions;
using GymErp.Common;
using GymErp.Common.Telemetry;
using GymErp.Domain.Subscriptions.Aggreates.Enrollments;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GymErp.Domain.Subscriptions.Features.AddNewEnrollment;

public class Handler(
    EnrollmentRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<Handler> logger)
{
    public async Task<Result<Guid>> HandleAsync(Request request)
    {
        GymErpTelemetry.IncrementNewEnrollment(stage: "enrollment", status: "attempt");

        using var activity = GymErpTelemetry.StartNewEnrollmentHandlerActivity();
        activity?.SetTag("stage", "enrollment");
        activity?.SetTag("client.id", request.Document);
        activity?.SetTag("client.email", request.Email);

        var sw = Stopwatch.StartNew();
        logger.LogDebug(
            "AddNewEnrollment: creating Enrollment aggregate for client.id {client.id} and client.email {client.email}",
            request.Document,
            request.Email);

        // FastEndpoints fornece o CancellationToken no endpoint, mas aqui o handler é resolvido via DI.
        // Para evitar falhas de resolução no Autofac, usamos CancellationToken.None neste nível.
        var cancellationToken = CancellationToken.None;

        try
        {
            var enrollmentResult = Enrollment.Create(
                request.Name,
                request.Email,
                request.Phone,
                request.Document,
                request.BirthDate,
                request.Gender,
                request.Address
            );

            if (enrollmentResult.IsFailure)
            {
                GymErpTelemetry.IncrementNewEnrollment(stage: "enrollment", status: "failure");
                GymErpTelemetry.RecordNewEnrollmentDurationMs("enrollment", "failure", sw.Elapsed.TotalMilliseconds);
                activity?.SetStatus(ActivityStatusCode.Error, enrollmentResult.Error);

                logger.LogWarning("AddNewEnrollment: enrollment validation failed: {Error}", enrollmentResult.Error);
                return Result.Failure<Guid>(enrollmentResult.Error);
            }

            var enrollment = enrollmentResult.Value;
            GymErpTelemetry.AddCommonEnrollmentTags(activity, enrollment.Id, enrollment.Client.Cpf);

            logger.LogDebug(
                "AddNewEnrollment: enrollment created with enrollment.id {enrollment.id}, persisting",
                enrollment.Id);

            await repository.AddAsync(enrollment, cancellationToken);
            await unitOfWork.Commit(cancellationToken);

            GymErpTelemetry.IncrementNewEnrollment(stage: "enrollment", status: "success");
            GymErpTelemetry.RecordNewEnrollmentDurationMs("enrollment", "success", sw.Elapsed.TotalMilliseconds);

            logger.LogInformation(
                "AddNewEnrollment: enrollment persisted with enrollment.id {enrollment.id} and client.id {client.id}",
                enrollment.Id,
                enrollment.Client.Cpf);
            return Result.Success(enrollment.Id);
        }
        catch (Exception ex)
        {
            GymErpTelemetry.IncrementNewEnrollment(stage: "enrollment", status: "failure");
            GymErpTelemetry.RecordNewEnrollmentDurationMs("enrollment", "failure", sw.Elapsed.TotalMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
} 