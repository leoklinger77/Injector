using Simple.Attributes;

namespace ConsoleExampleNet8.HtppService {

    [HttpClient(
        @interface: typeof(IMessageHandlerHttpService),
        name: "Google2",
        baseUrl: "https://www.google.com.br/",
        handler: typeof(HttpServiceMessageHandler))]
    public class MessageHandlerHttpService : IMessageHandlerHttpService {

        private readonly HttpClient _httpClient;

        public MessageHandlerHttpService(IHttpClientFactory factory) {
            _httpClient = factory.CreateClient("Google2");
        }

        public async Task GetGoogle() {
            try {
                var request = await _httpClient.GetAsync("api/v1/ExternalAuthentication/end-point");
                if (!request.IsSuccessStatusCode) {
                    throw new Exception("Faleired");
                }
            } catch (Exception ex) {

                throw;
            }
        }
    }
}
