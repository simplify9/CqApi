using Microsoft.OpenApi.Models;
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

            {
                typeof(ICommandHandler<>), new HandlerTypeMetadata
                {

                    Key = "post",
                    OpenApiOperation = new OpenApiOperation
                    {
                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",
                            }
                        }
                    }                
                }
            },
            {
                typeof(ICommandHandler<,>), new HandlerTypeMetadata
                {
                    Key = "post",
                    OpenApiOperation = new OpenApiOperation
                    {


                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }                
                }
            },
            {
                typeof(ICommandHandler<,,>), new HandlerTypeMetadata
                {
                    Key = "post/key",
                    OpenApiOperation = new OpenApiOperation
                    {


                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }

                }
            },
            {
                typeof(IQueryHandler<>), new HandlerTypeMetadata
                {
                    Key = "get",
                    OpenApiOperation = new OpenApiOperation
                    {


                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }
                }
            },
            {
                typeof(IQueryHandler<,>), new HandlerTypeMetadata
                {
                    Key = "get",
                    OpenApiOperation = new OpenApiOperation
                    {


                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }
                }
            },
            {
                typeof(ISearchyHandler), new HandlerTypeMetadata
                {
                    Key = "get",
                    OpenApiOperation = new OpenApiOperation
                    {
                        Parameters = new List<OpenApiParameter>
                        {
                            new OpenApiParameter
                            {
                                Name = "size",
                                AllowEmptyValue = true,
                                In = ParameterLocation.Query,

                                Schema = new OpenApiSchema
                                {
                                    Type = "integer",
                                }
                            }
                        },

                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }

                }
            },
            {
                typeof(IDeleteHandler<,>), new HandlerTypeMetadata
                {
                    Key = "delete/key",
                    OpenApiOperation = new OpenApiOperation
                    {


                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }
                }
            },
            {
                typeof(IGetHandler<,>), new HandlerTypeMetadata
                {
                    Key = "get/key",
                    OpenApiOperation = new OpenApiOperation
                    {


                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }
                }
            },
            {
                typeof(IQueryHandler<,,>), new HandlerTypeMetadata
                {
                    Key = "get/key",
                    OpenApiOperation = new OpenApiOperation
                    {


                        Description = "",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "OK",

                            }
                        }
                    }
                }
            },

        };
        public string Key { get; set; }
        //public bool SupportsNaming { get; set; }
        public OpenApiOperation OpenApiOperation { get; set; }
    }
}
