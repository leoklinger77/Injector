using Simple.Attributes;

namespace ConsoleExampleNet8.Services {

    [Singleton]
    public class DontInterfaceService {
        public string Get() {
            return "Get dont interface Simple Service";
        }
    }
}
