using Simple.Attributes;

namespace ApiExampleNet8.Services {
    [Transient]
    public class TransientService : ITransientService {

        public string Get() {
            return "Get Transient Simple Service";
        }
    }
}
