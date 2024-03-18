
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class RunUpdateViewsBackgroundTask : BackgroundService
    {
        private readonly ISongService _songService;
        private readonly ILogger _logger;
        private int _songId = 0;

        public RunUpdateViewsBackgroundTask(int songId, ISongService songService, ILogger logger)
        {
            _songId = songId;
            _songService = songService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background: Update song views service running.");            
            try
            {
                var result = await _songService.UpdateViewsAsync(_songId, stoppingToken);
                _logger.LogInformation($"Background: Update song views: {(int)result.Views}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Background: Update song views service stopping: {ex}");
            }
        }
    }
}
