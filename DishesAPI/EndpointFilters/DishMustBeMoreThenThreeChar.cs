
using DishesAPI.Models;
using MiniValidation;

namespace DishesAPI.EndpointFilters
{
    public class DishMustBeMoreThenThreeChar : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dishForCreationDto = context.GetArgument<DishForCreationDto>(2);

            if(!MiniValidator.TryValidate(dishForCreationDto, out var error))
            {
                return TypedResults.ValidationProblem(error);
            }
            var result = await next.Invoke(context);

            return result;
        }
    }
}
