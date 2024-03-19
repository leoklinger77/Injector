namespace Simple {
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Simple.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Injection {
        private static readonly ILogger _log = LoggerFactory.Create(builder => {
            builder.AddConsole();
            builder.AddSimpleConsole(c => {
                c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                c.UseUtcTimestamp = true;
                c.SingleLine = true;
            });
        }).CreateLogger(typeof(Injection));

        private static IServiceCollection _serviceCollection;
        private static IServiceProvider _servicesProvider;

        private static readonly Type _background = typeof(BackgroundAttribute);
        private static readonly Type _simgleton = typeof(SingletonAttribute);
        private static readonly Type _transient = typeof(TransientAttribute);
        private static readonly Type _http = typeof(HttpClientAttribute);
        private static readonly Type _scope = typeof(ScopeAttribute);

        private static bool _isInitialize = false;
        private static bool _isBuilder = false;

        public static void Initialize(this IServiceCollection services, Action<ILoggingBuilder> logger = null) {
            if (_isInitialize) {
                return;
            }
            _log.LogInformation("Starting 'SimpleInjector' by extension method");
            _serviceCollection = services ?? throw new ArgumentNullException(nameof(services));

            InjectorServices(logger);
        }

        public static void Initialize(bool build = false, Action<ILoggingBuilder> logger = null) {
            if (_isInitialize) {
                return;
            }
            _log.LogInformation("Starting 'SimpleInjector'");
            _serviceCollection ??= new ServiceCollection();

            InjectorServices(logger);

            if (build) {
                Build();
            }
        }

        private static void InjectorServices(Action<ILoggingBuilder> logger) {
            if (logger != null) {
                _serviceCollection.AddLogging(logger);
            } else {
                _serviceCollection.AddLogging(builder => {
                    builder.AddSimpleConsole(c => {
                        c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                        c.UseUtcTimestamp = true;
                        c.SingleLine = true;
                    });
                    builder.SetMinimumLevel(LogLevel.Information);
                });
            }

            foreach (var item in GetAllAssemblies()) {
                ServicesInjector(item);
            }

            _isInitialize = true;
        }

        public static void Build() {
            if (_isInitialize) {
                if (!_isBuilder) {
                    _isBuilder = true;
                    _servicesProvider = _serviceCollection.BuildServiceProvider();
                    _log.LogInformation("Builder completed");
                } else {
                    throw new InvalidOperationException();
                }
            } else {
                throw new InvalidOperationException();
            }
        }

        public static IServiceCollection Service() {
            return _serviceCollection;
        }

        public static T GetService<T>() {
            return _servicesProvider.GetService<T>();
        }

        private static void ServicesInjector(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                Attribute attribute = type.GetCustomAttribute(_scope, true);
                if (attribute != null) {
                    AddScoped(type);
                    continue;
                }
                attribute = type.GetCustomAttribute(_simgleton, true);
                if (attribute != null) {
                    AddSingleton(type);
                    continue;
                }
                attribute = type.GetCustomAttribute(_transient, true);
                if (attribute != null) {
                    AddTransient(type);
                    continue;
                }
                attribute = type.GetCustomAttribute(_http, true);
                if (attribute != null) {
                    AddHttpClient(type);
                    continue;
                }
                attribute = type.GetCustomAttribute(_background, true);
                if (attribute != null) {
                    AddBackground(type);
                    continue;
                }
            }
        }

        private static void AddTransient(Type type) {
            var attributes = type.GetCustomAttribute<TransientAttribute>();
            Type interfaceType = GetInterface(type, attributes);

            if (interfaceType == null) {
                _serviceCollection.TryAddTransient(type);
                _log.LogInformation($"[ Transient       ] IMPLEMENT: {type.Name}");
            } else {
                _serviceCollection.TryAddTransient(interfaceType, type);
                _log.LogInformation($"[ Transient       ] IMPLEMENT: {type.Name} | INTERFACE: {interfaceType.Name}");
            }
        }

        private static void AddSingleton(Type type) {
            var attributes = type.GetCustomAttribute<SingletonAttribute>();
            Type interfaceType = GetInterface(type, attributes);

            if (interfaceType == null) {
                _serviceCollection.TryAddSingleton(type);
                _log.LogInformation($"[ Singleton       ] IMPLEMENT: {type.Name}");
            } else {
                _serviceCollection.TryAddSingleton(interfaceType, type);
                _log.LogInformation($"[ Singleton       ] IMPLEMENT: {type.Name} | INTERFACE: {interfaceType.Name}");
            }
        }

        private static void AddScoped(Type type) {
            var attributes = type.GetCustomAttribute<ScopeAttribute>();
            Type interfaceType = GetInterface(type, attributes);

            if (interfaceType == null) {
                _serviceCollection.TryAddScoped(type);
                _log.LogInformation($"[ Transient       ] IMPLEMENT: {type.Name}");
            } else {
                _serviceCollection.TryAddScoped(interfaceType, type);
                _log.LogInformation($"[ Transient       ] IMPLEMENT: {type.Name} | INTERFACE: {interfaceType.Name}");
            }
        }

        private static void AddHttpClient(Type type) {
            var attributes = type.GetCustomAttribute<HttpClientAttribute>();

            Type interfaceType = GetInterface(type, attributes);
            var name = attributes.Name ?? type.Name;

            _serviceCollection.AddHttpClient(name, (http) => {
                http.BaseAddress = string.IsNullOrEmpty(attributes.BaseUrl) ? null : new Uri(attributes.BaseUrl);
            }).AddHttpMessageHandler((provider) => (System.Net.Http.DelegatingHandler)Activator.CreateInstance(attributes.Handler));


            if (interfaceType == null) {
                _serviceCollection.TryAddTransient(type);
                _log.LogInformation($"[ HttpClient      ] IMPLEMENT: {type.Name} with Client name Http: {name}");
            } else {
                _serviceCollection.TryAddTransient(interfaceType, type);
                _log.LogInformation($"[ HttpClient      ] IMPLEMENT: {type.Name} | INTERFACE: {interfaceType.Name} with Client name Http: {name}");
            }

            if (attributes.Handler != null) {
                _serviceCollection.TryAddTransient(attributes.Handler);
                _log.LogInformation($"[ HandlerMessage  ] HANDLER: {attributes.Handler.Name} To IMPLEMENT: {type.Name}");
            }
        }

        private static void AddBackground(Type type) {
            _serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), type));
        }

        private static Type GetInterface(Type type, SimpleInjectorAttribute attribute) {
            Type interfaceType = attribute.Interface ?? type.GetInterface($"I{type.Name}");
            if (interfaceType == null) {
                var interfaces = type.GetInterfaces();
                if (interfaces.Length == 1) {
                    interfaceType = interfaces[0];
                }

                if (interfaces.Length >= 2) {

                }
            }
            return interfaceType;
        }

        private static IEnumerable<Assembly> GetAllAssemblies() {
            Assembly assembly = Assembly.GetEntryAssembly();
            List<Assembly> list = new List<Assembly>();
            if (assembly != null) {
                list.Add(assembly);
                list.AddRange(assembly.GetReferencedAssemblies().Select(Assembly.Load));
            }

            _log.LogInformation($"Get Assembly names: {string.Join(" | ", list)}");

            return list;
        }
    }
}