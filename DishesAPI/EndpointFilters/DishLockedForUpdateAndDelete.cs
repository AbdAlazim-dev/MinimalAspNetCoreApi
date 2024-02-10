using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace DishesAPI.EndpointFilters
{
    public class DishLockedForUpdateAndDelete : IEndpointFilter
    {
        private readonly IEnumerable<Guid> _ids;
        public DishLockedForUpdateAndDelete(IEnumerable<Guid> ids)
        {
            _ids = ids;
        }
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var requestDishToEdit = new Guid();
            string methodVerb = "Edit";

            if (context.HttpContext.Request.Method == "PUT")
            {
                requestDishToEdit = context.GetArgument<Guid>(2);
            } 
            else
            {
                requestDishToEdit = context.GetArgument<Guid>(1);
                methodVerb = "Delete";
            }
            var secondDishId = new Guid("eacc5169-b2a7-41ad-92c3-dbb1a5e7af06");
            foreach(Guid id in _ids)
            {
                if (id == requestDishToEdit)
                {
                    return TypedResults.Problem(new()
                    {
                        Status = 400,
                        Title = "this dish is perfect and cant be "+ methodVerb,
                        Detail = "you can not "+ methodVerb +" perfection"
                    });
                }
            }
            var result = await next.Invoke(context);

            return result;
        }
    }
}
