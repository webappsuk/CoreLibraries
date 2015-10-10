#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Reflect;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable PossibleNullReferenceException
// ReSharper disable ObjectCreationAsStatement

namespace WebApplications.Utilities.Test.Reflect
{
    [TestClass]
    public class InfoHelperTests : UtilitiesTestBase
    {
        private InfoHelperClass _helper;

        #region GetMethodInfo(Action)
        [TestMethod]
        public void GetMethodInfo_Action_NoArgs()
        {
            MethodInfo info = InfoHelper.GetMethodInfo(() => InfoHelperClass.StaticVoidMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticVoidMethod");
        }

        [TestMethod]
        public void GetMethodInfo_Action_Args()
        {
            MethodInfo info = InfoHelper.GetMethodInfo(
                () => InfoHelperClass.StaticVoidArgsMethod(default(string), default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticVoidArgsMethod");
        }

        [TestMethod]
        public void GetMethodInfo_Action_OutArgs()
        {
            int a = 0, b;
            MethodInfo info = InfoHelper.GetMethodInfo(() => InfoHelperClass.StaticOutMethod(ref a, out b));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticOutMethod");
        }

        [TestMethod]
        public void GetMethodInfo_Action_OutArgsHelper()
        {
            MethodInfo info = InfoHelper.GetMethodInfo(
                () => InfoHelperClass.StaticOutMethod(ref InfoHelper<int>.RefOrOut, out InfoHelper<int>.RefOrOut));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticOutMethod");
        }

        [TestMethod]
        public void GetMethodInfo_Action_Invalid()
        {
            Expression<Action> exp = () => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMethodInfo_Action_InvalidThrowIfNotFoundThrows()
        {
            Expression<Action> exp = () => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetMethodInfo(Action<TInstance>)
        [TestMethod]
        public void GetMethodInfo_ActionInstance_NoArgs()
        {
            MethodInfo info = InfoHelper.GetMethodInfo<InfoHelperClass>(i => i.VoidMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "VoidMethod");
        }

        [TestMethod]
        public void GetMethodInfo_ActionInstance_Args()
        {
            MethodInfo info = InfoHelper.GetMethodInfo<InfoHelperClass>(
                i => i.VoidArgsMethod(default(string), default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "VoidArgsMethod");
        }

        [TestMethod]
        public void GetMethodInfo_ActionInstance_OutArgs()
        {
            int a = 0, b;
            MethodInfo info = InfoHelper.GetMethodInfo<InfoHelperClass>(i => i.OutMethod(ref a, out b));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "OutMethod");
        }

        [TestMethod]
        public void GetMethodInfo_ActionInstance_OutArgsHelper()
        {
            MethodInfo info = InfoHelper.GetMethodInfo<InfoHelperClass>(
                i => i.OutMethod(ref InfoHelper<int>.RefOrOut, out InfoHelper<int>.RefOrOut));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "OutMethod");
        }

        [TestMethod]
        public void GetMethodInfo_ActionInstance_Invalid()
        {
            Expression<Action<InfoHelperClass>> exp = i => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMethodInfo_ActionInstance_InvalidThrowIfNotFoundThrows()
        {
            Expression<Action<InfoHelperClass>> exp = i => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetMethodInfo(TInstance,Action<TInstance>)
        [TestMethod]
        public void GetMethodInfo_InstanceActionInstance_NoArgs()
        {
            MethodInfo info = _helper.GetMethodInfo(i => i.VoidMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "VoidMethod");
        }

        [TestMethod]
        public void GetMethodInfo_InstanceActionInstance_Args()
        {
            MethodInfo info = _helper.GetMethodInfo(i => i.VoidArgsMethod(default(string), default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "VoidArgsMethod");
        }

        [TestMethod]
        public void GetMethodInfo_InstanceActionInstance_Invalid()
        {
            Expression<Action<InfoHelperClass>> exp = i => new InfoHelperClass();
            MethodInfo info = _helper.GetMethodInfo(exp);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMethodInfo_InstanceActionInstance_InvalidThrowIfNotFoundThrows()
        {
            Expression<Action<InfoHelperClass>> exp = i => new InfoHelperClass();
            MethodInfo info = _helper.GetMethodInfo(exp, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetMethodInfo(Func<object>)
        [TestMethod]
        public void GetMethodInfo_Func_String()
        {
            MethodInfo info = InfoHelper.GetMethodInfo(() => InfoHelperClass.StaticStringMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticStringMethod");
        }

        [TestMethod]
        public void GetMethodInfo_Func_Int()
        {
            MethodInfo info = InfoHelper.GetMethodInfo(() => InfoHelperClass.StaticIntMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticIntMethod");
        }

        [TestMethod]
        public void GetMethodInfo_Func_Invalid()
        {
            Expression<Func<object>> exp = () => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMethodInfo_Func_InvalidThrowIfNotFoundThrows()
        {
            Expression<Func<object>> exp = () => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetMethodInfo(Func<TInstance,object>)
        [TestMethod]
        public void GetMethodInfo_FuncInstance_String()
        {
            MethodInfo info = InfoHelper.GetMethodInfo<InfoHelperClass>(i => i.StringMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StringMethod");
        }

        [TestMethod]
        public void GetMethodInfo_FuncInstance_Int()
        {
            MethodInfo info = InfoHelper.GetMethodInfo<InfoHelperClass>(i => i.IntMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "IntMethod");
        }
        
        [TestMethod]
        public void GetMethodInfo_FuncInstance_Invalid()
        {
            Expression<Func<InfoHelperClass, object>> exp = i => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMethodInfo_FuncInstance_InvalidThrowIfNotFoundThrows()
        {
            Expression<Func<InfoHelperClass, object>> exp = i => new InfoHelperClass();
            MethodInfo info = InfoHelper.GetMethodInfo(exp, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetMethodInfo(TInstance,Func<TInstance,TResult>)
        [TestMethod]
        public void GetMethodInfo_InstanceFuncInstance_String()
        {
            MethodInfo info = _helper.GetMethodInfo(i => i.StringMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StringMethod");
        }

        [TestMethod]
        public void GetMethodInfo_InstanceFuncInstance_Int()
        {
            MethodInfo info = _helper.GetMethodInfo(i => i.IntMethod());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "IntMethod");
        }

        [TestMethod]
        public void GetMethodInfo_InstanceFuncInstance_Invalid()
        {
            Expression<Func<InfoHelperClass, object>> exp = i => new InfoHelperClass();
            MethodInfo info = _helper.GetMethodInfo(exp);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMethodInfo_InstanceFuncInstance_InvalidThrowIfNotFoundThrows()
        {
            Expression<Func<InfoHelperClass, object>> exp = i => new InfoHelperClass();
            MethodInfo info = _helper.GetMethodInfo(exp, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetConstructorInfo
        [TestMethod]
        public void GetConstructorInfo_NoArgs()
        {
            ConstructorInfo info = InfoHelper.GetConstructorInfo(() => new InfoHelperClass());
            Assert.IsNotNull(info);
            Assert.AreEqual(info.GetParameters().Length, 0);
        }

        [TestMethod]
        public void GetConstructorInfo_Args()
        {
            ConstructorInfo info =
                InfoHelper.GetConstructorInfo(() => new InfoHelperClass(default(string), default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.GetParameters().Length, 2);
        }

        [TestMethod]
        public void GetConstructorInfo_Invalid()
        {
            ConstructorInfo info = InfoHelper.GetConstructorInfo(() => _helper);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetConstructorInfo_InvalidThrowIfNotFoundThrows()
        {
            ConstructorInfo info = InfoHelper.GetConstructorInfo(() => _helper, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetPropertyInfo(Func<TInstance, object>)
        [TestMethod]
        public void GetPropertyInfo_Instance_String()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo<InfoHelperClass>(i => i.StringProperty);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StringProperty");
        }

        [TestMethod]
        public void GetPropertyInfo_Instance_Int()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo<InfoHelperClass>(i => i.IntProperty);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "IntProperty");
        }

        [TestMethod]
        public void GetPropertyInfo_Instance_Invalid()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo<InfoHelperClass>(i => i.IntField);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyInfo_Instance_InvalidThrowIfNotFoundThrows()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo<InfoHelperClass>(i => i.IntField, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetPropertyInfo(Func<TResult>)
        [TestMethod]
        public void GetPropertyInfo_Result_String()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo(() => InfoHelperClass.StaticStringProperty);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticStringProperty");
        }

        [TestMethod]
        public void GetPropertyInfo_Result_Int()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo(() => InfoHelperClass.StaticIntProperty);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticIntProperty");
        }

        [TestMethod]
        public void GetPropertyInfo_Result_Invalid()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo(() => InfoHelperClass.StaticIntField);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyInfo_Result_InvalidThrowIfNotFoundThrows()
        {
            PropertyInfo info = InfoHelper.GetPropertyInfo(() => InfoHelperClass.StaticIntField, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetPropertyInfo(TInstance, Func<TInstance, TResult>)
        [TestMethod]
        public void GetPropertyInfo_InstanceResult_String()
        {
            PropertyInfo info = _helper.GetPropertyInfo(i => i.StringProperty);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StringProperty");
        }

        [TestMethod]
        public void GetPropertyInfo_InstanceResult_Int()
        {
            PropertyInfo info = _helper.GetPropertyInfo(i => i.IntProperty);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "IntProperty");
        }

        [TestMethod]
        public void GetPropertyInfo_InstanceResult_Invalid()
        {
            PropertyInfo info = _helper.GetPropertyInfo(i => i.IntField);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyInfo_InstanceResult_InvalidThrowIfNotFoundThrows()
        {
            PropertyInfo info = _helper.GetPropertyInfo(i => i.IntField, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetFieldInfo(Func<TInstance, object>)
        [TestMethod]
        public void GetFieldInfo_Instance_String()
        {
            FieldInfo info = InfoHelper.GetFieldInfo<InfoHelperClass>(i => i.StringField);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StringField");
        }

        [TestMethod]
        public void GetFieldInfo_Instance_Int()
        {
            FieldInfo info = InfoHelper.GetFieldInfo<InfoHelperClass>(i => i.IntField);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "IntField");
        }

        [TestMethod]
        public void GetFieldInfo_Instance_Invalid()
        {
            FieldInfo info = InfoHelper.GetFieldInfo<InfoHelperClass>(i => i.IntProperty);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFieldInfo_Instance_InvalidThrowIfNotFoundThrows()
        {
            FieldInfo info = InfoHelper.GetFieldInfo<InfoHelperClass>(i => i.IntProperty, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetFieldInfo(Func<TResult>)
        [TestMethod]
        public void GetFieldInfo_Result_String()
        {
            FieldInfo info = InfoHelper.GetFieldInfo(() => InfoHelperClass.StaticStringField);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticStringField");
        }

        [TestMethod]
        public void GetFieldInfo_Result_Int()
        {
            FieldInfo info = InfoHelper.GetFieldInfo(() => InfoHelperClass.StaticIntField);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StaticIntField");
        }

        [TestMethod]
        public void GetFieldInfo_Result_Invalid()
        {
            FieldInfo info = InfoHelper.GetFieldInfo(() => InfoHelperClass.StaticIntProperty);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFieldInfo_Result_InvalidThrowIfNotFoundThrows()
        {
            FieldInfo info = InfoHelper.GetFieldInfo(() => InfoHelperClass.StaticIntProperty, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetFieldInfo(TInstance, Func<TInstance, TResult>)
        [TestMethod]
        public void GetFieldInfo_InstanceResult_String()
        {
            FieldInfo info = _helper.GetFieldInfo(i => i.StringField);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "StringField");
        }

        [TestMethod]
        public void GetFieldInfo_InstanceResult_Int()
        {
            FieldInfo info = _helper.GetFieldInfo(i => i.IntField);
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "IntField");
        }

        [TestMethod]
        public void GetFieldInfo_InstanceResult_Invalid()
        {
            FieldInfo info = _helper.GetFieldInfo(i => i.IntProperty);
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFieldInfo_InstanceResult_InvalidThrowIfNotFoundThrows()
        {
            FieldInfo info = _helper.GetFieldInfo(i => i.IntProperty, true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetParameterInfo(Action<TParam>)
        [TestMethod]
        public void GetParameterInfo_Static_VoidFirstArg()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<string>(p => InfoHelperClass.StaticVoidArgsMethod(p, default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "sarg");
            Assert.AreEqual(info.ParameterType, typeof(string));
        }

        [TestMethod]
        public void GetParameterInfo_Static_VoidSecondArg()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticVoidArgsMethod(default(string), p));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "iarg");
            Assert.AreEqual(info.ParameterType, typeof(int));
        }

        [TestMethod]
        public void GetParameterInfo_Static_VoidFirstArgHelper()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<string>(p => InfoHelperClass.StaticVoidArgsMethod(InfoHelper<string>.Parameter, default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "sarg");
            Assert.AreEqual(info.ParameterType, typeof(string));
        }

        [TestMethod]
        public void GetParameterInfo_Static_VoidSecondArgHelper()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticVoidArgsMethod(default(string), InfoHelper<int>.Parameter));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "iarg");
            Assert.AreEqual(info.ParameterType, typeof(int));
        }

        [TestMethod]
        public void GetParameterInfo_Static_Ref()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticOutMethod(ref InfoHelper<int>.Parameter, out InfoHelper<int>.RefOrOut));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "a");
            Assert.AreEqual(info.ParameterType, typeof(int).MakeByRefType());
        }

        [TestMethod]
        public void GetParameterInfo_Static_Out()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticOutMethod(ref InfoHelper<int>.RefOrOut, out InfoHelper<int>.Parameter));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "b");
            Assert.AreEqual(info.ParameterType, typeof(int).MakeByRefType());
        }

        [TestMethod]
        public void GetParameterInfo_Static_FirstArg()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<string>(p => InfoHelperClass.StaticStringArgsMethod(p, default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "sarg");
            Assert.AreEqual(info.ParameterType, typeof(string));
        }

        [TestMethod]
        public void GetParameterInfo_Static_SecondArg()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticStringArgsMethod(default(string), p));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "iarg");
            Assert.AreEqual(info.ParameterType, typeof(int));
        }

        [TestMethod]
        public void GetParameterInfo_Static_Invalid_NotUsed()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticVoidMethod());
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetParameterInfo_Static_Invalid_UsedMultiple()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticSameArgsMethod(p, p, p));
            Assert.Fail("Didn't throw");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetParameterInfo_Static_InvalidThrowIfNotFoundThrows()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<int>(p => InfoHelperClass.StaticVoidMethod(), true);
            Assert.Fail("Didn't throw");
        }
        #endregion

        #region GetParameterInfo(Action<TInstance,TParam>)
        [TestMethod]
        public void GetParameterInfo_Instance_VoidFirstArg()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<InfoHelperClass, string>((i, p) => i.VoidArgsMethod(p, default(int)));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "sarg");
            Assert.AreEqual(info.ParameterType, typeof(string));
        }

        [TestMethod]
        public void GetParameterInfo_Instance_VoidSecondArg()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<InfoHelperClass, int>((i, p) => i.VoidArgsMethod(default(string), p));
            Assert.IsNotNull(info);
            Assert.AreEqual(info.Name, "iarg");
            Assert.AreEqual(info.ParameterType, typeof(int));
        }

        [TestMethod]
        public void GetParameterInfo_Instance_Invalid()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<InfoHelperClass, string>((i, p) => i.VoidMethod());
            Assert.IsNull(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetParameterInfo_Instance_InvalidThrowIfNotFoundThrows()
        {
            ParameterInfo info = InfoHelper.GetParameterInfo<InfoHelperClass, string>((i, p) => i.VoidMethod(), true);
            Assert.Fail("Didn't throw");
        }
        #endregion
    }

    public class InfoHelperClass
    {
        public static string StaticStringField;
        public static int StaticIntField;

        public static string StaticStringProperty { get; set; }
        public static int StaticIntProperty { get; set; }

        public static void StaticVoidMethod()
        {
        }

        public static void StaticVoidArgsMethod(string sarg, int iarg)
        {
        }

        public static string StaticStringArgsMethod(string sarg, int iarg)
        {
            return null;
        }

        public static void StaticSameArgsMethod(int arg1, int arg2, int arg3)
        {
        }

        public static string StaticStringMethod()
        {
            return null;
        }

        public static int StaticIntMethod()
        {
            return 0;
        }

        public static void StaticOutMethod(ref int a, out int b)
        {
            b = 0;
        }

        public InfoHelperClass()
        {
        }

        public InfoHelperClass(string sarg, int iarg)
        {
        }

        public string StringField;
        public int IntField;
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }

        public void VoidMethod()
        {
            
        }
        
        public string StringMethod()
        {
            return null;
        }

        public int IntMethod()
        {
            return 0;
        }

        public void VoidArgsMethod(string sarg, int iarg)
        {

        }

        public void OutMethod(ref int a, out int b)
        {
            b = 0;
        }
    }
}