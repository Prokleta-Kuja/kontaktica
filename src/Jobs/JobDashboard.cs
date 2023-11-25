using Hangfire;
using Hangfire.Dashboard;

namespace kontaktica.Jobs;

public static class JobDashboard
{
    public static void UseJobDashboard(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard(C.Routes.Jobs, new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthFilter() },
            DashboardTitle = "kontaktica jobs",
            DisplayStorageConnectionString = false,
        });
    }
}

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var ctx = context.GetHttpContext();
        return ctx.DynDnsCheck();
    }
}