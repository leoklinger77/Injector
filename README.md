# Injector

# Document

# Get Start
-- install-package

### Web Application.


    var builder = WebApplication.CreateBuilder(args);            
    builder.Services.AddControllers();            
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();   
    
    //Add Simple Injection
    builder.Services.Initialize();           

    var app = builder.Build();    


### Console Application
    static void Main(string[] args) {
        //Add Simple Injection
        Injection.Initialize();
    }

Depois da inicialização do Injeção, caso precise fazer alguma configuração na Injeção de Dependencia, pode obter o ServiceCollection da seguinte maneira:

    Injection.Service()
        .AddScoped<IScopeService, ScopeService>();

Após realizar toda a configuração desejada, realize o build da seguinda maneira:

    // Injection Build
    Injection.Build();   


Para simplificar ainda mais, você pode informar que deseja fazer o build automáticamente, informando 'true' como parametro do initialize, dessa maneira, você não podera realizar configuração após a inicialição:

    Injection.Initialize(build: true);



## Importante
#### Caso não exista interface, será injetado o serviço da mesma forma. 
#### O Padrão esperado das interfaces começa com I e possui o mesmo nome da classe implementadora. Exemplo abaixo:

## Scope
Para adicionar um serviço do tipo Scope, basta colocar a annotation na implementação do serviço da seguinte maneira:

    //Interface
    public interface IScopeService {
        string Get();
    }

    //Add Annotation for Class implementation
    [Scope]
    public class ScopeService : IScopeService {
        public string Get() {
            return "Get Scope Simple Service";
        }
    }


## Singleton

    [Singleton]
    public class SingletonService : ISingletonService {        
        public string Get() {
            return "Get Singleton Simple Service";
        }
    }

## Transient
    [Transient]
    public class TransientService : ITransientService {
        public string Get() {
            return "Get Transient Simple Service";
        }
    }

## Backgroud
Você pode implementar um serviço em Backgroud utilizando a implementação do IHostedService ou BackgroundService

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
    
#### or 

    [Background]
    public class ExampleBackgroundService : BackgroundService {
        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            return Task.CompletedTask;
        }
    }

## HttpClient
    [HttpClient]
    public class SimpleHttpService : ISimpleHttpService {
        private readonly HttpClient _httpClient;
        public SimpleHttpService(HttpClient httpClient) {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.google.com.br/");
        }

        public async Task GetGoogle() {
            try {
                var request = await _httpClient.GetAsync("search");
                if (!request.IsSuccessStatusCode) {
                    throw new Exception("Faleired");
                }
            } catch (Exception ex) {

                throw;
            }
        }
    }

## Configurando HttpClient com MessageHandler
Para utilizar um MessageHandler, informe o Type da class que implementa um DelegatingHandler na assinatura 'handler' da seguinte forma:

    [HttpClient(handler: typeof(HttpServiceMessageHandler))]

Por default o nome do contexto de injeção do HttpClient sera o nome da class de implementação, exemplo:

    [HttpClient(handler: typeof(HttpServiceMessageHandler))]
    public class HandlerHttpService : ISimpleHttpService {
        
        private readonly HttpClient _httpClient;
        
        public HandlerHttpService(IHttpClientFactory factory) {
            _httpClient = factory.CreateClient(nameof(HandlerHttpService));
        }
    }

Caso queire customizar o nome, pode utilizar o parametro da assinature 'Name', da seguinte forma
        
    [HttpClient(name: "Google", handler: typeof(HttpServiceMessageHandler))]
    public class HandlerHttpService : ISimpleHttpService {
        
        private readonly HttpClient _httpClient;
        
        public HandlerHttpService(IHttpClientFactory factory) {
            _httpClient = factory.CreateClient("Google");
        }
    }


Voce também pode informar a Url Base do serviço, e caso o nome da interface não tenha o padrão .net, você também pode informa-lo da seguinte forma:


    [HttpClient(
    @interface: typeof(ISimpleHttpService), 
    name: "Google", 
    baseUrl: "https://www.google.com.br/", 
    handler: typeof(HttpServiceMessageHandler))]
    public class WithMessageHandlerHttpService : ISimpleHttpService {
        
        private readonly HttpClient _httpClient;
        
        public WithMessageHandlerHttpService(IHttpClientFactory factory) {
            _httpClient = factory.CreateClient("Google");
        }

        public async Task GetGoogle() {
            try {
                var request = await _httpClient.GetAsync("api/v1/ExternalAuthentication/end-point");
                if (!request.IsSuccessStatusCode) {
                    throw new Exception("Faleired");
                }
            } catch (Exception ex) {

                throw;
            }
        }
    }

    //Message Handler implement DelegationHandler
    public class HttpServiceMessageHandler : DelegatingHandler {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return base.SendAsync(request, cancellationToken);
        }
    }

