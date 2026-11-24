namespace lib_management_system.Extensions
{
    public static class CorsSetup
    {
        public const string PolicyName = "AllowFrontend";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(
                              "http://localhost:5173", "http://127.0.0.1:5173",   // React (Vite)
                              "http://localhost:4200", "http://127.0.0.1:4200")    // Angular
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();   // required so the refresh-token cookie can be sent
                });
            });

            return services;
        }
    }
}
