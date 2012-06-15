using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Display;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations
{
    public class ExtJsMetaData
    {
        private static readonly ConcurrentDictionary<Type, ExtJsMetaData> _metaDataCache = new ConcurrentDictionary<Type, ExtJsMetaData>();

        [JsonProperty("fields")]
        public IEnumerable<ExtJsField> Fields { get; private set; }

        [JsonProperty("validations")]
        public IEnumerable<Base> Validations { get; private set; }

        [JsonProperty("apiUrl")]
        public string ApiUrl { get; private set; }

        [JsonProperty("idProperty")]
        public string IdProperty { get; private set; }

        private static IEnumerable<ExtJsField> GetFields(Type input)
        {
            // get properties for a given type and populate them as ext4 fields
            return input
                .GetProperties()
                .Where(item => item.CustomAttributes
                    .All(prop => prop.AttributeType != typeof(IgnoreDataMemberAttribute)))
                .Select(item => new ExtJsField(item));
        }

        public static ExtJsMetaData GetExtJs4Model(Type input)
        {
            // simple caching to avoid multiple generations of metadata
            // does not need an expiry as this data will not change unless project is re-compiled
            ExtJsMetaData cacheHit;

            if (_metaDataCache.TryGetValue(input, out cacheHit))
                return cacheHit;

            // grab our fields for our type input then grab the validations in a flattened list
            List<ExtJsField> fields = GetFields(input).ToList();

            ApiLocationAttribute apiAttribute =
                (ApiLocationAttribute)input.GetCustomAttributes(typeof(ApiLocationAttribute), false).FirstOrDefault();

            PropertyInfo idProperty = input.GetProperties().FirstOrDefault(item => Utility.GetInheritedAttributes(item).OfType<IdPropertyAttribute>().Any());

            ExtJsMetaData model = new ExtJsMetaData
            {
                Fields = fields,
                Validations = fields.SelectMany(item => item.GetValidations()),
                ApiUrl = apiAttribute != null ? apiAttribute.ApiUrl : null,
                IdProperty = idProperty != null ? idProperty.Name : null
            };

            _metaDataCache.AddOrUpdate(input, model, (key, oldValue) => oldValue);

            return model;
        }

        public static IDictionary<string, ExtJsMetaData> GetModelsForTypesDictionary(IEnumerable<Type> types)
        {
            // convert flat list into a keyvalue pair dictionary, using the type name as the key
            // use dictionary method to return
            return GetModelsForTypesDictionary(types.ToDictionary(item => item.Name));
        }

        public static IDictionary<string, ExtJsMetaData> GetModelsForTypesDictionary(IEnumerable<KeyValuePair<string, Type>> types)
        {
            return types.AsParallel().ToDictionary(item => item.Key, item => GetExtJs4Model(item.Value));
        }
    }
}