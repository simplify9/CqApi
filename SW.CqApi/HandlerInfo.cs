using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SW.CqApi
{
    internal class HandlerInfo
    {
        //public bool VoidReturnType;
        public object Instance { get; set; }
        public Type HandlerType { get; set; }
        public MethodInfo Method { get; set; }
        public IList<Type> ArgumentTypes { get; set; }
        //public string Name { get; set; }
    }
}
