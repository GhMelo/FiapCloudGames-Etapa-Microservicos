using Application.Input.CompraInput;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_Cloud_Games.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class CompraController(
        ICompraService compraService
    ) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var comprasDto = compraService.ObterTodosComprasDto();
                return Ok(comprasDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/CompraPorId/{id:int}")]
        public IActionResult GetCompraPorId([FromRoute] int id)
        {
            try
            {
                var compraDto = compraService.ObterCompraDtoPorId(id);
                return Ok(compraDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/CompraPorStatus/{status}")]
        public IActionResult GetCompraPorStatus([FromRoute] string status)
        {
            try
            {
                var compraDto = compraService.ObterCompraDtoPorStatus(status);
                return Ok(compraDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/verificar_compra")]
        public IActionResult Post([FromBody] CompraVerificacaoInput input)
        {
            try
            {
                compraService.VerificarCompra(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] CompraCadastroInput input)
        {
            try
            {
                compraService.CadastrarCompra(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] CompraAlteracaoInput input)
        {
            try
            {
                compraService.AlterarCompra(input);
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
                compraService.DeletarCompra(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
