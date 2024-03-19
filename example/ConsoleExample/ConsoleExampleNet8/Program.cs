using ConsoleExampleNet8.HtppService;
using ConsoleExampleNet8.Services;
using Simple;
using Simple.Attributes;

namespace ConsoleExampleNet8 {
    internal class Program {
        static void Main(string[] args) {
            Injection.Initialize(build: true);

            var singleton = Injection.GetService<Example>();
            singleton.Run();
        }
    }

    [Singleton]
    public class Example {

        private readonly IScopeService _scopeService;
        private readonly ISimpleHttpService _httpService;
        public Example(IScopeService scopeService, ISimpleHttpService httpService) {
            _scopeService = scopeService;
            _httpService = httpService;
        }

        public void Run() {
            Console.WriteLine(_scopeService.Get());

            _httpService.GetGoogle().Wait();
        }
    }
}
