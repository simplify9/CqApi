using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi
{
    internal class HandlerTypeMetadata
    {
        public static readonly IDictionary<Type, HandlerTypeMetadata> Handlers = new Dictionary<Type, HandlerTypeMetadata>
        {
                {typeof(ICommandHandler), new HandlerTypeMetadata
                {
                    Key = "post",
                }},
                {typeof(ICommandHandler<>), new HandlerTypeMetadata
                {
                    Key = "post",
                    SupportsNaming = true
                }},
                {typeof(ICommandHandler<,>), new HandlerTypeMetadata
                {
                    Key = "post/key"
                }},
                {typeof(IQueryHandler), new HandlerTypeMetadata
                {
                    Key = "get"
                }},
                {typeof(IQueryHandler<>), new HandlerTypeMetadata
                {
                    Key = "get"
                }},
                {typeof(ISearchyHandler), new HandlerTypeMetadata
                {
                    Key = "get"
                }},
                {typeof(IDeleteHandler<>), new HandlerTypeMetadata
                {
                    Key = "delete/key"
                }},
                {typeof(IGetHandler<>), new HandlerTypeMetadata
                {
                    Key = "get/key"
                }},



        };
        public string Key { get; set; }

        public bool SupportsNaming { get; set; }
    }
}
