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


        public async Task<object> Invoke(params object[] parameters)
        {
            var task = (Task)Method.Invoke(Instance, parameters);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task);
            // return (Task<object>)Method.Invoke(Instance, parameters);
        }

    }
}
