using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;

namespace Company.Utility.Workflow.Service
{
    public class ParseQueryString : Activity
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParseQueryString(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var query = _httpContextAccessor.HttpContext!.Request.Query;
            var items = query!.Select(x => $"<li>{x.Key}: {x.Value}</li>");
            context.SetVariable("ParsedResult", $"<ul>{string.Join("\n", items)}</ul>");
            return Done();
        }
    }
}