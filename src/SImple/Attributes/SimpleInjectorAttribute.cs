namespace Simple.Attributes {
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class SimpleInjectorAttribute : Attribute {
        public Type Interface { get; }
        public SimpleInjectorAttribute(Type @interface = null) {
            Interface = @interface;
        }
    }
}