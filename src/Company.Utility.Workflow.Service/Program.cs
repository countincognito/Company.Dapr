using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

namespace Company.Utility.Workflow.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddElsa(elsa =>
            {
                // Configure management feature to use EF Core.
                elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));

                elsa.UseWorkflowRuntime(runtime =>
                {
                    runtime.UseEntityFrameworkCore();
                });

                elsa.UseJavaScript();
                elsa.UseLiquid();

                // Expose API endpoints.
                elsa.UseWorkflowsApi();

                // Add services for HTTP activities and workflow middleware.
                elsa.UseHttp(http =>
                {
                    http.ConfigureHttpOptions = options => options.BasePath = "/wf";
                });

                // Use timers.
                elsa.UseScheduling();

                // Configure identity so that we can create a default admin user.
                elsa.UseIdentity(identity =>
                {
                    identity.UseAdminUserProvider();
                    identity.TokenOptions = options =>
                    {
                        options.SigningKey = "secret-token-signing-key";
                        options.AccessTokenLifetime = TimeSpan.FromDays(1);
                    };
                });

                // Use default authentication (JWT).
                elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());

                // Register custom activities.
                elsa.AddActivitiesFrom<Program>();
            });

            // Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
            builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            // Add services to the container.
            builder.Services.AddControllers();
            // Add Razor pages.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWorkflowsApi();
            app.UseWorkflows();
            app.MapControllers();
            app.MapRazorPages();
            app.Run();
        }
    }
}