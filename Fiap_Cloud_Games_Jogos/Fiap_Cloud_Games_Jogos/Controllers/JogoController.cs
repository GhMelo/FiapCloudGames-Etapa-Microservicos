using Application.Input.JogoInput;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace Fiap_Cloud_Games_Jogos.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class JogoController(
        IJogoService jogoService
        ) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var todosJogosDto = jogoService.ObterTodosJogosDto();
                return Ok(todosJogosDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{guid:Guid}")]
        public async Task<IActionResult> GetCompras(Guid guid)
        {
            try
            {
                var todosJogosDto = await jogoService.BuscarJogosRecomendadosPorGenero(guid);
                return Ok(todosJogosDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/JogoPorId/{id:int}")]
        public IActionResult GetJogoPorId([FromRoute] int id)
        {
            try
            {
                var jogoDto = jogoService.ObterJogoDtoPorId(id);
                return Ok(jogoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/JogoPorTitulo/{titulo}")]
        public IActionResult GetJogoPorTitulo([FromRoute] string titulo)
        {
            try
            {
                var jogoDto = jogoService.ObterJogoDtoPorTitulo(titulo);
                return Ok(jogoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] JogoCadastroInput input)
        {
            try
            {
                jogoService.CadastrarJogo(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("compra-jogo")]
        public IActionResult PostComprarJogo([FromBody] JogoCompraInput input)
        {
            try
            {
                jogoService.JogoCompra(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public IActionResult Put([FromBody] JogoAlteracaoInput input)
        {
            try
            {
                jogoService.AlterarJogo(input);
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
                jogoService.DeletarJogo(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
