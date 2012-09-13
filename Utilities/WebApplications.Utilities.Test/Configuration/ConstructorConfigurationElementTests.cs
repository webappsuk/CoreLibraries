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
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Configuration;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ConstructorConfigurationElementTests : ConfigurationTestBase
    {
        private const string DefaultValueForOptionalArgument = "Default Value.";

        private static ConstructorConfigurationElement GenerateEmptyConstructorConfigurationElement()
        {
            return new ConstructorConfigurationElement();
        }

        private static ConstructorConfigurationElement GenerateConstructorConfigurationElementForType(Type type)
        {
            return new ConstructorConfigurationElement {Type = type};
        }

        private static ConstructorConfigurationElementTestClassWithGuidProperty
            GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(Type type)
        {
            return new ConstructorConfigurationElementTestClassWithGuidProperty {Type = type};
        }

        [TestMethod]
        public void ConstructorConfigurationElement_Extends_ConfigurationElement()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateEmptyConstructorConfigurationElement();
            Assert.IsInstanceOfType(constructorConfigurationElement, typeof (ConfigurationElement),
                                    "The ConstructorConfigurationElement class should extend Utilities.Configuration.ConfigurationElement.");
        }

        [TestMethod]
        public void Type_SetToType_ReturnsSetValue()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateEmptyConstructorConfigurationElement();
            Type testType =
                ChooseRandomTypeFromList(
                    (new List<Type> {typeof (int), typeof (char), typeof (bool), typeof (DateTime), typeof (double)}));
            constructorConfigurationElement.Type = testType;
            Assert.AreEqual(testType, constructorConfigurationElement.Type,
                            "The Type field should return the same value as it was last set to.");
        }

        [TestMethod]
        public void Type_HasTypeNameConverter()
        {
            MemberInfo propertyInfo = typeof (ConstructorConfigurationElement).GetMember("Type").First();
            List<TypeConverterAttribute> typeConverterAttributes =
                propertyInfo.GetCustomAttributes(false).OfType<TypeConverterAttribute>().ToList();
            Assert.AreEqual(1, typeConverterAttributes.Count,
                            "There should be exactly one TypeConverter attribute on the Type property.");
            Assert.IsTrue(
                typeConverterAttributes.First().ConverterTypeName.StartsWith("System.Configuration.TypeNameConverter,"));
        }

        [TestMethod]
        public void Parameters_ParameterAdded_ContainsNewParameter()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateEmptyConstructorConfigurationElement();

            String name = Random.RandomString(10);
            ParameterElement parameter = new ParameterElement {Name = name};

            constructorConfigurationElement.Parameters.Add(parameter);

            Assert.IsTrue(constructorConfigurationElement.Parameters.Contains(parameter),
                          "After adding a parameter element to the Parameters property, Parameters.Contains should return true for the added parameter.");
        }

        [TestMethod]
        public void Parameters_SetToNewParameterCollection_ReturnsNewParameterCollection()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateEmptyConstructorConfigurationElement();

            ParameterCollection parameterCollection = new ParameterCollection();
            constructorConfigurationElement.Parameters = parameterCollection;

            Assert.AreSame(parameterCollection, constructorConfigurationElement.Parameters,
                           "After setting the Parameters property, it should then return this new value.");
        }

        [TestMethod]
        public void GetInstance_TypeHasConstructorTakingOneGuid_ResultIsEqualToCallingResultOfGetConstructor()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {Name = "guid", Value = Guid.NewGuid().ToString()});

            TestClassWithConstructorTakingOneGuid instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingOneGuid>();

            TestClassWithConstructorTakingOneGuid constructorResult =
                constructorConfigurationElement.GetConstructor<TestClassWithConstructorTakingOneGuid>()();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(constructorResult);
            Assert.AreEqual(constructorResult.Guid, instance.Guid,
                            "The result of GetInstance should be the same as calling the result of GetConstructor.");
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingOneGuidAndGuidValueSuppliedByParameterOnly_GuidTakesValueFromParameter()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {Name = "guid", Value = parameterValue.ToString()});

            TestClassWithConstructorTakingOneGuid instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingOneGuid>();

            Assert.AreEqual(parameterValue, instance.Guid,
                            "The value of a parameter in Parameters should be passed to the constructor.");
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingOneGuidAndGuidValueSuppliedByPropertyOnly_GuidTakesValueFromProperty()
        {
            ConstructorConfigurationElementTestClassWithGuidProperty constructorConfigurationElement =
                GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(
                    typeof (TestClassWithConstructorTakingOneGuid));

            Guid propertyValue = Guid.NewGuid();
            constructorConfigurationElement.Guid = propertyValue;

            TestClassWithConstructorTakingOneGuid instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingOneGuid>();

            Assert.AreEqual(propertyValue, instance.Guid,
                            "The value of a ConfigurationProperty of the ConstructorConfigurationElement should be passed to the constructor.");
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingOneGuidAndGuidValueSuppliedByBothPropertyAndParameter_GuidTakesValueFromParameter
            ()
        {
            ConstructorConfigurationElementTestClassWithGuidProperty constructorConfigurationElement =
                GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(
                    typeof (TestClassWithConstructorTakingOneGuid));

            Guid propertyValue = Guid.NewGuid();
            constructorConfigurationElement.Guid = propertyValue;

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {Name = "guid", Value = parameterValue.ToString()});

            TestClassWithConstructorTakingOneGuid instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingOneGuid>();

            Assert.AreEqual(parameterValue, instance.Guid,
                            "The value of a parameter in Parameters should be passed to the constructor, overriding and ConfigurationProperty of the same name.");
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void
            GetConstructor_TypeHasConstructorTakingOneGuidAndGuidValueNotSupplied_ThrowsInvalidOperationException()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            constructorConfigurationElement.GetConstructor<TestClassWithConstructorTakingOneGuid>();
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void
            GetConstructor_TypeHasConstructorTakingOneGuidButParameterNameNotMatchingDueToUsingDifferentCase_ThrowsInvalidOperationException
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            // Set the value of GUID rather than guid, and so not supply enough parameters for the only constructor
            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {Name = "GUID", Value = parameterValue.ToString()});

            constructorConfigurationElement.GetConstructor<TestClassWithConstructorTakingOneGuid>();
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingOneGuidAndGuidValueSuppliedByNonRequiredParameter_GuidTakesValueFromParameter
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "guid",
                                                                   Value = parameterValue.ToString(),
                                                                   IsRequired = false
                                                               });

            TestClassWithConstructorTakingOneGuid instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingOneGuid>();

            Assert.AreEqual(parameterValue, instance.Guid,
                            "The value of a parameter in Parameters with IsRequired=false should be passed to the constructor if possible.");
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingOneGuidWithUnwantedParameterWithIsRequiredFalse_UnwantedParameterIsIgnored
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {Name = "guid", Value = parameterValue.ToString()});

            Guid unwantedParameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "unwantedParameterNotInAnyConstructor",
                                                                   Value = unwantedParameterValue.ToString(),
                                                                   IsRequired = false
                                                               });

            TestClassWithConstructorTakingOneGuid instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingOneGuid>();

            Assert.AreEqual(parameterValue, instance.Guid,
                            "If no constructors exist which can use a parameter with IsRequired=false, it should be ignored.");
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void
            GetConstructor_TypeHasConstructorTakingOneGuidWithUnwantedParameterWithIsRequiredTrue_ThrowsInvalidOperationException
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {Name = "guid", Value = parameterValue.ToString()});

            Guid unwantedParameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "unwantedParameterNotInAnyConstructor",
                                                                   Value = unwantedParameterValue.ToString(),
                                                                   IsRequired = true
                                                               });

            constructorConfigurationElement.GetConstructor<TestClassWithConstructorTakingOneGuid>();
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void
            GetConstructor_TypeHasConstructorTakingOneGuidWithInvalidStringRepresentationOfGuidSuppliedAsValue_ThrowsInvalidOperationException
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "guid",
                                                                   Value = "Not a valid string representation of a Guid."
                                                               });

            constructorConfigurationElement.GetConstructor<TestClassWithConstructorTakingOneGuid>();
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void GetConstructor_TypeDoesNotCastIntoTheRequestedInstanceType_ThrowsInvalidOperationException()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingOneGuid));

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "guid",
                                                                   Value = "Not a valid string representation of a Guid."
                                                               });

            // TestClassWithConstructorTakingOneGuid does not cast to int
            constructorConfigurationElement.GetConstructor<int>();
        }

        [TestMethod]
        public void
            GetInstance_TypeInheritsFromClassInstanceIsCastTo_ConstructorsFromInheritingClassUsedThenLaterCastToOutputType
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(
                    typeof (TestClassInheritingFromTestClassWithConstructorTakingOneGuid));

            Guid parameterValue = Guid.NewGuid();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "guidForInheritingClass",
                                                                   Value = parameterValue.ToString(),
                                                                   IsRequired = false
                                                               });

            TestClassWithConstructorTakingOneGuid instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingOneGuid>();

            Assert.AreEqual(parameterValue, instance.Guid,
                            "The value of a parameter in Parameters should be passed to the constructor if possible.");
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingThreeIntsAndValuesSuppliedByParameters_ParametersSuppliedToConstructorArgumentsOfSameName
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingThreeInts));

            int parameterValueA = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "a",
                                                                   Value =
                                                                       parameterValueA.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            int parameterValueC = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "c",
                                                                   Value =
                                                                       parameterValueC.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            int parameterValueB = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "b",
                                                                   Value =
                                                                       parameterValueB.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            TestClassWithConstructorTakingThreeInts instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingThreeInts>();

            Assert.AreEqual(parameterValueA, instance.A,
                            "The value of a parameter in Parameters should be passed to the constructor as the argument whose name it matches.");
            Assert.AreEqual(parameterValueB, instance.B,
                            "The value of a parameter in Parameters should be passed to the constructor as the argument whose name it matches.");
            Assert.AreEqual(parameterValueC, instance.C,
                            "The value of a parameter in Parameters should be passed to the constructor as the argument whose name it matches.");
        }

        [TestMethod]
        public void GetInstance_TypeHasConstructorTakingThreeIntsWithUnwantedProperty_UnwantedPropertyIsIgnored()
        {
            ConstructorConfigurationElementTestClassWithGuidProperty constructorConfigurationElement =
                GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(
                    typeof (TestClassWithConstructorTakingThreeInts));

            Guid unwantedParameterValue = Guid.NewGuid();
            constructorConfigurationElement.Guid = unwantedParameterValue;

            int parameterValueA = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "a",
                                                                   Value =
                                                                       parameterValueA.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            int parameterValueC = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "c",
                                                                   Value =
                                                                       parameterValueC.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            int parameterValueB = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "b",
                                                                   Value =
                                                                       parameterValueB.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            TestClassWithConstructorTakingThreeInts instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingThreeInts>();

            Assert.AreEqual(parameterValueA, instance.A,
                            "The value of a parameter in Parameters should be passed to the constructor as the argument whose name it matches.");
            Assert.AreEqual(parameterValueB, instance.B,
                            "The value of a parameter in Parameters should be passed to the constructor as the argument whose name it matches.");
            Assert.AreEqual(parameterValueC, instance.C,
                            "The value of a parameter in Parameters should be passed to the constructor as the argument whose name it matches.");
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void
            GetInstance_TypeHasMultipleConstructorsAndTooFewParametersSuppliedToDistinguishWhichToUse_ThrowsInvalidOperationException
            ()
        {
            ConstructorConfigurationElementTestClassWithGuidProperty constructorConfigurationElement =
                GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(
                    typeof (TestClassWithMultipleConstructors));

            int parameterValue = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "requiredParameter",
                                                                   Value =
                                                                       parameterValue.ToString(CultureInfo.InvariantCulture)
                                                               });

            TestClassWithMultipleConstructors instance =
                constructorConfigurationElement.GetInstance<TestClassWithMultipleConstructors>();
        }

        [TestMethod]
        public void
            GetInstance_TypeHasMultipleConstructorsAndEnoughParametersSuppliedForTwoConstructorsButSomeMarkedIsRequiredFalse_ChoosesConstructorUsingAllRequiredParameters
            ()
        {
            ConstructorConfigurationElementTestClassWithGuidProperty constructorConfigurationElement =
                GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(
                    typeof (TestClassWithMultipleConstructors));

            int parameterValue = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "requiredParameter",
                                                                   Value =
                                                                       parameterValue.ToString(CultureInfo.InvariantCulture)
                                                               });

            int optionalParameterValue = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "optionalParameter",
                                                                   Value =
                                                                       optionalParameterValue.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            TestClassWithMultipleConstructors instance =
                constructorConfigurationElement.GetInstance<TestClassWithMultipleConstructors>();

            Assert.AreEqual("Constructor with optionalParameter", instance.ConstructorUsed);
        }

        [TestMethod]
        public void
            GetInstance_TypeHasMultipleConstructorsAndEnoughParametersSuppliedForTwoConstructorsAndAllMarkedIsRequiredFalse_ChoosesConstructorUsingMostOptionalParameters
            ()
        {
            ConstructorConfigurationElementTestClassWithGuidProperty constructorConfigurationElement =
                GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(
                    typeof (TestClassWithMultipleConstructors));

            int parameterValueA = Random.Next(short.MaxValue);
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "a",
                                                                   Value =
                                                                       parameterValueA.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            int parameterValueB = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "b",
                                                                   Value =
                                                                       parameterValueB.ToString(
                                                                           CultureInfo.InvariantCulture),
                                                                   IsRequired = false
                                                               });

            int parameterValueC = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "c",
                                                                   Value =
                                                                       parameterValueC.ToString(
                                                                           CultureInfo.InvariantCulture),
                                                                   IsRequired = false
                                                               });

            int parameterValueD = Random.Next(short.MaxValue);
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "d",
                                                                   Value =
                                                                       parameterValueD.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            TestClassWithMultipleConstructors instance =
                constructorConfigurationElement.GetInstance<TestClassWithMultipleConstructors>();

            Assert.AreEqual("Constructor with two parameters: a and d.", instance.ConstructorUsed);
        }

        [TestMethod]
        public void
            GetInstance_TypeHasMultipleConstructorsAndEnoughParametersSuppliedToDistinguishWhichToUse_ChoosesConstructorUsingMostParameters
            ()
        {
            ConstructorConfigurationElementTestClassWithGuidProperty constructorConfigurationElement =
                GenerateConstructorConfigurationElementTestClassWithGuidPropertyForType(
                    typeof (TestClassWithMultipleConstructors));

            int parameterValueA = Random.Next(short.MaxValue);
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "a",
                                                                   Value =
                                                                       parameterValueA.ToString(
                                                                           CultureInfo.InvariantCulture)
                                                               });

            int parameterValueB = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "b",
                                                                   Value =
                                                                       parameterValueB.ToString(
                                                                           CultureInfo.InvariantCulture),
                                                                   IsRequired = false
                                                               });

            int parameterValueC = Random.Next();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "c",
                                                                   Value =
                                                                       parameterValueC.ToString(
                                                                           CultureInfo.InvariantCulture),
                                                                   IsRequired = false
                                                               });

            TestClassWithMultipleConstructors instance =
                constructorConfigurationElement.GetInstance<TestClassWithMultipleConstructors>();

            Assert.AreEqual("Constructor with three parameters: a, b and c.", instance.ConstructorUsed);
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingOneArgumentWithDefaultValueAndNoValueIsSupplied_ConstructorUsesDefaultValue
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(
                    typeof (TestClassWithConstructorTakingArgumentWithDefaultValue));

            TestClassWithConstructorTakingArgumentWithDefaultValue instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingArgumentWithDefaultValue>();

            Assert.AreEqual(DefaultValueForOptionalArgument, instance.Value,
                            "When no parameter element exists to provide a value to an optional argument in the constructor, the default value should be used.");
        }

        [TestMethod]
        public void GetConstructor_TypeHasConstructorWithOutParam_ConstructorWithOutParamIsIgnored()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorWithOutParam));

            string parameterValue = Random.Next().ToString(CultureInfo.InvariantCulture);
            constructorConfigurationElement.Parameters.Add(new ParameterElement {Name = "value", Value = parameterValue});
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "outParam",
                                                                   Value = parameterValue,
                                                                   IsRequired = false
                                                               });

            Func<TestClassWithConstructorWithOutParam> constructor =
                constructorConfigurationElement.GetConstructor<TestClassWithConstructorWithOutParam>();

            // As the constructor with the out param is ignored, we should find the alternative has been used instead
            Assert.AreEqual("ConstructorWithoutOutParam", constructor().ConstructorUsed);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void GetConstructor_TypeHasConstructorWithRefParam_ThrowsInvalidOperationException()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorWithRefParam));

            string parameterValue = Random.Next().ToString(CultureInfo.InvariantCulture);
            constructorConfigurationElement.Parameters.Add(new ParameterElement {Name = "value", Value = parameterValue});
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "refParam",
                                                                   Value = parameterValue,
                                                                   IsRequired = false
                                                               });

            // An exception is thrown as no valid cosntructer could be found as a result of the exclusion
            constructorConfigurationElement.GetConstructor<TestClassWithConstructorWithRefParam>();
        }

        [TestMethod]
        public void
            GetInstance_TypeHasConstructorTakingTypeArgumentAndTypeNameConverterSpecifiedAsTypeConverter_ConvertsValueToTypeUsingTypeConverter
            ()
        {
            ConstructorConfigurationElement constructorConfigurationElement =
                GenerateConstructorConfigurationElementForType(typeof (TestClassWithConstructorTakingTypeArgument));

            Type testType =
                ChooseRandomTypeFromList(new List<Type> {typeof (int), typeof (string), typeof (decimal), typeof (Type)});
            string typeName = testType.ToString();
            constructorConfigurationElement.Parameters.Add(new ParameterElement
                                                               {
                                                                   Name = "type",
                                                                   Value = typeName,
                                                                   TypeConverter = typeof (TypeNameConverter)
                                                               });

            TestClassWithConstructorTakingTypeArgument instance =
                constructorConfigurationElement.GetInstance<TestClassWithConstructorTakingTypeArgument>();

            Assert.AreEqual(testType, instance.Type,
                            "Where a type converter is specified for a parameter, it should be used to convert the string representation of the value into the correct type.");
        }

        #region Nested type: ConstructorConfigurationElementTestClassWithGuidProperty
        private class ConstructorConfigurationElementTestClassWithGuidProperty : ConstructorConfigurationElement
        {
            [ConfigurationProperty("guid")]
            public Guid Guid
            {
                get { return GetProperty<Guid>("guid"); }
                set { SetProperty("guid", value); }
            }
        }
        #endregion

        #region Nested type: TestClassInheritingFromTestClassWithConstructorTakingOneGuid
        private class TestClassInheritingFromTestClassWithConstructorTakingOneGuid :
            TestClassWithConstructorTakingOneGuid
        {
            public TestClassInheritingFromTestClassWithConstructorTakingOneGuid(Guid guidForInheritingClass)
                : base(guidForInheritingClass)
            {
            }
        }
        #endregion

        #region Nested type: TestClassWithConstructorTakingArgumentWithDefaultValue
        private class TestClassWithConstructorTakingArgumentWithDefaultValue
        {
            public TestClassWithConstructorTakingArgumentWithDefaultValue(string value = DefaultValueForOptionalArgument)
            {
                Value = value;
            }

            public string Value { get; private set; }
        }
        #endregion

        #region Nested type: TestClassWithConstructorTakingOneGuid
        private class TestClassWithConstructorTakingOneGuid
        {
            public TestClassWithConstructorTakingOneGuid(Guid guid)
            {
                Guid = guid;
            }

            public Guid Guid { get; protected set; }
        }
        #endregion

        #region Nested type: TestClassWithConstructorTakingThreeInts
        private class TestClassWithConstructorTakingThreeInts
        {
            private TestClassWithConstructorTakingThreeInts(int a, int b, int c)
            {
                A = a;
                B = b;
                C = c;
            }

            public int A { private set; get; }
            public int B { private set; get; }
            public int C { private set; get; }
        }
        #endregion

        #region Nested type: TestClassWithConstructorTakingTypeArgument
        private class TestClassWithConstructorTakingTypeArgument
        {
            public TestClassWithConstructorTakingTypeArgument(Type type)
            {
                Type = type;
            }

            public Type Type { get; private set; }
        }
        #endregion

        #region Nested type: TestClassWithConstructorWithOutParam
        private class TestClassWithConstructorWithOutParam
        {
            public TestClassWithConstructorWithOutParam(string value, out string outParam)
            {
                Value = value;
                outParam = value;
                ConstructorUsed = "ConstructorWithOutParam";
            }

            public TestClassWithConstructorWithOutParam(string value)
            {
                Value = value;
                ConstructorUsed = "ConstructorWithoutOutParam";
            }

            public string Value { get; private set; }
            public string ConstructorUsed { get; private set; }
        }
        #endregion

        #region Nested type: TestClassWithConstructorWithRefParam
        private class TestClassWithConstructorWithRefParam
        {
            public TestClassWithConstructorWithRefParam(string value, ref string refParam)
            {
                Value = value;
                refParam = String.Format("{0}: {1}", value, refParam);
                    // this is a meaningless way to both use and set the ref param
            }

            public string Value { get; private set; }
        }
        #endregion

        #region Nested type: TestClassWithMultipleConstructors
        private class TestClassWithMultipleConstructors
        {
            public TestClassWithMultipleConstructors(int requiredParameter, int optionalParameter = -1)
            {
                A = requiredParameter;
                B = optionalParameter;
                ConstructorUsed = "Constructor with optionalParameter";
            }

            public TestClassWithMultipleConstructors(int requiredParameter)
            {
                A = requiredParameter;
                ConstructorUsed = "Constructor without optionalParameter";
            }

            public TestClassWithMultipleConstructors(short a, int b, int c)
            {
                A = a;
                B = b;
                C = c;
                ConstructorUsed = "Constructor with three parameters: a, b and c.";
            }

            public TestClassWithMultipleConstructors(short a, int b)
            {
                A = a;
                B = b;
                ConstructorUsed = "Constructor with two parameters: a, and b.";
            }

            public TestClassWithMultipleConstructors(short a, short d)
            {
                A = a;
                B = d;
                ConstructorUsed = "Constructor with two parameters: a and d.";
            }

            public int A { private set; get; }
            public int B { private set; get; }
            public int C { private set; get; }
            public string ConstructorUsed { private set; get; }
        }
        #endregion
    }
}