using Application.Dtos;
using Application.Input.UsuarioInput;
using Application.Interfaces.IService;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class UsuarioService(
        IUsuarioRepository usuarioRepository,
        HttpClient HttpClient,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository
        ) : IUsuarioService
    {
        public void AlterarUsuario(UsuarioAlteracaoInput usuarioAlteracaoInput)
        {
            var usuario = usuarioRepository.ObterPorId(usuarioAlteracaoInput.Id);
            usuario.Nome = usuarioAlteracaoInput.Nome;
            usuario.Tipo = usuarioAlteracaoInput.Tipo;
            usuario.Email = usuarioAlteracaoInput.Email;
            usuario.Senha = usuarioAlteracaoInput.Senha!;
            usuarioRepository.Alterar(usuario);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuario, "usuario alterado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public void CadastrarUsuarioPadrao(UsuarioCadastroInput UsuarioCadastroInput)
        {
            var usuario = new Usuario(UsuarioCadastroInput.Email, UsuarioCadastroInput.Nome, UsuarioCadastroInput.Senha, UsuarioCadastroInput.Tipo);
            usuarioRepository.Cadastrar(usuario);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuario, "usuario cadastrado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public void DeletarUsuario(int id)
        {
            usuarioRepository.Deletar(id);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(id, "usuario deletado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public IEnumerable<UsuarioDto> ObterTodosUsuariosDto()
        {
            var todosUsuarios = usuarioRepository.ObterTodos();
            var usuariosDto = new List<UsuarioDto>();
            usuariosDto = todosUsuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                DataCriacao = u.DataCriacao,
                Tipo = u.Tipo,
                Nome = u.Nome,
                Email = u.Email,
                Senha = u.Senha,
                ExternalId = u.ExternalId,

                Biblioteca = u.Biblioteca.Select(uj => new UsuarioBibliotecaDto()
                {
                    Id = uj.Id,
                    DataCriacao = uj.DataCriacao,
                    UsuarioId = uj.UsuarioId,
                    JogoExternalId = uj.JogoExternalId
                }).ToList()
            }
            ).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuariosDto.Count, "usuario obtido",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return usuariosDto;
        }

        public UsuarioDto ObterUsuarioDtoPorId(int id)
        {
            var usuario = usuarioRepository.ObterPorId(id);
            var usuarioDto = new UsuarioDto();

            usuarioDto.Id = usuario.Id;
            usuarioDto.DataCriacao = usuario.DataCriacao;
            usuarioDto.Tipo = usuario.Tipo;
            usuarioDto.Nome = usuario.Nome;
            usuarioDto.Email = usuario.Email;
            usuarioDto.Senha = usuario.Senha;
            usuarioDto.ExternalId = usuario.ExternalId;
            usuarioDto.Biblioteca = usuario.Biblioteca.Select(j => new UsuarioBibliotecaDto
            {
                Id = j.Id,
                DataCriacao = j.DataCriacao,
                UsuarioId = j.UsuarioId,
                JogoExternalId = j.JogoExternalId
            }).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuarioDto, "usuario obtido",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return usuarioDto;
        }

        public UsuarioDto ObterUsuarioDtoPorNome(string nome)
        {
            var usuario = usuarioRepository.BuscarUm(x=>x.Nome == nome);
            var usuarioDto = new UsuarioDto();

            usuarioDto.Id = usuario!.Id;
            usuarioDto.DataCriacao = usuario.DataCriacao;
            usuarioDto.Tipo = usuario.Tipo;
            usuarioDto.Nome = usuario.Nome;
            usuarioDto.Email = usuario.Email;
            usuarioDto.Senha = usuario.Senha;
            usuarioDto.ExternalId = usuario.ExternalId;
            usuarioDto.Biblioteca = usuario.Biblioteca.Select(j => new UsuarioBibliotecaDto
            {
                Id = j.Id,
                DataCriacao = j.DataCriacao,
                UsuarioId = j.UsuarioId,
                JogoExternalId = j.JogoExternalId
            }).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuarioDto, "usuario obtido",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return usuarioDto;
        }
    }
}
