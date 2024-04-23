using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers{

    //specific about where we actually use the ActionFilter
    //we're checking, see if the user is authenticated inside the action filter
    //the way that we use our action filter is specify service filter
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]  //  /api/users
    public class BaseApiController : ControllerBase
    {
        
    }

}
