using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;

namespace Company.Utility.Workflow.Service
{
    [Activity("Demo", "Demo", Description = "A demo activity for parsing query strings.")]
    public class ParseQueryString : CodeActivity<string>
    {
        protected override void Execute(ActivityExecutionContext context)
        {
            var httpContextAccessor = context.GetRequiredService<IHttpContextAccessor>();
            var query = httpContextAccessor.HttpContext!.Request.Query;
            var items = query!.Select(x => $"{x.Key}: {x.Value}");
            context.SetResult(string.Join(", ", items));
        }
    }
}