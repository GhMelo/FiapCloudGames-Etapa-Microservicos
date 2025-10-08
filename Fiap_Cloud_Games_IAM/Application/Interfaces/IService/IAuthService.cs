using Application.Input.AuthInput;

namespace Application.Interfaces.IService
{
    public interface IAuthService
    {
        public string FazerLogin(UsuarioLoginInput usuario);
    }
}
