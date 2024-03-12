using Microsoft.Extensions.Hosting;
using Simple.Attributes;

namespace ConsoleExampleNet8.Background {
    [Background]
    public class ExampleBackgroundService : BackgroundService {
        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            return Task.CompletedTask;
        }
    }
}
