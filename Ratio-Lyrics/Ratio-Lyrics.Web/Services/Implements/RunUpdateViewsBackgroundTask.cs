
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class RunUpdateViewsBackgroundTask : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private int _songId = 0;

        public RunUpdateViewsBackgroundTask(int songId, IServiceProvider serviceProvider, ILogger logger)
        {
            _songId = songId;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background: Update song views service running.");            
            try
            {
                using(var scope = _serviceProvider.CreateScope())
                {
                    var songService = scope.ServiceProvider.GetRequiredService<ISongService>();
                    var result = await songService.UpdateViewsAsync(_songId, stoppingToken);
                    _logger.LogInformation($"Background: Update song views: {(int)result.Views}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Background: Update song views service stopping: {ex}");
            }
        }
    }
}
