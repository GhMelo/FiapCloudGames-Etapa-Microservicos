using Application.Dtos;
using Application.Input.PagamentoInput;
using Application.Interfaces.IService;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class PagamentoService(
        IPagamentoRepository pagamentoRepository,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository
    ) : IPagamentoService
    {
        public void AlterarPagamento(PagamentoAlteracaoInput pagamentoAlteracaoInput)
        {
            var pagamento = pagamentoRepository.ObterPorId(pagamentoAlteracaoInput.Id);

            pagamento.ValorPago = pagamentoAlteracaoInput.ValorPago;
            pagamento.Metodo = pagamentoAlteracaoInput.Metodo;

            pagamentoRepository.Alterar(pagamento);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(pagamento, "pagamento alterada",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public void CadastrarPagamento(PagamentoCadastroInput pagamentoCadastroInput)
        {
            var pagamento = new Pagamento(
                pagamentoCadastroInput.CompraId,
                pagamentoCadastroInput.ValorPago,
                pagamentoCadastroInput.Metodo
            );

            pagamentoRepository.Cadastrar(pagamento);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(pagamento, "pagamento cadastrado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public void DeletarPagamento(int id)
        {
            pagamentoRepository.Deletar(id);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(id, "pagamento deletado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public IEnumerable<PagamentoDto> ObterTodosPagamentosDto()
        {
            var todosPagamentos = pagamentoRepository.ObterTodos();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(todosPagamentos.Count(), "pagamento obtido",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            return todosPagamentos.Select(p => new PagamentoDto
            {
                Id = p.Id,
                CompraId = p.CompraId,
                ValorPago = p.ValorPago,
                DataPagamento = p.DataPagamento,
                Metodo = p.Metodo
            }).ToList();
        }

        public PagamentoDto ObterPagamentoDtoPorId(int id)
        {
            var pagamento = pagamentoRepository.ObterPorId(id);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(pagamento, "pagamento obtido",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            return new PagamentoDto
            {
                Id = pagamento.Id,
                CompraId = pagamento.CompraId,
                ValorPago = pagamento.ValorPago,
                DataPagamento = pagamento.DataPagamento,
                Metodo = pagamento.Metodo
            };
        }
    }
}
