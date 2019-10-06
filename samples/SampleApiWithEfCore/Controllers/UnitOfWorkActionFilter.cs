using Microsoft.AspNetCore.Mvc.Filters;

namespace SampleApiWithEfCore.Controllers
{
    public class UnitOfWorkActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // ending the transaction
            throw new System.NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // start the transaction
            throw new System.NotImplementedException();
        }
    }
}