using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi
{
    internal class HandlerInfo
    {
        //public bool VoidReturnType;
        public Type HandlerType { get; set; }
        public MethodInfo Method { get; set; }
        public IList<Type> ArgumentTypes { get; set; }
        //public string Name { get; set; }
        public string Key { get; set; }
        public string Resource { get; set; }

        public Type NormalizedInterfaceType { get; set; }

    }
}
