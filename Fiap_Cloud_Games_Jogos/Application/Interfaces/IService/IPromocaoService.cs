using Application.Dtos;
using Application.Input.PromocaoInput;

namespace Application.Interfaces.IService
{
    public interface IPromocaoService
    {
        IEnumerable<PromocaoDto> ObterTodosPromocaoDto();
        PromocaoDto ObterPromocaoDtoPorNomePromocao(string nomePromocao);
        PromocaoDto ObterPromocaoDtoPorId(int id);
        void CadastrarPromocao(PromocaoCadastroInput promocaoCadastroInput);
        void AlterarPromocao(PromocaoAlteracaoInput promocaoAlteracaoInput);
        void DeletarPromocao(int id);
    }
}
