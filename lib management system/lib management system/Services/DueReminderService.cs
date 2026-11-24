namespace lib_management_system.Services
{
    // Background job: runs once shortly after startup, then every 24 hours. Each run asks
    // the ReminderService to email users whose loans are due within the configured window.
    public class DueReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DueReminderService> _logger;

        public DueReminderService(IServiceScopeFactory scopeFactory, ILogger<DueReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Due-reminder background job started (runs daily).");
            using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
            do
            {
                try
                {
                    // Repositories are scoped, so create a scope per run.
                    using var scope = _scopeFactory.CreateScope();
                    var reminders = scope.ServiceProvider.GetRequiredService<IReminderService>();
                    await reminders.SendDueRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Due-reminder run failed.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
