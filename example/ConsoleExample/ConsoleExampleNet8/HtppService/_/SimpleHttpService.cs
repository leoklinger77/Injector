namespace ConsoleExampleNet8.HtppService {
    using Simple.Attributes;

    [HttpClient]
    public class SimpleHttpService : ISimpleHttpService {

        private readonly HttpClient _httpClient;

        public SimpleHttpService(HttpClient httpClient) {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.google.com.br/");
        }

        public async Task GetGoogle() {
            try {
                var request = await _httpClient.GetAsync("/search?q=imagem&sca_esv=95f056e3806c3e9f&hl=pt-BR&authuser=0&tbm=isch&sxsrf=ACQVn0_-RlNFnRBYFgucwnDAhSj3tHHsYg%3A1710204094143&source=hp&biw=1886&bih=909&ei=vqTvZcuQBaPI1sQPxsqnmAU&iflsig=ANes7DEAAAAAZe-yzu86m2P0kslvu3mCsWVufEVN58sR&udm=&ved=0ahUKEwjL55Xcvu2EAxUjpJUCHUblCVMQ4dUDCA8&uact=5&oq=imagem&gs_lp=EgNpbWciBmltYWdlbTIIEAAYgAQYsQMyCBAAGIAEGLEDMggQABiABBixAzIIEAAYgAQYsQMyCBAAGIAEGLEDMgUQABiABDIIEAAYgAQYsQMyCBAAGIAEGLEDMggQABiABBixAzIFEAAYgARI-QpQ4wVYuglwAXgAkAEAmAF2oAGKBaoBAzAuNrgBA8gBAPgBAYoCC2d3cy13aXotaW1nmAIHoAKmBagCCsICBxAjGOoCGCfCAgQQIxgnmAMIkgcDMS42oAeFHw&sclient=img");
                if (!request.IsSuccessStatusCode) {
                    throw new Exception("Faleired");
                }
            } catch (Exception ex) {
                throw;
            }
        }
    }
}
