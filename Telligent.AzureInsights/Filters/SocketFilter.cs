using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Telligent.Evolution.Extensibility.Api.Version1;

namespace Telligent.AzureInsights.Filters
{
    public class SocketFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public SocketFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            var request = item as RequestTelemetry;

            if (request != null && request.Url.LocalPath.Contains("/socket.ashx"))
            {
                return;
            }

            Next.Process(item);
        }
    }
}
