using lib_management_system.Repositories;
using lib_management_system.Services;

namespace lib_management_system.Extensions
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBorrowRepository, BorrowRepository>();
            services.AddScoped<IBorrowService, BorrowService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IBookRequestRepository, BookRequestRepository>();
            services.AddScoped<IBookRequestService, BookRequestService>();
            services.AddScoped<IMembershipRequestRepository, MembershipRequestRepository>();
            services.AddScoped<IMembershipRequestService, MembershipRequestService>();

            return services;
        }
    }
}
