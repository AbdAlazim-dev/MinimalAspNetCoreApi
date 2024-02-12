
using Microsoft.AspNetCore.Http;

namespace DishesAPI.EndpointFilters;

public class LogNotFoundResponseFilter : IEndpointFilter
{
    private readonly ILogger<LogNotFoundResponseFilter> _logger;
    public LogNotFoundResponseFilter(ILogger<LogNotFoundResponseFilter> logger)
    {
        _logger = logger;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        //cast the result to hold the statusCode
        var actualResult = (result is INestedHttpResult) ? ((INestedHttpResult)result).Result :
            (IResult)result;

        if((actualResult as IStatusCodeHttpResult)?.StatusCode == (int)StatusCodes.Status404NotFound)
        {
            _logger.LogInformation($"Resourse {context.HttpContext.Request.Path} was not found.");
        }

        return result;
    }
}