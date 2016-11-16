using System.Net;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Telligent.AzureInsights.Filters
{
    public class ForbiddenFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public ForbiddenFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            var request = item as RequestTelemetry;

            if (request != null && int.Parse(request.ResponseCode) == (int)HttpStatusCode.Forbidden)
            {
                return;
            }

            Next.Process(item);
        }
    }
}
