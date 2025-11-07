using Ganss.Xss;
using System.Text;
using System.Text.Json;

namespace TaskManagment.Middlewares
{
    public class InputValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<InputValidationMiddleware> _logger;
        private readonly HtmlSanitizer _sanitizer;

        public InputValidationMiddleware(RequestDelegate next, ILogger<InputValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _sanitizer = new HtmlSanitizer();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentLength > 0 &&
                (context.Request.Method == HttpMethods.Post ||
                 context.Request.Method == HttpMethods.Put ||
                 context.Request.Method == HttpMethods.Patch))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(body);
                        if (ContainsMaliciousContent(jsonDoc.RootElement))
                        {
                            _logger.LogWarning("Blocked XSS attempt on {Path}", context.Request.Path);

                            var result = Results.BadRequest(new
                            {
                                error = "Malicious HTML or script detected.",
                                path = context.Request.Path,
                                time = DateTime.UtcNow
                            });

                            await result.ExecuteAsync(context);
                            return;
                        }
                    }
                    catch (JsonException)
                    {
                        _logger.LogWarning("Skipped non-JSON body on {Path}", context.Request.Path);
                    }
                }
            }

            await _next(context);
        }

        private bool ContainsMaliciousContent(JsonElement element)
        {
            // payload checking
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        if (ContainsMaliciousContent(prop.Value)) return true;
                    }
                    break;
                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        if (ContainsMaliciousContent(item)) return true;
                    }
                    break;
                case JsonValueKind.String:
                    return IsMalicious(element);
            }
            return false;
        }

        private bool IsMalicious(JsonElement element)
        {
            var original = element.GetString() ?? string.Empty;
            var sanitized = _sanitizer.Sanitize(original);
            return !string.Equals(original, sanitized, StringComparison.Ordinal);
        }
    }
}
