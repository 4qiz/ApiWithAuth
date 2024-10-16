using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthWith2Fa.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("asdf");
        }

        [HttpGet("with-custom-auth-check")]
        public IActionResult GetWithCheck()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                // Возвращаем кастомный ответ с данными
                return Unauthorized(new { error = "не авторизован", status = false, data = "дополнительные данные" });
            }
            return Ok("asdf");
        }
    }
}
