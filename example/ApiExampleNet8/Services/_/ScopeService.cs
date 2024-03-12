using Simple.Attributes;

namespace ApiExampleNet8.Services {

    [Scope]
    public class ScopeService : IScopeService {
        public string Get() {
            return "Get Scope Simple Service";
        }
    }
}
