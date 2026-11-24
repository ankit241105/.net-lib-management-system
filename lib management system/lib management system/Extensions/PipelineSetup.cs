using Serilog;

namespace lib_management_system.Extensions
{
    // The HTTP request pipeline
    public static class PipelineSetup
    {
        public static WebApplication UseApiPipeline(this WebApplication app)
        {
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Library API v1");
                });
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseCors(CorsSetup.PolicyName);

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
