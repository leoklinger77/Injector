# Injector

The objective of this package is to simplify the development of .NET Core projects, providing additional ease to the developer when instantiating services in Dependency Injection. Its implementation aims to free the developer from concerns related to service configurations, allowing him to focus fully on the development and implementation of functionalities.

The code in this package goes through all the project's assemblies, identifying all Annotations Classes and automatically instantiating them in the ServiceCollection. This eliminates the need for manual intervention by the developer, providing a more efficient and hassle-free approach to configuring services.

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

After Injection initialization, if you need to perform any configuration in Dependency Injection, you can obtain the ServiceCollection as follows:

    Injection.Service()
        .AddScoped<IScopeService, ScopeService>();

After carrying out all the desired configuration, carry out the build as follows:

    // Injection Build
    Injection.Build();   


To simplify it even further, you can inform that you want to build automatically, entering 'true' as the initialization parameter, this way, you cannot perform the configuration after initialization:

    Injection.Initialize(build: true);



## Important
#### If there is no interface, the service will be injected in the same way.
#### The expected Pattern of interfaces starts with I and has the same name as the implementing class. Example below:

## Scope
To add a Scope service, simply place the annotation in the service implementation as follows:

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
To add singleton

    [Singleton]
    public class SingletonService : ISingletonService {        
        public string Get() {
            return "Get Singleton Simple Service";
        }
    }

## Transient
To add transient

    [Transient]
    public class TransientService : ITransientService {
        public string Get() {
            return "Get Transient Simple Service";
        }
    }

## Background
You can implement a service in Background using the implementation of IHostedService or BackgroundService

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
To use a MessageHandler, enter the Type of the class that implements a DelegatingHandler in the 'handler' signature as follows:

    [HttpClient(handler: typeof(HttpServiceMessageHandler))]

By default, the name of the HttpClient injection context will be the name of the implementation class, as follows:

    [HttpClient(handler: typeof(HttpServiceMessageHandler))]
    public class HandlerHttpService : ISimpleHttpService {
        
        private readonly HttpClient _httpClient;
        
        public HandlerHttpService(IHttpClientFactory factory) {
            _httpClient = factory.CreateClient(nameof(HandlerHttpService));
        }
    }

If you want to customize the name, you can use the 'name' signature parameter, as follows:
        
    [HttpClient(name: "Google", handler: typeof(HttpServiceMessageHandler))]
    public class HandlerHttpService : ISimpleHttpService {
        
        private readonly HttpClient _httpClient;
        
        public HandlerHttpService(IHttpClientFactory factory) {
            _httpClient = factory.CreateClient("Google");
        }
    }


You can also enter the Base Url of the service, and if the interface name does not have the .net standard, you can also enter it as follows:


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

