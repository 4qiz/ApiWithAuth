using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthWithRoles.Controllers.v1
{
    //[Authorize(Roles = "User")]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Open method for all users
        /// </summary>
        /// <response code="201">Sample response</response>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("User functionality");
        }
    }
}
