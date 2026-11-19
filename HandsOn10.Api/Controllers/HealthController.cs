using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace HandsOn10.Api.Controllers;
[ApiController][Route("api/health")]
public sealed class HealthController : ControllerBase {
 [HttpGet][AllowAnonymous] public IActionResult Get()=>Ok(new {status="OK",projeto="HANDS-ON #10 | Segurança de APIs REST"});
}
