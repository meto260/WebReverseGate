public class RequestCatcher {
    private readonly RequestDelegate _next;

    public RequestCatcher(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        context.Request.EnableBuffering();
        var savedata = new SaveData();
        try {
            var sw = new Stopwatch();
            sw.Start();
            var reqId = Guid.NewGuid();
            var requestMethod = context.Request.Method;
            var requestUri = context.Request.GetDisplayUrl();

            using var ms = new MemoryStream();
            await context.Request.Body.CopyToAsync(ms);
            string requestBody = Encoding.UTF8.GetString(ms.ToArray());
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            var requestHeader = JsonSerializer.Serialize(context.Request.Headers);
            var requestCookies = JsonSerializer.Serialize(context.Request.Cookies);
            var requestBytesLength = Encoding.UTF8.GetByteCount(requestBody);
            var requestContentType = context.Request.ContentType;
            var requestHttps = context.Request.IsHttps;
            var requestTrailers = context.Request.CheckTrailersAvailable();
            var requestIp = context.Connection.RemoteIpAddress?.ToString() ?? "";
            var trace = context.TraceIdentifier;

            // Call the next delegate/middleware in the pipeline.
            await _next(context);

            try {
                var responseHeaders = context.Response.Headers;
                var responseCookies = context.Response.Cookies;
                var responseStatus = context.Response.StatusCode;
                var responseContentType = context.Response.ContentType;
                await savedata.ResSave(new ResModel {
                    reqId = reqId,
                    responseHeaders = responseHeaders.Count > 0 ? JsonSerializer.Serialize(responseHeaders) : "",
                    responseCookies = JsonSerializer.Serialize(responseCookies),
                    responseStatus = responseStatus
                });
            }
            catch (Exception exc) {
                await savedata.ExceptionSave(exc.Message);
            }
            var reqElapsed = sw.Elapsed.TotalMilliseconds;
            await savedata.ReqSave(new ReqModel {
                reqId = reqId,
                requestMethod = requestMethod,
                requestUri = requestUri,
                requestBody = requestBody,
                requestHeader = requestHeader,
                requestCookies = requestCookies,
                requestBytesLength = requestBytesLength,
                requestContentType = requestContentType ?? "",
                requestHttps = requestHttps,
                requestTrailers = requestTrailers,
                trace = trace,
                requestElapsed = reqElapsed,
                clientIp = requestIp
            });

            sw.Stop();
        }
        catch (Exception exc) {
            await savedata.ExceptionSave(exc.Message);
        }
    }
}
public static class RequestCatcherMiddlewareExtensions {
    /// <summary>
    /// method for capturing requests flowing through
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseRequestCatcher(
        this IApplicationBuilder builder) {
        return builder.UseMiddleware<RequestCatcher>();
    }
}