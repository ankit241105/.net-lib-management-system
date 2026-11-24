using lib_management_system.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services
    .AddApiServices()
    .AddGlobalExceptionHandling()
    .AddPersistence(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddCorsPolicy()
    .AddApplicationServices()
    .AddEmailAndReminders(builder.Configuration);



var app = builder.Build();

app.UseApiPipeline();

app.Run();
