namespace ConsoleExampleNet8.HtppService {
    public class HttpServiceMessageHandler : DelegatingHandler {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
