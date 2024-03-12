namespace Simple.Attributes {
    using System;
    public class SingletonAttribute : SimpleInjectorAttribute {
        public SingletonAttribute(Type @interface = null) : base(@interface) { }
    }
}