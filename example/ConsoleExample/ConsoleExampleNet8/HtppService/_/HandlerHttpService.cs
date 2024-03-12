using Simple.Attributes;

namespace ConsoleExampleNet8.HtppService {
    [HttpClient(name: "Google", handler: typeof(HttpServiceMessageHandler))]
    public class HandlerHttpService : IHandlerHttpService {

        private readonly HttpClient _httpClient;

        public HandlerHttpService(IHttpClientFactory factory) {

            _httpClient = factory.CreateClient("Google");

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
