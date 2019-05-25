using InQuant.Localizations.DbStringLocalizer;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.Localizations
{
    public class LocalizationInitHosted : IHostedService
    {
        private readonly LocalizationModelContext _localizationModelContext;

        public LocalizationInitHosted(LocalizationModelContext localizationModelContext)
        {
            _localizationModelContext = localizationModelContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _localizationModelContext.EnsureLoadAllLocations();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
