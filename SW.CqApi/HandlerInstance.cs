using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi
{
    class HandlerInstance
    {
        public object Instance { get; set; }

        public MethodInfo Method { get; set; }


        public Task<object> Invoke(params object[] parameters)
        {
            return (Task<object>)Method.Invoke(Instance, parameters);
        }

    }
}
