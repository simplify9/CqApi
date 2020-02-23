using Microsoft.OpenApi.Models;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi.Utils
{
    internal static class CustomAttributeUtils
    {
        public static OpenApiResponses ToOpenApiResponses(this IEnumerable<ReturnsAttribute> attributes, OpenApiComponents components)
        {
            var responses = new OpenApiResponses();
            foreach(var attribute in attributes)
            {
                OpenApiSchema schema = TypeUtils.ExplodeParameter(attribute.Type, components);
                var mediaType = new OpenApiMediaType
                {
                    Schema = schema
                };

                responses.Add(attribute.StatusCode.ToString(), new OpenApiResponse
                {
                    Description = attribute.Description,
                    Content = { ["application/json"] = mediaType }
                });
            }

            return responses;
        }

    }
}
