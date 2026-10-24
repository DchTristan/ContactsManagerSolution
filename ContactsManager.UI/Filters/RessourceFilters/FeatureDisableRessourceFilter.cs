using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.RessourceFilters
{
    public class FeatureDisableRessourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisableRessourceFilter> _logger;
        private readonly bool _isDisabled;

        public FeatureDisableRessourceFilter(ILogger<FeatureDisableRessourceFilter> logger, bool isDisabled = true)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //TO DO : before logic
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(FeatureDisableRessourceFilter),
                nameof(OnResourceExecutionAsync));

            if(_isDisabled)
            {
                //context.Result = new NotFoundResult();

                context.Result = new StatusCodeResult(501);
            }
            else
            {
                await next();
            }

            

            //TO DO : after logic
            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(FeatureDisableRessourceFilter),
                nameof(OnResourceExecutionAsync));
        }
    }
}
