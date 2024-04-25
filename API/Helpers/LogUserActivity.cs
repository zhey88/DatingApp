using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        //take a look at an action filter, and this means we can do something with the request 
        //even before it's executed by an endpoint or after it's been executed by our endpoint
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //this result context gives us the ActionExecutedContext,means the API action has completed
            //and we're going to get the result context back from this.
            //If we wanted to do something before this action, then we could use the ActionExecutingContext
            //We're going to wait until the API has done its job and then we're going to do something 
            //in order to update the last active property inside the user.
            var resultContext = await next();

            //Make sure the user is authenticated
            //gives us access to the HTTP context so we can do things like get a hold of our user's username
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            //get userId extension method, get the userId with TokenService and ClaimsPrincipalExtensions
            var userId = resultContext.HttpContext.User.GetUserId();
            //to get access to our repository because we're going to update something for our user
            //get hold of our services as well so that we can update this particular property
            var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            //get a hold of our user
            var user = await uow.UserRepository.GetUserByIdAsync(userId);
            //to update one property for the user that we get from our repository inside here
            user.LastActive = DateTime.UtcNow;
            //update our database
            await uow.Complete();
        }
    }
}