using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;

namespace Company.Utility.Workflow.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var elsaSection = builder.Configuration.GetSection("Elsa");

            // Elsa services.
            builder.Services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    .AddConsoleActivities()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddEmailActivities(elsaSection.GetSection("Smtp").Bind)
                    .AddQuartzTemporalActivities()
                    .AddJavaScriptActivities()
                    .AddWorkflowsFrom<Startup>()
                    .AddActivitiesFrom<ReadQueryString>()
                    .AddWorkflow<EchoQueryStringWorkflow>()
                );

            // Elsa API endpoints.
            builder.Services.AddElsaApiEndpoints();
            builder.Services.AddElsaSwagger();

            // For Dashboard.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseHttpActivities();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();




            // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
            app.MapControllers();

            // // For Dashboard.
            // app.MapFallbackToPage("/_Host");



            app.Run();
        }
    }
}