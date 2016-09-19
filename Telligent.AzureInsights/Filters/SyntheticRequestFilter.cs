using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Telligent.AzureInsights.Filters
{
    /// <summary>
    /// This filters out a dependency call that is under 100 milliseconds. In many cases, fast queries are not interesting to diagnose.
    /// </summary>
    public class SyntheticRequestFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public SyntheticRequestFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (!string.IsNullOrEmpty(item.Context.Operation.SyntheticSource)) { return; }

            Next.Process(item);
        }
    }
}
