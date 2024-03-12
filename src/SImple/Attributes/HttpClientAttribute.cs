namespace Simple.Attributes {
    using System;
    public class HttpClientAttribute : SimpleInjectorAttribute {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public Type Handler { get; set; }
        public HttpClientAttribute(Type @interface = null, string name = null, string baseUrl = null, Type handler = null) : base(@interface) {
            Name = name;
            BaseUrl = baseUrl;
            Handler = handler;
        }
    }
}
