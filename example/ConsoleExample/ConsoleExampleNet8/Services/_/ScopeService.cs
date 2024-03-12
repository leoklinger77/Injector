using Simple.Attributes;

namespace ConsoleExampleNet8.Services {

    [Scope]
    public class ScopeService : IScopeService {
        public string Get() {
            return "Get Scope Simple Service";
        }
    }
}
