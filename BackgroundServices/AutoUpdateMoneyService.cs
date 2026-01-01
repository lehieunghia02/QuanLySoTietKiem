using QuanLySoTietKiem.BackgroundServices.Interfaces;

namespace QuanLySoTietKiem.BackgroundServices
{
    public class AutoUpdateMoneyService(
        IServiceProvider serviceProvider,
        ILogger<AutoUpdateMoneyService> logger
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogInformation("Starting the auto update process: {time}", DateTime.Now);
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var autoUpdateService = scope.ServiceProvider
                            .GetRequiredService<IAutoUpdateMoneyService>();
                        await autoUpdateService.ProcessAutoUpdateAsync();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in the auto update process");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}