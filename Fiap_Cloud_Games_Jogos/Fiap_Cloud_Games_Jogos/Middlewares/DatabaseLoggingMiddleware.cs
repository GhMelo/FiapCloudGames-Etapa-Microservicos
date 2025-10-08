using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Domain.Entity;
using Domain.Interfaces.IRepository;

namespace FIAP_Cloud_Games.Middlewares
{
    public class DatabaseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public DatabaseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IMongoRepository<LogRequest> logRepository)
        {
            var sw = Stopwatch.StartNew();
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

            context.Request.EnableBuffering();
            string bodyContent = string.Empty;

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                context.Request.Body.Position = 0;
                bodyContent = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var originalBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            string errorMessage = string.Empty;
            int statusCode;

            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;

                if (statusCode >= 400)
                {
                    if (statusCode == 401)
                    {
                        errorMessage = "Não autorizado";
                    }
                    else
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                        responseBody.Seek(0, SeekOrigin.Begin);

                        try
                        {
                            var json = JsonDocument.Parse(responseText);

                            if (json.RootElement.TryGetProperty("errors", out var errorsElement))
                            {
                                var messages = new List<string>();

                                foreach (var property in errorsElement.EnumerateObject())
                                {
                                    var field = property.Name;
                                    foreach (var message in property.Value.EnumerateArray())
                                    {
                                        messages.Add($"{field}: {message.GetString()}");
                                    }
                                }

                                errorMessage = string.Join(" | ", messages);
                            }
                            else
                            {
                                errorMessage = responseText;
                            }
                        }
                        catch
                        {
                            errorMessage = responseText;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                statusCode = 500;
                errorMessage = ex.ToString();

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Erro interno no servidor.");
            }

            sw.Stop();

            var log = new LogRequest
            {
                CorrelationId = correlationId,
                Path = context.Request.Host + context.Request.Path,
                Method = context.Request.Method,
                StatusCode = statusCode,
                Body = bodyContent,
                Timestamp = DateTime.UtcNow,
                ExecutionTimeMs = sw.ElapsedMilliseconds,
                ErrorMessage = errorMessage != string.Empty ? Truncate(errorMessage, 1000) : errorMessage
            };

            try
            {
                await logRepository.InsertOneAsync(log);
            }
            catch
            {
                // Ignorar errors de loggin em si
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }

        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}