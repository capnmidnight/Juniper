using System;

namespace Juniper.VeldridIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IndexTypeAttribute : Attribute
    {
        public Type Value { get; }

        public IndexTypeAttribute(Type value)
        {
            Value = value;
        }
    }
}
