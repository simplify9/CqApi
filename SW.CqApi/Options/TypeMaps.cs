using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi.Options
{
    public class TypeMaps
    {
        private class TypedExample
        {
            public Type Type { get; set; }
            public object Example { get; set; }
        }
        private Dictionary<Type, TypedExample> Maps { get; } = new Dictionary<Type, TypedExample>();
        public void AddMap<T1, T2>(T2 example)
        {
            Maps.Add(typeof(T1), new TypedExample{
                Type = typeof(T2),
                Example = example
            });
        }
        public void AddMap<T1, T2>()
        {
            Maps.Add(typeof(T1), new TypedExample
            {
                Type = typeof(T2)
            });
        }

    }
}
