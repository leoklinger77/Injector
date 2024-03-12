using Simple.Attributes;

namespace ConsoleExampleNet8.Services {

    [Singleton]
    public class SingletonService : ISingletonService {        
        public string Get() {
            return "Get Singleton Simple Service";
        }
    }
}
