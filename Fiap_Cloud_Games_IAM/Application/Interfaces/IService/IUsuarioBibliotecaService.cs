using Application.Dtos;
using Application.Input.UsuarioBibliotecaInput;

namespace Application.Interfaces.IService
{
    public interface IUsuarioBibliotecaService
    {
        Task CadastrarUsuarioBiblioteca(UsuarioBibliotecaCadastroInput usuarioBibliotecaCadastroInput);
        UsuarioBibliotecaDto ObterUsuarioBibliotecaPorId(int id);
        IEnumerable<UsuarioBibliotecaDto> ObterUsuarioBibliotecaPorJogoExternalId(Guid JogoExternalId);
        IEnumerable<UsuarioBibliotecaDto> ObterUsuarioBibliotecaPorUsuarioId(int id);
    }
}
