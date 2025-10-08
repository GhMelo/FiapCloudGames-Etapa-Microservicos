using Application.Input.PromocaoInput;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace Fiap_Cloud_Games_Jogos.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class PromocaoController(
        IPromocaoService promocaoService
        ) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var todosPromocaoDto = promocaoService.ObterTodosPromocaoDto();
                return Ok(todosPromocaoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/PromocaoPorId/{id:int}")]
        public IActionResult GetPromocaoPorId([FromRoute] int id)
        {
            try
            {
                var promocaoDto = promocaoService.ObterPromocaoDtoPorId(id);
                return Ok(promocaoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/PromocaoPorNomePromocao/{nomePromocao}")]
        public IActionResult GetPromocaoPorNomePromocao([FromRoute] string nomePromocao)
        {
            try
            {
                var promocaoDto = promocaoService.ObterPromocaoDtoPorNomePromocao(nomePromocao);
                return Ok(promocaoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] PromocaoCadastroInput input)
        {
            try
            {
                promocaoService.CadastrarPromocao(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] PromocaoAlteracaoInput input)
        {
            try
            {
                promocaoService.AlterarPromocao(input);
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
                promocaoService.DeletarPromocao(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
