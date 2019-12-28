using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class HandlerNameAttribute : Attribute
    {
        public HandlerNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }
    }
}
