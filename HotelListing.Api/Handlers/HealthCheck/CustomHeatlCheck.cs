using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HotelListing.API.Handlers.HealthCheck
{
    public class CustomHeatlCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //here were gonna check

            var isHealthy = false;


            if(isHealthy)
            {


                return Task.FromResult(HealthCheckResult.Healthy("were good buddy"));
            }

            //return Task.FromResult(HealthCheckResult.Unhealthy("go find a doctor urgente!!"));

            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "go find a doctor urgente!!"));
        }
    }
}
