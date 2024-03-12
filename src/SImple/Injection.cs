namespace Simple {
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Simple.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Injection {
        private static IServiceCollection _serviceCollection;
        private static IServiceProvider _servicesProvider;

        private static readonly Type _background = typeof(BackgroundAttribute);
        private static readonly Type _simgleton = typeof(SingletonAttribute);
        private static readonly Type _transient = typeof(TransientAttribute);
        private static readonly Type _http = typeof(HttpClientAttribute);
        private static readonly Type _scope = typeof(ScopeAttribute);

        private static bool _isInitialize = false;

        public static void Initialize(this IServiceCollection services) {
            if (_isInitialize) {
                return;
            }
            _serviceCollection = services ?? throw new ArgumentNullException(nameof(services));
            Initialize();
        }

        public static void Initialize(bool build = false) {
            if (_isInitialize) {
                return;
            }
            _serviceCollection ??= new ServiceCollection();
            LoagindAssembly();
            _isInitialize = true;
            if (build) {
                Build();
            }
        }

        public static void Build() {
            if (_isInitialize) {
                _servicesProvider = _serviceCollection.BuildServiceProvider();
            }
        }

        public static IServiceCollection Service() => _serviceCollection;

        public static T GetService<T>() {
            return _servicesProvider.GetService<T>();
        }

        private static void LoagindAssembly() {
            foreach (var item in GetAllAssemblies()) {
                ServicesInjector(item);
            }
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
            Type interfaceType = attributes.Interface ?? type.GetInterface($"I{type.Name}");

            if (interfaceType == null) {
                _serviceCollection.TryAddTransient(type);
            } else {
                _serviceCollection.TryAddTransient(interfaceType, type);
            }
        }

        private static void AddSingleton(Type type) {
            var attributes = type.GetCustomAttribute<SingletonAttribute>();
            Type interfaceType = attributes.Interface ?? type.GetInterface($"I{type.Name}");

            if (interfaceType == null) {
                _serviceCollection.TryAddSingleton(type);
            } else {
                _serviceCollection.TryAddSingleton(interfaceType, type);
            }
        }

        private static void AddScoped(Type type) {
            var attributes = type.GetCustomAttribute<ScopeAttribute>();
            Type interfaceType = attributes.Interface ?? type.GetInterface($"I{type.Name}");

            if (interfaceType == null) {
                _serviceCollection.TryAddScoped(type);
            } else {
                _serviceCollection.TryAddScoped(interfaceType, type);
            }
        }

        private static void AddHttpClient(Type type) {
            var attributes = type.GetCustomAttribute<HttpClientAttribute>();

            Type interfaceType = attributes.Interface ?? type.GetInterface($"I{type.Name}");            
            var name = attributes.Name ?? type.Name;

            _serviceCollection.AddHttpClient(name, (http) => {
                http.BaseAddress = string.IsNullOrEmpty(attributes.BaseUrl) ? null : new Uri(attributes.BaseUrl);
            }).AddHttpMessageHandler((provider) => (System.Net.Http.DelegatingHandler)Activator.CreateInstance(attributes.Handler));

            if (attributes.Handler != null) {
                _serviceCollection.TryAddTransient(attributes.Handler);
            }

            if (interfaceType == null) {
                _serviceCollection.TryAddTransient(type);
            } else {
                _serviceCollection.TryAddTransient(interfaceType, type);
            }
        }

        private static void AddBackground(Type type) {
            _serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), type));
        }

        private static IEnumerable<Assembly> GetAllAssemblies() {
            Assembly assembly = Assembly.GetEntryAssembly();
            List<Assembly> list = new List<Assembly>();
            if (assembly != null) {
                list.Add(assembly);
                list.AddRange(assembly.GetReferencedAssemblies().Select(Assembly.Load));
            }
            return list;
        }
    }
}