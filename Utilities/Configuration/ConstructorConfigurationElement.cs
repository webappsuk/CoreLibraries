#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   A configuration element that is used for object construction.
    /// </summary>
    public class ConstructorConfigurationElement : ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the type of the element.
        /// </summary>
        /// <value>The type of the object to construct.</value>
        /// <exception cref="ConfigurationErrorsException">The property is read-only or locked.</exception>
        [ConfigurationProperty("type", IsRequired = true)]
        [TypeConverter(typeof (TypeNameConverter))]
        [NotNull]
        [UsedImplicitly]
        public virtual Type Type
        {
            get { return (Type) this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        ///   Gets or sets the parameters to be passed to the constructor.
        /// </summary>
        /// <value>
        ///   The <see cref="WebApplications.Utilities.Configuration.ParameterCollection"/>,
        ///   which is all of the child elements within the parameters element in the configuration file.
        /// </value>
        /// <exception cref="ConfigurationErrorsException">The property is read-only or locked.</exception>
        [ConfigurationProperty("parameters", IsRequired = false, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof (ParameterCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        [NotNull]
        [UsedImplicitly]
        public virtual ParameterCollection Parameters
        {
            get { return (ParameterCollection) this["parameters"]; }
            set { this["parameters"] = value; }
        }

        /// <summary>
        ///   Gets an instance of the <see cref="object"/>.
        /// </summary>
        /// <returns>An instance of an object of type <typeparamref name="T"/>.</returns>
        /// <remarks>
        ///   This should only be called once as it calls the <see cref="GetConstructor{T}"/> method each time.
        ///   Instead you should use <see cref="GetConstructor{T}"/> where possible and store the resultant
        ///   <see cref="Func{TResult}"/>, which can then be called repeatedly to create new instances.
        /// </remarks>
        [UsedImplicitly]
        [NotNull]
        public T GetInstance<T>()
        {
            return GetConstructor<T>()();
        }

        /// <summary>
        ///   Gets the constructor as an action, which can be used to create instances of the specified object.
        /// </summary>
        /// <typeparam name="T">The type to get the constructor for.</typeparam>
        /// <returns>
        ///   Returns a <see cref="Func{TResult}"/> that can be used to create an instance of the specified object.
        /// </returns>
        /// <remarks>
        ///   This should only be called once as it uses reflection.
        ///   Store the resulting <see cref="Func{TResult}"/> where possible and use it to repeatedly create new instances.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///   <para>The configuration system found multiple potential constructor matches.</para>
        ///   <para>-or-</para>
        ///   <para>The configuration system couldn't find a constructor with the relevant parameters.</para>
        ///   <para>-or-</para>
        ///   <para>The configuration system cannot assign the created type to the return type.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public Func<T> GetConstructor<T>()
        {
            Type instanceType = Type;

            // Build dictionary of parameters from property.
            Dictionary<string, PInfo> parameters = (from ConfigurationProperty property in Properties
                                                    select
                                                        new PInfo(property, this[property])).ToDictionary(
                                                            info => info.Name);

            // Overwrite with elements in parameters collection.
            foreach (ParameterElement element in Parameters)
            {
                PInfo info = new PInfo(element);
                PInfo i;
                if (!parameters.TryGetValue(info.Name, out i))
                {
                    parameters.Add(element.Name, info);
                }
                else
                {
                    parameters[info.Name] = info;
                }
            }

            // Count required and optional parameters.
            int required = 0;
            int optional = 0;
            foreach (PInfo info in parameters.Values)
            {
                if (info.IsRequired) required++;
                else optional++;
            }

            // Find the best matching constructor
            int requiredScore = -1;
            int optionalScore = -1;
            bool isAmbiguous = false;
            ConstructorInfo constructor = null;
            ParameterInfo[] parameterInfos = null;
            foreach (
                ConstructorInfo c in
                    Type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                         BindingFlags.CreateInstance))
            {
                // Get parameters;
                ParameterInfo[] pis = c.GetParameters();

                // If we don't have enough parameters we definitely don't match!
                if (pis.GetLength(0) < required)
                    continue;

                int rs = 0;
                int os = 0;
                foreach (ParameterInfo pi in pis)
                {
                    // We can ignore retvals.
                    if (pi.IsRetval)
                        continue;

                    // If we encounter an output or reference value, we will not be able to use the constructer, and so must exclude it
                    if (pi.IsOut || pi.ParameterType.IsByRef)
                    {
                        rs = -1;
                        break;
                    }


                    // If the parameter name is unknown, then can't match!
                    if (pi.Name == null)
                    {
                        rs = -1;
                        break;
                    }

                    PInfo info;
                    if (!parameters.TryGetValue(pi.Name, out info))
                    {
                        // If the parameter is required and we don't have a match then constructor doesn't match.
                        if (!pi.IsOptional)
                        {
                            rs = -1;
                            break;
                        }
                        continue;
                    }

                    if (info.IsRequired)
                    {
                        // If we have a required property and the parameter type is not assignable from the explicit type
                        // then we do not match.
                        if ((info.Type != null) && (!pi.ParameterType.IsAssignableFrom(info.Type)))
                        {
                            rs = -1;
                            break;
                        }
                        rs++;
                    }
                    else
                        os++;
                }

                // If we haven't got all the required parameters, or we don't have as many as the current best match
                // then we're not the best match.
                if ((rs < required) ||
                    (rs < requiredScore) ||
                    ((rs == requiredScore && os < optionalScore)))
                    continue;

                // If we have the same required and optional score as our previous winner then we have an ambiguous match.
                if ((rs == requiredScore) && (os == optionalScore))
                {
                    isAmbiguous = true;
                    continue;
                }

                // We're the new unambiguous winner
                isAmbiguous = false;
                requiredScore = rs;
                optionalScore = os;
                constructor = c;
                parameterInfos = pis;
            }

            if (isAmbiguous)
                throw new InvalidOperationException(
                    string.Format(
                        Resources.ConstructorConfigurationElement_GetConstructor_ConstructorIsAmbiguous,
                        Type));

            if (constructor == null)
                throw new InvalidOperationException(
                    string.Format(
                        Resources.ConstructorConfigurationElement_GetConstructor_CannotFindConstructor,
                        Type));

            List<ConstantExpression> arguments = new List<ConstantExpression>();
            foreach (ParameterInfo p in parameterInfos)
            {
                object value = null;
                PInfo info;
                bool useDefault = true;
                if (parameters.TryGetValue(p.Name, out info) &&
                    ((info.Type == null) ||
                     (p.ParameterType.IsAssignableFrom(info.Type))))
                {
                    if (!info.ValueSet)
                    {
                        info.Type = p.ParameterType;
                    }
                    value = info.Value;
                    useDefault = false;
                }

                if (useDefault)
                    value = p.RawDefaultValueSafe();

                arguments.Add(Expression.Constant(value, p.ParameterType));
            }

            Expression create = Expression.New(constructor, arguments);

            // Check to see if we are being asked to cast.
            Type returnType = typeof (T);
            if (returnType != instanceType)
            {
                if (!returnType.IsAssignableFrom(instanceType))
                    throw new InvalidOperationException(
                        string.Format(
                            Resources.ConstructorConfigurationElement_GetConstructor_CreatedTypeNotAssignable,
                            instanceType,
                            returnType));
                create = create.Convert(returnType);
            }

            // Compile the lambda and return
            return (Func<T>) Expression.Lambda(create).Compile();
        }

        #region Nested type: PInfo
        /// <summary>
        ///   Used to hold information about parameters.
        /// </summary>
        private class PInfo
        {
            /// <summary>
            ///   A <see cref="bool"/> value which indicates whether the parameter is required in the constructor.
            /// </summary>
            public bool IsRequired;

            /// <summary>
            ///   The name of the parameter.
            /// </summary>
            public string Name;

            /// <summary>
            ///   Gets or sets the type converter.
            /// </summary>
            private TypeConverter _converter;

            private Type _type;

            private object _value;

            /// <summary>
            ///   The <see cref="string"/> equivalent of the value.
            /// </summary>
            private string _valueStr;

            /// <summary>
            ///   Initializes a new instance of the <see cref="PInfo"/> class.
            /// </summary>
            /// <param name="element">The parameter element.</param>
            public PInfo([NotNull] ParameterElement element)
            {
                Name = element.Name;
                IsRequired = element.IsRequired;
                _valueStr = element.Value;

                Type convertorType = element.TypeConverter;
                if (convertorType == null)
                {
                    if (element.Type == null)
                        return;

                    // If we have an explicit type we can grab the type converter now.
                    Type = element.Type;
                    return;
                }
                _converter = (TypeConverter) Activator.CreateInstance(convertorType, true);
                if (element.Type != null)
                    Type = element.Type;
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="PInfo"/> class.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="value">The value.</param>
            public PInfo([NotNull] ConfigurationProperty property, object value)
            {
                Name = property.Name;
                IsRequired = false;
                Value = value;
                _type = property.Type;
            }

            /// <summary>
            ///   The type (if specified).
            /// </summary>
            /// <exception cref="ArgumentNullException">
            ///   The type being set is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            ///   <para>Cannot create a default type converter for the parameter value specified.</para>
            ///   <para>-or-</para>
            ///   <para>Couldn't convert the <see cref="string"/> value to the type specified.</para>
            ///   <para>-or-</para>
            ///   <para>The parameter type is not assignable from the type returned by the converter.</para>
            ///   <para>-or-</para>
            ///   <para>The converter returned a <see langword="null"/> value for a non-nullable type.</para>
            /// </exception>
            public Type Type
            {
                get { return _type; }
                set
                {
                    if (_type == value)
                        return;

                    if (value == null)
                        throw new ArgumentNullException("value");

                    _type = value;

                    if (_converter == null)
                    {
                        // Get default converter for type
                        _converter = TypeDescriptor.GetConverter(_type);
                        if (_converter == null)
                            throw new InvalidOperationException(string.Format(
                                Resources.PInfo_TypeProperty_CannotCreateDefaultTypeConverter,
                                Name,
                                _type));
                    }

                    if (!_converter.CanConvertFrom(typeof (string)))
                        throw new InvalidOperationException(string.Format(
                            Resources.PInfo_TypeProperty_ConverterCannotConvertFromString,
                            _converter.GetType(),
                            Name));

                    // If we have a null value just create the default anyway.
                    if (_valueStr == null)
                    {
                        Value = _type.Default();
                    }
                    else
                    {
                        try
                        {
                            Value = _converter.ConvertFromString(_valueStr);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    Resources.PInfo_TypeProperty_CannotConvertValueStrToDestinationType,
                                    _valueStr,
                                    Name,
                                    _type), e);
                        }

                        if (Value != null)
                        {
                            Type valueType = Value.GetType();
                            if (!_type.IsAssignableFrom(valueType))
                                throw new InvalidOperationException(string.Format(
                                    Resources.PInfo_TypeProperty_TypeNotAssignableFromConvertedType,
                                    _converter.GetType(),
                                    valueType,
                                    Name,
                                    _type));
                        }
                        else if (!_type.IsClass)
                            throw new InvalidOperationException(string.Format(
                                Resources.PInfo_TypeProperty_ConveterReturnedNullForNonNullableType,
                                _converter.GetType(),
                                Name,
                                _type));
                    }
                }
            }

            /// <summary>
            ///   Gets or sets a <see cref="bool"/> value indicating whether the value is set.
            /// </summary>
            public bool ValueSet { get; private set; }

            /// <summary>
            ///   The value parsed from the <see cref="string"/> equivalent of the value.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            ///   No value has been set for the parameter requested.
            /// </exception>
            public object Value
            {
                get
                {
                    if (!ValueSet)
                        throw new InvalidOperationException(string.Format(
                            Resources.PInfo_ValueProperty_NoValueSet,
                            Name));
                    return _value;
                }
                set
                {
                    _value = value;
                    _valueStr = value == null ? null : _value.ToString();
                    ValueSet = true;
                }
            }

            /// <summary>
            ///   Returns a <see cref="string"/> that represents this instance.
            /// </summary>
            /// <returns>
            ///   A <see cref="string"/> representation of this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{0}{1} = '{2}'",
                                     IsRequired ? "(REQUIRED)" : string.Empty,
                                     Name,
                                     _valueStr);
            }
        }
        #endregion
    }
}