using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AuthAuthEasyLib.Atributes
{
    // TODO: IMPLEMENT "RequireAuth" Attribute
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireAuthAttribute: ActionFilterAttribute
    {

        public RequireAuthAttribute()
        {
            throw new NotImplementedException();
        }



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
        }
    }
}
