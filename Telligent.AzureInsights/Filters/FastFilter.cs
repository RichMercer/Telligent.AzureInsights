using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Telligent.AzureInsights.Filters
{
    /// <summary>
    /// This filters out a dependency call that is under 100 milliseconds. In many cases, fast queries are not interesting to diagnose.
    /// </summary>
    public class FastFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public FastFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            var request = item as DependencyTelemetry;

            if (request != null && request.Duration.Milliseconds < 100)
            {
                return;
            }

            this.Next.Process(item);
        }
    }
}
