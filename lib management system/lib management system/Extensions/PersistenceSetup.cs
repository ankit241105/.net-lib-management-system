using lib_management_system.Data;
using Microsoft.EntityFrameworkCore;

namespace lib_management_system.Extensions
{
    // Database/EF Core registration
    public static class PersistenceSetup
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
