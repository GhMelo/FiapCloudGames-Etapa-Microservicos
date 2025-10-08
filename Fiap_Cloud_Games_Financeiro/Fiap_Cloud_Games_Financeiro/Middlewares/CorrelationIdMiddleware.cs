namespace FIAP_Cloud_Games.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationIdHeader)
                || string.IsNullOrWhiteSpace(correlationIdHeader))
            {
                correlationIdHeader = Guid.NewGuid().ToString();
            }

            context.Items["CorrelationId"] = correlationIdHeader.ToString();

            if (!context.Response.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.Response.Headers.Add("X-Correlation-ID", correlationIdHeader.ToString());
            }

            await _next(context);
        }
    }
}
