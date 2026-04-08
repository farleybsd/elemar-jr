using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Trace;

namespace GymErp.Common.Telemetry;

internal static class GymErpTelemetry
{
    internal const string ServiceName = "GymErp";

    internal static readonly ActivitySource ActivitySource = new(ServiceName);
    internal static readonly Meter Meter = new(ServiceName);

    private static readonly Counter<long> NewEnrollmentTotal =
        Meter.CreateCounter<long>("new_enrollment_total", unit: "{count}");

    private static readonly Histogram<double> NewEnrollmentDurationMs =
        Meter.CreateHistogram<double>("new_enrollment_duration_ms", unit: "ms");

    private static readonly Histogram<double> NewEnrollmentJourneyDurationMs =
        Meter.CreateHistogram<double>("new_enrollment_journey_duration_ms", unit: "ms");

    internal static Activity? StartNewEnrollmentHandlerActivity() =>
        ActivitySource.StartActivity("NewEnrollment.Handler", ActivityKind.Internal);

    internal static Activity? StartEnrollmentCreatedConsumerActivity(string module) =>
        ActivitySource.StartActivity($"{module}.Handler", ActivityKind.Internal);

    internal static void AddCommonEnrollmentTags(Activity? activity, Guid enrollmentId, string? clientId = null)
    {
        if (activity is null) return;
        activity.SetTag("enrollment.id", enrollmentId);
        if (!string.IsNullOrWhiteSpace(clientId))
            activity.SetTag("client.id", clientId);
    }

    internal static void AddMessagingTags(Activity? activity, string system, string destination, string? consumerGroup = null)
    {
        if (activity is null) return;
        activity.SetTag("messaging.system", system);
        activity.SetTag("messaging.destination", destination);
        if (!string.IsNullOrWhiteSpace(consumerGroup))
            activity.SetTag("messaging.consumer.group", consumerGroup);
    }

    internal static void IncrementNewEnrollment(string stage, string status)
    {
        NewEnrollmentTotal.Add(
            1,
            new TagList
            {
                { "stage", stage },
                { "status", status }
            });
    }

    internal static void RecordNewEnrollmentDurationMs(string stage, string status, double durationMs)
    {
        NewEnrollmentDurationMs.Record(
            durationMs,
            new TagList
            {
                { "stage", stage },
                { "status", status }
            });
    }

    internal static void RecordNewEnrollmentJourneyDurationMs(string stage, double durationMs)
    {
        NewEnrollmentJourneyDurationMs.Record(
            durationMs,
            new TagList
            {
                { "stage", stage }
            });
    }
}

