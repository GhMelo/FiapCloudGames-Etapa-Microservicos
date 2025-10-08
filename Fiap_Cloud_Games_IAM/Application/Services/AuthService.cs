using Application.Input.AuthInput;
using Application.Interfaces.IService;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService(
        IConfiguration configuration,
        HttpClient HttpClient,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository,
        IUsuarioRepository usuarioRepository
        ) : IAuthService
    {
        public string FazerLogin(UsuarioLoginInput usuario)
        {
            var usuarioLogin = usuarioRepository.BuscarUm(x=>x.Nome == usuario.Nome);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuarioLogin, "usuario logado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            if (usuarioLogin != null 
                && (usuario.Nome == usuarioLogin.Nome && usuario.Senha == usuarioLogin.Senha))
            {
                if (usuarioLogin.Tipo == TipoUsuario.Administrador)
                {
                    var token = GenerateToken(usuarioLogin.ExternalId, "Administrador", usuarioLogin.Email);
                    return token;
                }
                else
                {
                    var token = GenerateToken(usuarioLogin.ExternalId, "UsuarioPadrao", usuarioLogin.Email);
                    return token;
                }
            }
            else
            {
                return string.Empty;
            }
        }
        private string GenerateToken(Guid externalId, string role, string email)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, externalId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
