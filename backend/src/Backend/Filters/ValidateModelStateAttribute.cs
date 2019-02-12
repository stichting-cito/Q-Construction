using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Citolab.QConstruction.Backend.Filters
{
    /// <summary>
    ///     Validate modelstate
    /// </summary>
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Executed before each action is done.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            //Ignore some properties because they are set in the 
            actionContext.ModelState.Remove("LastModified");
            actionContext.ModelState.Remove("LastModifiedBy");
            if (actionContext.HttpContext.Request.Method.ToUpper() == "POST")
            {
                actionContext.ModelState.Remove("Id");
                actionContext.ModelState.Remove("CreatedBy");
                actionContext.ModelState.Remove("Created");
            }
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Result = new BadRequestObjectResult(actionContext.ModelState);
            }
        }
    }
}