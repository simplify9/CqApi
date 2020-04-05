using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi.Options
{
    public class TypeMaps
    {
        public class TypedExample
        {
            public Type Type { get; set; }
            public object Example { get; set; }
            public IOpenApiAny OpenApiExample { get; set; }
        }

        private Dictionary<Type, TypedExample> Maps { get; } = new Dictionary<Type, TypedExample>();
        public void AddMap<T1, T2>(IOpenApiAny example)
        {
            Maps.Add(typeof(T1), new TypedExample{
                Type = typeof(T2),
                OpenApiExample = example
            });
        }
        public void AddMap<T1, T2>(T2 example)
        {
            Maps.Add(typeof(T1), new TypedExample{
                Type = typeof(T2),
                Example = example
            });
        }

        public bool ContainsMap(Type type)
        {
            if (Maps.ContainsKey(type)) return true;
            return false;
        }

        public bool ContainsMap<T>()
        {
            if (Maps.ContainsKey(typeof(T))) return true;
            return false;
        }

        public TypedExample GetMap(Type type)
        {
            return Maps[type];
        }

        public TypedExample GetMap<T>()
        {
            return Maps[typeof(T)];
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
