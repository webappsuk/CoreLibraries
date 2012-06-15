using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApplications.Utilities.DataAnnotations
{
    // deals with serializing ext4 metadata structures into their javascript counterparts
    public class ExtJsModelSerializer
    {
        // default settings for serializer
        private const Formatting Format = Formatting.Indented;

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string GetSerializedModel(Type input)
        {
            // single type
            return JsonConvert
                .SerializeObject(
                    ExtJsMetaData.GetExtJs4Model(input),
                    Format,
                    _settings
                );
        }

        public static string GetSerializedModels(IEnumerable<Type> types)
        {
            // list of types
            return JsonConvert
                .SerializeObject(
                    ExtJsMetaData.GetModelsForTypesDictionary(types),
                    Format,
                    _settings
                );
        }
    }
}