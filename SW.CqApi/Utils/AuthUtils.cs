using SW.PrimitiveTypes;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Models;

namespace SW.CqApi.Utils
{
    internal static class AuthUtils
    {
        public static IEnumerable<string> GetRoles(this IDictionary<string, IDictionary<string, HandlerInfo>> resourceHandlers)
        {
            List<string> roles = new List<string>();
            foreach (var res in resourceHandlers)
            {
                var key = res.Key.ToLower();
                roles.Add( $"{key}.*");
                foreach(var handler in res.Value)
                {
                    var protect = handler.Value.HandlerType.GetCustomAttribute<ProtectAttribute>();
                    if(protect != null && protect.RequireRole)
                    {
                        string name = handler.Value.HandlerType.Name.ToLower();
                        roles.Add( $"{key}.{name}");
                    }
                }
            }
            return roles;
        }
    }
}
