using Application.Input.PagamentoInput;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_Cloud_Games.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class PagamentoController(
            IPagamentoService pagamentoService
        ) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var pagamentosDto = pagamentoService.ObterTodosPagamentosDto();
                return Ok(pagamentosDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/PagamentoPorId/{id:int}")]
        public IActionResult GetPagamentoPorId([FromRoute] int id)
        {
            try
            {
                var pagamentoDto = pagamentoService.ObterPagamentoDtoPorId(id);
                return Ok(pagamentoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/Pagamento")]
        public IActionResult PostPagamento([FromBody] PagamentoCadastroInput input)
        {
            try
            {
                pagamentoService.CadastrarPagamento(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] PagamentoAlteracaoInput input)
        {
            try
            {
                pagamentoService.AlterarPagamento(input);
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
                pagamentoService.DeletarPagamento(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
