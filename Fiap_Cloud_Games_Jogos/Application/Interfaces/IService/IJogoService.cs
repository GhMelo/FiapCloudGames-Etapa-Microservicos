using Application.Dtos;
using Application.Input.JogoInput;
using Domain.Entities;

namespace Application.Interfaces.IService
{
    public interface IJogoService
    {
        IEnumerable<JogoDto> ObterTodosJogosDto();
        JogoDto ObterJogoDtoPorTitulo(string titulo);
        JogoDto ObterJogoDtoPorId(int id);
        Task JogoCompra(JogoCompraInput jogoCompra);
        void CadastrarJogo(JogoCadastroInput jogoCadastroInput);
        void AlterarJogo(JogoAlteracaoInput jogoAlteracaoInput);
        void DeletarJogo(int id);
        public Task<IEnumerable<Jogo>> BuscarJogosRecomendadosPorGenero(Guid usuarioExternalId);
    }
}
