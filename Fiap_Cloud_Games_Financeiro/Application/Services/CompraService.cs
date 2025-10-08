using Application.Dtos;
using Application.Input.CompraInput;
using Application.Interfaces.IService;
using Domain.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Domain.Events;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class CompraService(
        ICompraRepository compraRepository,
        IPagamentoRepository pagamentoRepository,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository,
        IConfiguration configuration
    ) : ICompraService
    {
        string SERVERLESS_FUNCTION_URL = configuration["ServelessEmail:ServelessEmail"]!;
        string GATEWAY_URL = configuration["Urls:Gateway"]!;

        public void AlterarCompra(CompraAlteracaoInput compraAlteracaoInput)
        {
            var compra = compraRepository.ObterPorId(compraAlteracaoInput.Id);

            compra.Status = compraAlteracaoInput.Status;
            compra.ValorCompra = compraAlteracaoInput.ValorCompra;

            compraRepository.Alterar(compra);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(compra, "Compra alterada",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public async Task CadastrarCompra(CompraCadastroInput compraCadastroInput)
        {
            var compra = new Compra(
                compraCadastroInput.UsuarioExternalId,
                compraCadastroInput.JogoExternalId,
                compraCadastroInput.ValorCompra,
                compraCadastroInput.Status,
                compraCadastroInput.PromocaoExternalId
            );

            compraRepository.Cadastrar(compra);

            var email = ObterEmailDoToken();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(compra, "Compra cadastrada",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            if (!string.IsNullOrEmpty(email))
            {
                var queryString = $"SendEmailFunction?to={email}&subject=Confirmação%20de%20Compra&body=Sua%20compra%20foi%20realizada%20com%20sucesso!";
                var url = SERVERLESS_FUNCTION_URL + queryString;

                var response = await httpClient.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Falha ao enviar email: {response.StatusCode}");
                }
            }
        }

        public void DeletarCompra(int id)
        {
            compraRepository.Deletar(id);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(id, "Compra deletada",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public IEnumerable<CompraDto> ObterTodosComprasDto()
        {
            var todasCompras = compraRepository.ObterTodos();
            try
            {

                eventStoreRepository.SalvarEventoAsync(new DomainEvent(todasCompras.Count(), "Compras obtidas",
                    httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
                return todasCompras.Select(c => new CompraDto
                {
                    Id = c.Id,
                    DataCriacao = c.DataCriacao,
                    Status = c.Status,
                    ValorCompra = c.ValorCompra,
                    UsuarioExternalId = c.UsuarioExternalId,
                    JogoExternalId = c.JogoExternalId,
                    PromocaoExternalId = c.PromocaoExternalId,
                    Pagamentos = c.Pagamentos.Select(p => new PagamentoDto
                    {
                        Id = p.Id,
                        CompraId = p.CompraId,
                        ValorPago = p.ValorPago,
                        DataPagamento = p.DataPagamento,
                        Metodo = p.Metodo
                    }).ToList()
                }).ToList();
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public CompraDto ObterCompraDtoPorId(int id)
        {
            var compra = compraRepository.ObterPorId(id);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(compra, "Compras obtidas",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return new CompraDto
            {
                Id = compra.Id,
                DataCriacao = compra.DataCriacao,
                Status = compra.Status,
                ValorCompra = compra.ValorCompra,
                UsuarioExternalId = compra.UsuarioExternalId,
                JogoExternalId = compra.JogoExternalId,
                PromocaoExternalId = compra.PromocaoExternalId,
                Pagamentos = compra.Pagamentos.Select(p => new PagamentoDto
                {
                    Id = p.Id,
                    CompraId = p.CompraId,
                    ValorPago = p.ValorPago,
                    DataPagamento = p.DataPagamento,
                    Metodo = p.Metodo
                }).ToList()
            };
        }

        public CompraDto ObterCompraDtoPorStatus(string status)
        {
            var compra = compraRepository.BuscarUm(c => c.Status == status);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(compra, "Compras obtidas",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return new CompraDto
            {
                Id = compra!.Id,
                DataCriacao = compra.DataCriacao,
                Status = compra.Status,
                ValorCompra = compra.ValorCompra,
                UsuarioExternalId = compra.UsuarioExternalId,
                JogoExternalId = compra.JogoExternalId,
                PromocaoExternalId = compra.PromocaoExternalId,
                Pagamentos = compra.Pagamentos.Select(p => new PagamentoDto
                {
                    Id = p.Id,
                    CompraId = p.CompraId,
                    ValorPago = p.ValorPago,
                    DataPagamento = p.DataPagamento,
                    Metodo = p.Metodo
                }).ToList()
            };
        }
        private string? ObterEmailDoToken()
        {
            var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader)) return null;

            var token = authHeader.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var emailClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email || c.Type == "email");
            return emailClaim.ToString();
        }

        public async Task VerificarCompra(CompraVerificacaoInput compraVerificacaoInput)
        {
            var compraVerificacao = compraRepository.BuscarUm(x => x.Id == compraVerificacaoInput.compraId);
            if (compraVerificacao is null)
                throw new Exception("Compra não encontrada.");

            var pagamentos = pagamentoRepository.Buscar(x => x.CompraId == compraVerificacao.Id && x.Status == "Processado");

            var totalPago = pagamentos.Sum(p => p.ValorPago);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(compraVerificacao.Id, "Compras verificada",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            if (totalPago >= compraVerificacao.ValorCompra)
            {
                compraVerificacao.Status = "Pago";
                compraRepository.Alterar(compraVerificacao);

                var usuarioBibliotecaInput = new 
                {
                    UsuarioExternalId = compraVerificacao.UsuarioExternalId, 
                    JogoExternalId = compraVerificacao.JogoExternalId      
                };

                var urlIam = GATEWAY_URL + "iam/UsuarioBibliotecaPorUsuarioId";
                var json = JsonSerializer.Serialize(usuarioBibliotecaInput);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseIam = httpClient.PostAsync(urlIam, content);
            }
        }
    }
}
