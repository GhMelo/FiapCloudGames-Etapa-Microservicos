using Application.Dtos;
using Application.Input.PagamentoInput;

namespace Application.Interfaces.IService
{
    public interface IPagamentoService
    {
        PagamentoDto ObterPagamentoDtoPorId(int id);
        IEnumerable<PagamentoDto> ObterTodosPagamentosDto();
        void CadastrarPagamento(PagamentoCadastroInput pagamentoCadastroInput);
        void AlterarPagamento(PagamentoAlteracaoInput pagamentoAlteracaoInput);
        void DeletarPagamento(int id);
    }
}
