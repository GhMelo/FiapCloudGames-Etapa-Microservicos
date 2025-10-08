using Application.Dtos;
using Application.Input.CompraInput;

namespace Application.Interfaces.IService
{
    public interface ICompraService
    {
        CompraDto ObterCompraDtoPorStatus(string status);
        CompraDto ObterCompraDtoPorId(int id);
        IEnumerable<CompraDto> ObterTodosComprasDto();
        Task VerificarCompra(CompraVerificacaoInput compraVerificacaoInput);
        Task CadastrarCompra(CompraCadastroInput compraCadastroInput);
        void AlterarCompra(CompraAlteracaoInput compraAlteracaoInput);
        void DeletarCompra(int id);
    }
}
