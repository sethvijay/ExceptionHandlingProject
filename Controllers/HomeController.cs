using ExceptionHandlingProject.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionHandlingProject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    [ServiceFilter(typeof(ExceptionFilter))]
    public IActionResult Get()
    {
        throw new Exception("Exception in Home Controller.");
    }
    //https://medium.com/@mbektas0506/exception-handling-using-filters-in-asp-net-core-57eaca7053a4
    //https://medium.com/@mbektas0506/exception-handling-using-middlewares-in-asp-net-core-b325668c4c5d
}
