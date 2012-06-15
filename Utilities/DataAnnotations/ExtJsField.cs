using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Display;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;
using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations
{
    public class ExtJsField
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("type")]
        public string Type { get; private set; }

        // only applicable to dates
        [JsonProperty("dateFormat")]
        public string DateFormat { get; private set; }

        // set if field is nullable
        [JsonProperty("useNull")]
        public bool? UseNull { get; private set; }

        // display properties
        [JsonProperty("displayTag")]
        public string DisplayTag { get; private set; }

        private readonly IEnumerable<Base> _validations;

        [JsonProperty("isEnumType")]
        public bool IsEnumType { get; private set; }

        private static readonly Dictionary<Type, string> JavascriptTypeDictionary
            = new Dictionary<Type, string>
            {
                { typeof(string), "string" },
                { typeof(int), "int"},
                { typeof(float), "float" },
                { typeof(DateTime), "date"},
                { typeof(double), "float"},
                { typeof(bool), "bool"},
                { typeof(short), "int"},
                // nullables
                { typeof(int?), "int"},
                { typeof(float?), "float" },
                { typeof(double?), "float" },
                { typeof(bool?), "bool"},
                { typeof(DateTime?), "date"},
                { typeof(short?), "int"}
            };

        public ExtJsField(PropertyInfo prop)
        {
            // we only understand simple properties in our models and these classes reflect that.
            // the available types are mapped above
            Name = prop.Name;


            if (typeof(Enum).IsAssignableFrom(prop.PropertyType))
            {
                IsEnumType = true;
            }

            Type = GetJavascriptType(!IsEnumType ? prop.PropertyType : Enum.GetUnderlyingType(prop.PropertyType));

            DateFormat = GetDateFormat(prop);

            UseNull = IsNullable(prop);

            if (Attribute.IsDefined(prop, typeof(DisplayAttribute)))
            {
                var displayAttribute = prop.GetCustomAttribute<DisplayAttribute>();
                // we could do more than just name, if we wanted
                DisplayTag = displayAttribute.Tag;
            }

            _validations = GetValidations(prop);

            // clean up any validations that deal with length
            _validations = RefactorValidations();
        }

        private IEnumerable<Base> RefactorValidations()
        {
            var originalValidations = _validations.ToList();
            // deal with mapping of maxlength/minlength/range into one ext4 output
            var lengthValidations = originalValidations.OfType<Length>().ToList();

            if (lengthValidations.Count() > 1)
            {
                // store our non-length validators
                var original = originalValidations.Except(lengthValidations).ToList();

                // merge our length validators
                var newLength = new Length
                {
                    Max = lengthValidations.Min(item => item.Max),
                    Min = lengthValidations.Max(item => item.Min),
                    ErrorTag = lengthValidations.FirstOrDefault().ErrorTag,
                    Field = lengthValidations.FirstOrDefault().Field,
                };

                original.Add(newLength);

                return original;
            }

            return _validations;
        }

        public static string GetJavascriptType(Type type)
        {
            try
            {
                return JavascriptTypeDictionary.ContainsKey(type) ? JavascriptTypeDictionary[type] : "auto";
            }
            catch (KeyNotFoundException e)
            {
                throw new KeyNotFoundException("An error occoured when attempting to serialize the following type:" + type.Name, e);
            }
        }

        private IEnumerable<Base> GetValidations(PropertyInfo prop)
        {
            // get our validation attributes, these should all be part of the IExt4Validator interface
            var validators = Utility.GetInheritedAttributes(prop)
                .OfType<ITravellerValidator>()
                .Select(attrib =>
                {
                    var validator = attrib.GetExt4Validator();
                    validator.Field = prop.Name;
                    return validator;

                });

            if (IsEnumType)
            {
                validators = validators.Union(new List<Base>
                {
                    new Inclusion(new IncludeAttribute(Enum.GetValues(prop.PropertyType))) { Field = prop.Name }
                });
            }

            return validators;
        }

        private static string GetDateFormat(PropertyInfo prop)
        {
            // if we have a date, use the standard MS date format
            return prop.PropertyType == typeof(DateTime)
                || prop.PropertyType == typeof(DateTime?) ? "c" : null;
        }

        public static bool? IsNullable(PropertyInfo prop)
        {
            // work out if our field property should be nullable
            if (prop.PropertyType.IsGenericType
                && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }
            return null;
        }

        public IEnumerable<Base> GetValidations()
        {
            return _validations;
        }
    }
}