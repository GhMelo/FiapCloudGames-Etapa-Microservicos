using Application.Dtos;
using Application.Input.UsuarioInput;

namespace Application.Interfaces.IService
{
    public interface IUsuarioService
    {
        UsuarioDto ObterUsuarioDtoPorNome(string nome);
        UsuarioDto ObterUsuarioDtoPorId(int id);
        IEnumerable<UsuarioDto> ObterTodosUsuariosDto();
        void CadastrarUsuarioPadrao(UsuarioCadastroInput usuarioCadastroInput);
        void AlterarUsuario(UsuarioAlteracaoInput usuarioAlteracaoInput);
        void DeletarUsuario(int id);
    }
}
