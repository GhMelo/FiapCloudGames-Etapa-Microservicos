using Application.Dtos;
using Application.Input.PromocaoInput;
using Application.Interfaces.IService;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class PromocaoService(
            IPromocaoRepository promocaoRepository,
            HttpClient HttpClient,
            IHttpContextAccessor httpContextAccessor,
            IEventStoreRepository eventStoreRepository
        ) : IPromocaoService
    {
        public void AlterarPromocao(PromocaoAlteracaoInput promocaoAlteracaoInput)
        {
            var promocaoAlteracao = promocaoRepository.ObterPorId(promocaoAlteracaoInput.PromocaoId);
            promocaoAlteracao.NomePromocao = promocaoAlteracaoInput.NomePromocao;
            promocaoAlteracao.PorcentagemDesconto = promocaoAlteracaoInput.PorcentagemDesconto;
            promocaoAlteracao.DataInicio = promocaoAlteracaoInput.DataInicio;
            promocaoAlteracao.DataFim = promocaoAlteracaoInput.DataFim;
            promocaoAlteracao.JogoId = promocaoAlteracaoInput.JogoId;
            promocaoAlteracao.Status = promocaoAlteracaoInput.Status;
            promocaoRepository.Alterar(promocaoAlteracao);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(promocaoAlteracao, "promocaoAlteracao alterada",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public void CadastrarPromocao(PromocaoCadastroInput promocaoCadastroInput)
        {
            var promocaoCadastro = new Promocao(promocaoCadastroInput.JogoId,
                promocaoCadastroInput.NomePromocao,
                promocaoCadastroInput.PorcentagemDesconto,
                promocaoCadastroInput.DataInicio,
                promocaoCadastroInput.DataFim,
                promocaoCadastroInput.Status);
            promocaoRepository.Cadastrar(promocaoCadastro); 
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(
                promocaoCadastro,
                "promocao cadastrada",
                httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()
            ));
        }

        public void DeletarPromocao(int id)
        {
            var promocao = promocaoRepository.ObterPorId(id);
            promocaoRepository.Deletar(id);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(
               promocao,
               "promocao deletada",
               httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()
           ));
        }

        public PromocaoDto ObterPromocaoDtoPorId(int id)
        {
            var promocaoBd = promocaoRepository.ObterPorId(id);
            var promocaoDto = new PromocaoDto
            {
                Id = promocaoBd.Id,
                NomePromocao = promocaoBd.NomePromocao,
                JogoId = promocaoBd.JogoId,
                PorcentagemDesconto = promocaoBd.PorcentagemDesconto,
                DataInicio = promocaoBd.DataInicio,
                DataFim = promocaoBd.DataFim,
                ExternalId = promocaoBd.ExternalId,
                Status = promocaoBd.Status,
                JogoPromocao = new JogoDto
                {
                    Id = promocaoBd.JogoPromocao.Id,
                    Nome = promocaoBd.JogoPromocao.Nome,
                    Produtora = promocaoBd.JogoPromocao.Produtora,
                    Valor = promocaoBd.JogoPromocao.Valor,
                    ExternalId = promocaoBd.JogoPromocao.ExternalId
                }
            };
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(
               promocaoDto.NomePromocao,
               "promocao obtida",
               httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()
           ));
            return promocaoDto;
        }

        public PromocaoDto ObterPromocaoDtoPorNomePromocao(string nomePromocao)
        {
            var promocaoBd = promocaoRepository.BuscarUm(x=>x.NomePromocao == nomePromocao);
            var promocaoDto = new PromocaoDto
            {
                Id = promocaoBd!.Id,
                NomePromocao = promocaoBd.NomePromocao,
                JogoId = promocaoBd.JogoId,
                PorcentagemDesconto = promocaoBd.PorcentagemDesconto,
                DataInicio = promocaoBd.DataInicio,
                DataFim = promocaoBd.DataFim,
                ExternalId = promocaoBd.ExternalId,
                Status = promocaoBd.Status,
                JogoPromocao = new JogoDto
                {
                    Id = promocaoBd.JogoPromocao.Id,
                    Nome = promocaoBd.JogoPromocao.Nome,
                    Produtora = promocaoBd.JogoPromocao.Produtora,
                    Valor = promocaoBd.JogoPromocao.Valor,
                    ExternalId = promocaoBd.JogoPromocao.ExternalId
                }
            };
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(
               promocaoDto.NomePromocao,
               "promocao obtida",
               httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()
           ));
            return promocaoDto;
        }

        public IEnumerable<PromocaoDto> ObterTodosPromocaoDto()
        {
            var promocaoBd = promocaoRepository.ObterTodos();
            var promocaoDto = new List<PromocaoDto>();
            promocaoDto = promocaoBd.Select(p => new PromocaoDto
            {
                Id = p!.Id,
                NomePromocao = p.NomePromocao,
                JogoId = p.JogoId,
                PorcentagemDesconto = p.PorcentagemDesconto,
                DataInicio = p.DataInicio,
                DataFim = p.DataFim,
                ExternalId = p.ExternalId,
                Status = p.Status,
                JogoPromocao = new JogoDto
                {
                    Id = p.JogoPromocao.Id,
                    Nome = p.JogoPromocao.Nome,
                    Produtora = p.JogoPromocao.Produtora,
                    Valor = p.JogoPromocao.Valor,
                    ExternalId = p.JogoPromocao.ExternalId
                }
            }).ToList();
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(
               promocaoDto.Count,
               "promocao obtida",
               httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()
           ));
            return promocaoDto;
        }
    }
}
