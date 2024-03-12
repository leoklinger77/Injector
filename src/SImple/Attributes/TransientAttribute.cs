namespace Simple.Attributes {
    using System;
    public class TransientAttribute : SimpleInjectorAttribute {
        public TransientAttribute(Type @interface = null) : base(@interface) { }
    }
}