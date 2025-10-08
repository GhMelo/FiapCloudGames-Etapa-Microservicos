using Application.Dtos;
using Application.Input.UsuarioBibliotecaInput;
using Application.Interfaces.IService;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UsuarioBibliotecaService(
        IUsuarioBibliotecaRepository usuarioBibliotecaRepository,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository,
        IUsuarioRepository usuarioRepository,
        HttpClient httpClient,
        IConfiguration configuration
        ) : IUsuarioBibliotecaService
    {
        string SERVERLESS_FUNCTION_URL = configuration["ServelessEmail:ServelessEmail"]!;
        public Task CadastrarUsuarioBiblioteca(UsuarioBibliotecaCadastroInput usuarioBibliotecaCadastroInput)
        {
            var usuario = usuarioRepository.BuscarUm(x => x.ExternalId == usuarioBibliotecaCadastroInput.UsuarioExternalId);

            var usuarioBiblioteca = new UsuarioBiblioteca(usuario!.Id, usuarioBibliotecaCadastroInput.JogoExternalId);

            usuarioBibliotecaRepository.Cadastrar(usuarioBiblioteca);

            var subject = Uri.EscapeDataString("Parabéns pela sua compra!");
            var body = Uri.EscapeDataString($"Olá, {usuario.Nome}! 🎉\n\nSeu jogo ja esta adicionado a sua biblioteca");

            var queryString = $"SendEmailFunction?to={usuario.Email}&subject={subject}&body={body}";
            var url = SERVERLESS_FUNCTION_URL + queryString;
            
            var teste = httpClient.PostAsync(url, null);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuarioBiblioteca.Id, "jogo adicionado ao usuario",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            return Task.CompletedTask;
        }
        public UsuarioBibliotecaDto ObterUsuarioBibliotecaPorId(int id)
        {
            var usuarioBiblioteca = usuarioBibliotecaRepository.ObterPorId(id);
            var usuarioBibliotecaDto = new UsuarioBibliotecaDto()
            {
                Id = usuarioBiblioteca.Id,
                UsuarioId = usuarioBiblioteca.UsuarioId,
                JogoExternalId = usuarioBiblioteca.JogoExternalId,
                DataCriacao = usuarioBiblioteca.DataCriacao,
                UsuarioBiblioteca = new UsuarioDto()
                {
                    Nome = usuarioBiblioteca.Usuario.Nome,
                    Email = usuarioBiblioteca.Usuario.Email,
                    Id = usuarioBiblioteca.Usuario.Id,
                    ExternalId = usuarioBiblioteca.Usuario.ExternalId,
                    Senha = usuarioBiblioteca.Usuario.Senha,
                    DataCriacao = usuarioBiblioteca.Usuario.DataCriacao,
                    Tipo = usuarioBiblioteca.Usuario.Tipo
                }
            };
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(usuarioBibliotecaDto.Id, "biblitoeca obtida",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return usuarioBibliotecaDto;
        }

        public IEnumerable<UsuarioBibliotecaDto> ObterUsuarioBibliotecaPorJogoExternalId(Guid JogoExternalId)
        {
            var bibliotecaFiltrada = usuarioBibliotecaRepository.Buscar(x => x.JogoExternalId == JogoExternalId);

            var bibliotecaDto = bibliotecaFiltrada.Select(ub => new UsuarioBibliotecaDto()
            {
                Id = ub.Id,
                UsuarioId = ub.UsuarioId,
                JogoExternalId = ub.JogoExternalId,
                DataCriacao = ub.DataCriacao,
                UsuarioBiblioteca = new UsuarioDto()
                {
                    Id = ub.Usuario.Id,
                    Nome = ub.Usuario.Nome,
                    Email = ub.Usuario.Email,
                    Senha = ub.Usuario.Senha,
                    Tipo = ub.Usuario.Tipo,
                    ExternalId = ub.Usuario.ExternalId
                }
            }).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(bibliotecaDto.Count, "biblitoeca obtida",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return bibliotecaDto;
        }

        public IEnumerable<UsuarioBibliotecaDto> ObterUsuarioBibliotecaPorUsuarioId(int id)
        {
            var bibliotecaFiltrada = usuarioBibliotecaRepository.Buscar(x => x.UsuarioId == id);

            var bibliotecaDto = bibliotecaFiltrada.Select(ub => new UsuarioBibliotecaDto()
            {
                Id = ub.Id,
                UsuarioId = ub.UsuarioId,
                JogoExternalId = ub.JogoExternalId,
                DataCriacao = ub.DataCriacao,
                UsuarioBiblioteca = new UsuarioDto()
                {
                    Id = ub.Usuario.Id,
                    Nome = ub.Usuario.Nome,
                    Email = ub.Usuario.Email,
                    Senha = ub.Usuario.Senha,
                    Tipo = ub.Usuario.Tipo,
                    ExternalId = ub.Usuario.ExternalId
                }
            }).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(bibliotecaDto.Count, "biblitoeca obtida",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return bibliotecaDto;
        }
    }
}
