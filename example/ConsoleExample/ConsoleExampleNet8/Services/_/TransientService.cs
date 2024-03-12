using Simple.Attributes;

namespace ConsoleExampleNet8.Services {
    [Transient]
    public class TransientService : ITransientService {
        public string Get() {
            return "Get Transient Simple Service";
        }
    }
}
