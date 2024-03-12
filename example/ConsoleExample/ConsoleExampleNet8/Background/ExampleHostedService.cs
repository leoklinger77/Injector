using Microsoft.Extensions.Hosting;
using Simple.Attributes;

namespace ConsoleExampleNet8.Background {

    [Background]
    public class ExampleHostedService : IHostedService, IDisposable {
        public void Dispose() {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }
    }
}
