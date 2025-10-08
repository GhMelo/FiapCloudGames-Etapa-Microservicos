using Application.Input.AuthInput;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_Cloud_Games.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService
        ) : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuarioLoginInput usuario)
        {
            var usuarioLoginToken = authService.FazerLogin(usuario);

            if (usuarioLoginToken != string.Empty)
            {
                return Ok(usuarioLoginToken);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

