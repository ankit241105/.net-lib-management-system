using lib_management_system.Common;
using lib_management_system.Services;

namespace lib_management_system.Extensions
{
    // Email sending + the daily due-date reminder background job.
    public static class BackgroundJobsSetup
    {
        public static IServiceCollection AddEmailAndReminders(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddHostedService<DueReminderService>();
            return services;
        }
    }
}
