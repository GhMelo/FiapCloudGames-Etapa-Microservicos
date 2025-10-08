using Application.Dtos;
using Application.Input.JogoInput;
using Application.Interfaces.IService;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Application.Services
{
    public class JogoService(
            IJogoRepository jogoRepository,
            HttpClient HttpClient,
            IHttpContextAccessor httpContextAccessor,
            IEventStoreRepository eventStoreRepository,
            IConfiguration configuration
        ) : IJogoService
    {
        string GATEWAY_URL = configuration["Urls:Gateway"]!;
        string ELASTIC_URL = "https://fiap-cloud-games-elastic-search-e0f3e2.es.eastus.azure.elastic.cloud";
        string ELASTIC_API_KEY = "cVlrUWc1a0I1VWNjTWNfZC1CRnU6dDA4VlNqOUJkeWZFWDY5WEQzRDVodw==";

        public void AlterarJogo(JogoAlteracaoInput jogoAlteracaoInput)
        {
            var jogo = jogoRepository.ObterPorId(jogoAlteracaoInput.Id);
            jogo.Nome = jogoAlteracaoInput.Nome;
            jogo.Produtora = jogoAlteracaoInput.Produtora;
            jogo.Valor = jogoAlteracaoInput.Valor;
            jogo.Genero = jogoAlteracaoInput.Genero;
            jogoRepository.Alterar(jogo);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(jogo, "Jogo criado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            EnviarJogoParaElastic(jogo);
        }

        public void CadastrarJogo(JogoCadastroInput jogoCadastroInput)
        {
            var jogo = new Jogo(jogoCadastroInput.Nome, jogoCadastroInput.Produtora, jogoCadastroInput.Valor, jogoCadastroInput.Genero);

            jogoRepository.Cadastrar(jogo);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(jogo, "Jogo alterado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            EnviarJogoParaElastic(jogo);

        }

        public void DeletarJogo(int id)
        {
            jogoRepository.Deletar(id);
            eventStoreRepository.SalvarEventoAsync(new DomainEvent(id, "Jogo Removido",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
        }

        public async Task JogoCompra(JogoCompraInput jogoCompra)
        {
            var url = GATEWAY_URL + "financeiro/Compra";

            var json = JsonSerializer.Serialize(jogoCompra);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await eventStoreRepository.SalvarEventoAsync(new DomainEvent(jogoCompra, "Jogo comprado",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            var response = await HttpClient.PostAsync(url, content);

            await EnviarCompraParaElastic(jogoCompra);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao enviar JogoCompra. Status: {response.StatusCode}");
            }
        }
        public async Task EnviarCompraParaElastic(JogoCompraInput jogoCompra)
        {
            var url = $"{ELASTIC_URL}/compras/_doc/" + Guid.NewGuid().ToString();

            var compraElastic = new
            {
                usuarioExternalId = jogoCompra.UsuarioExternalId,
                jogoExternalId = jogoCompra.JogoExternalId,
                promocaoExternalId = jogoCompra.PromocaoExternalId,
                valorCompra = jogoCompra.ValorCompra,
                status = jogoCompra.Status
            };

            var json = JsonSerializer.Serialize(compraElastic);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("ApiKey", ELASTIC_API_KEY);

            var response = await HttpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao enviar compra para Elasticsearch. Status: {response.StatusCode}");
            }
        }
        public JogoDto ObterJogoDtoPorId(int id)
        {
            var jogo = jogoRepository.ObterPorId(id);
            var jogoDto = new JogoDto();

            jogoDto.Id = jogo.Id;
            jogoDto.Nome = jogo.Nome;
            jogoDto.Produtora = jogo.Produtora;
            jogoDto.DataCriacao = jogo.DataCriacao;
            jogoDto.Valor = jogo.Valor;
            jogoDto.ExternalId = jogo.ExternalId;
            jogoDto.Genero = jogo.Genero;

            jogoDto.PromocoesAderidas = jogo.PromocoesAderidas.Select(pa => new PromocaoDto()
            {
                Id = pa.Id,
                DataInicio = pa.DataInicio,
                DataFim = pa.DataFim,
                JogoId = pa.JogoId,
                NomePromocao = pa.NomePromocao,
                PorcentagemDesconto = pa.PorcentagemDesconto,
                Status = pa.Status,
                ExternalId = pa.ExternalId
            }).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(jogoDto, "Jogos obtidos",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            return jogoDto;
        }
        public async Task EnviarJogoParaElastic(Jogo jogo)
        {
            var url = $"{ELASTIC_URL}/games/_doc/{jogo.Id}";

            var jogoElastic = new
            {
                id = jogo.Id,
                name = jogo.Nome,
                genre = jogo.Genero,
                developer = jogo.Produtora,
                externalId= jogo.ExternalId,
            };

            var json = JsonSerializer.Serialize(jogoElastic);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("ApiKey", ELASTIC_API_KEY);

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(jogo, "Jogos enviado para elastic",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            var response = await HttpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao enviar jogo para Elasticsearch. Status: {response.StatusCode}");
            }

        }
        public async Task<IEnumerable<Jogo>> BuscarJogosRecomendadosPorGenero(Guid usuarioExternalId)
        {

            var urlCompras = $"{ELASTIC_URL}/compras/_search";

            var queryCompras = new
            {
                query = new
                {
                    match = new
                    {
                        usuarioExternalId = usuarioExternalId
                    }
                }
            };

            var jsonCompras = JsonSerializer.Serialize(queryCompras);
            var contentCompras = new StringContent(jsonCompras, Encoding.UTF8, "application/json");

            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("ApiKey", ELASTIC_API_KEY);

            var responseCompras = await HttpClient.PostAsync(urlCompras, contentCompras);
            if (!responseCompras.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao buscar compras do usuário. Status: {responseCompras.StatusCode}");
            }

            var responseComprasJson = await responseCompras.Content.ReadAsStringAsync();
            using var docCompras = JsonDocument.Parse(responseComprasJson);
            var hitsCompras = docCompras.RootElement
                .GetProperty("hits")
                .GetProperty("hits")
                .EnumerateArray()
                .Select(h => h.GetProperty("_source").GetProperty("jogoExternalId").GetGuid());

            var generosComprados = new HashSet<string>();
            foreach (var jogoId in hitsCompras)
            {
                var urlJogo = $"{ELASTIC_URL}/games/_search";
                var queryJogo = new
                {
                    query = new
                    {
                        match = new { externalId = jogoId }
                    }
                };

                var jsonJogo = JsonSerializer.Serialize(queryJogo);
                var contentJogo = new StringContent(jsonJogo, Encoding.UTF8, "application/json");

                var responseJogo = await HttpClient.PostAsync(urlJogo, contentJogo);
                if (!responseJogo.IsSuccessStatusCode) continue;

                var responseJogoJson = await responseJogo.Content.ReadAsStringAsync();
                using var docJogo = JsonDocument.Parse(responseJogoJson);
                var hitsJogo = docJogo.RootElement.GetProperty("hits").GetProperty("hits").EnumerateArray();
                foreach (var hit in hitsJogo)
                {
                    var genero = hit.GetProperty("_source").GetProperty("genre").GetString();
                    if (!string.IsNullOrEmpty(genero))
                    {
                        generosComprados.Add(genero);
                    }
                }
            }

            var recomendados = new List<Jogo>();
            foreach (var genero in generosComprados)
            {
                var urlGenero = $"{ELASTIC_URL}/games/_search";
                var queryGenero = new
                {
                    query = new
                    {
                        match = new { genre = genero }
                    }
                };

                var jsonGenero = JsonSerializer.Serialize(queryGenero);
                var contentGenero = new StringContent(jsonGenero, Encoding.UTF8, "application/json");

                var responseGenero = await HttpClient.PostAsync(urlGenero, contentGenero);
                if (!responseGenero.IsSuccessStatusCode) continue;

                var responseGeneroJson = await responseGenero.Content.ReadAsStringAsync();
                using var docGenero = JsonDocument.Parse(responseGeneroJson);
                var hitsGenero = docGenero.RootElement.GetProperty("hits").GetProperty("hits").EnumerateArray();

                foreach (var hit in hitsGenero)
                {
                    var source = hit.GetProperty("_source");
                    recomendados.Add(new Jogo(source.GetProperty("name").GetString(),
                        source.GetProperty("developer").GetString(),
                        10,
                        source.GetProperty("genre").GetString())
                    {
                        Id = source.GetProperty("id").GetInt32(),
                        Nome = source.GetProperty("name").GetString(),
                        Genero = source.GetProperty("genre").GetString(),
                        Produtora = source.GetProperty("developer").GetString(),
                        ExternalId = source.GetProperty("externalId").GetGuid()
                    });
                }
            }

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(recomendados.Count, "Jogos recomendados encontrados",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));
            return recomendados;
        }
        public JogoDto ObterJogoDtoPorTitulo(string titulo)
        {

            var jogo = jogoRepository.BuscarUm(x=>x.Nome == titulo);

            var jogoDto = new JogoDto();

            jogoDto.Id = jogo!.Id;
            jogoDto.Nome = jogo.Nome;
            jogoDto.Produtora = jogo.Produtora;
            jogoDto.DataCriacao = jogo.DataCriacao;
            jogoDto.Valor = jogo.Valor;
            jogoDto.Genero = jogo.Genero!;
            jogoDto.ExternalId = jogo.ExternalId;

            jogoDto.PromocoesAderidas = jogo.PromocoesAderidas.Select(pa => new PromocaoDto()
            {
                Id = pa.Id,
                DataInicio = pa.DataInicio,
                DataFim = pa.DataFim,
                JogoId = pa.JogoId,
                NomePromocao = pa.NomePromocao,
                PorcentagemDesconto = pa.PorcentagemDesconto,
                Status = pa.Status,
                ExternalId = pa.ExternalId
            }).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(jogoDto, "Jogos encontrados",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            return jogoDto;
        }

        public IEnumerable<JogoDto> ObterTodosJogosDto()
        {
            var todosJogos = jogoRepository.ObterTodos();
            var jogoDto = new List<JogoDto>();
            jogoDto = todosJogos.Select(tj => new JogoDto()
            {
                Id = tj.Id,
                Nome = tj.Nome,
                Produtora = tj.Produtora,
                DataCriacao = tj.DataCriacao,
                ExternalId = tj.ExternalId,
                Genero = tj.Genero!,
                Valor = tj.Valor,
                PromocoesAderidas = tj.PromocoesAderidas?.Select(pa => new PromocaoDto()
                {
                    Id = pa.Id,
                    DataInicio = pa.DataInicio,
                    DataFim = pa.DataFim,
                    JogoId = pa.JogoId,
                    NomePromocao = pa.NomePromocao,
                    PorcentagemDesconto = pa.PorcentagemDesconto,
                    Status = pa.Status,
                    ExternalId = pa.ExternalId
                }).ToList() ?? new List<PromocaoDto>()
            }).ToList();

            eventStoreRepository.SalvarEventoAsync(new DomainEvent(jogoDto.Count, "Jogos encontrados",
                httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()));

            return jogoDto;
        }
    }
}
