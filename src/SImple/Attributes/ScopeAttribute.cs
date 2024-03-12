namespace Simple.Attributes {
    using System;
    public class ScopeAttribute : SimpleInjectorAttribute {
        public ScopeAttribute(Type @interface = null) : base(@interface) { }
    }
}
