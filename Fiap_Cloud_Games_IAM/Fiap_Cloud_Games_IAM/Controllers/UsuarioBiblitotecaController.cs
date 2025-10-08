using Application.Input.UsuarioBibliotecaInput;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace Fiap_Cloud_Games_IAM.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UsuarioBibliotecaController(
            IUsuarioBibliotecaService usuarioBibliotecaService
        ) : ControllerBase
    {
        [HttpGet("/UsuarioBibliotecaPorId/{id:int}")]
        public IActionResult GetUsuarioPorId([FromRoute] int id)
        {
            try
            {
                var usuarioBibliotecaDto = usuarioBibliotecaService.ObterUsuarioBibliotecaPorId(id);
                return Ok(usuarioBibliotecaDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/UsuarioBibliotecaPorUsuarioId/{id:int}")]
        public IActionResult GetUsuarioBibliotecaPorUsuarioId([FromRoute] int id)
        {
            try
            {
                var usuario = usuarioBibliotecaService.ObterUsuarioBibliotecaPorUsuarioId(id);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/UsuarioBibliotecaPorJogoExternalId/{jogoExternalId:Guid}")]
        public IActionResult GetUsuarioBibliotecaPorUsuarioId([FromRoute] Guid jogoExternalId)
        {
            try
            {
                var usuario = usuarioBibliotecaService.ObterUsuarioBibliotecaPorJogoExternalId(jogoExternalId);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/UsuarioBibliotecaPorUsuarioId")]
        public IActionResult PostUsuarioBiblioteca([FromBody] UsuarioBibliotecaCadastroInput usuarioBibliotecaCadastroInput)
        {
            try
            {
                usuarioBibliotecaService.CadastrarUsuarioBiblioteca(usuarioBibliotecaCadastroInput);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
