using Application.Input.UsuarioInput;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_Cloud_Games.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UsuarioController(
            IUsuarioService usuarioService
        ) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var usuariosDto = usuarioService.ObterTodosUsuariosDto();
                return Ok(usuariosDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/UsuarioPorId/{id:int}")]
        public IActionResult GetUsuarioPorId([FromRoute] int id)
        {
            try
            {
                var usuarioDto = usuarioService.ObterUsuarioDtoPorId(id);
                return Ok(usuarioDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/UsuarioPorNome/{nome}")]
        public IActionResult GetUsuarioPorNome([FromRoute] string nome)
        {
            try
            {
                var usuario = usuarioService.ObterUsuarioDtoPorNome(nome);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/UsuarioPadrao")]
        public IActionResult PostUsuarioPadrao([FromBody] UsuarioCadastroInput input)
        {
            try
            {
                usuarioService.CadastrarUsuarioPadrao(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] UsuarioAlteracaoInput input)
        {
            try
            {
                usuarioService.AlterarUsuario(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                usuarioService.DeletarUsuario(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
