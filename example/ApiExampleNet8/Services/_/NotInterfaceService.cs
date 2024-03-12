using Simple.Attributes;

namespace ApiExampleNet8.Services {

    [Singleton]
    public class NotInterfaceService {
        public string Get() {
            return "Get Not interface Simple Service";
        }
    }
}
