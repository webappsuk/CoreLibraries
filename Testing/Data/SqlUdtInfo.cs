// Type: System.Data.SqlClient.SqlUdtInfo
// Assembly: System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Data.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;

namespace WebApplications.Testing.Data
{
    public class SqlUdtInfo
    {
        public readonly Format SerializationFormat;
        public readonly bool IsByteOrdered;
        public readonly bool IsFixedLength;
        public readonly int MaxByteSize;
        public readonly string Name;
        public readonly string ValidationMethodName;
        [ThreadStatic]
        private static Dictionary<Type, SqlUdtInfo> m_types2UdtInfo;

        private SqlUdtInfo(SqlUserDefinedTypeAttribute attr)
        {
            this.SerializationFormat = attr.Format;
            this.IsByteOrdered = attr.IsByteOrdered;
            this.IsFixedLength = attr.IsFixedLength;
            this.MaxByteSize = attr.MaxByteSize;
            this.Name = attr.Name;
            this.ValidationMethodName = attr.ValidationMethodName;
        }

        internal static SqlUdtInfo GetFromType(Type target)
        {
            SqlUdtInfo fromType = SqlUdtInfo.TryGetFromType(target);
            if (fromType == null)
                throw new InvalidOperationException();
            else
                return fromType;
        }

        internal static SqlUdtInfo TryGetFromType(Type target)
        {
            if (SqlUdtInfo.m_types2UdtInfo == null)
                SqlUdtInfo.m_types2UdtInfo = new Dictionary<Type, SqlUdtInfo>();
            SqlUdtInfo sqlUdtInfo = (SqlUdtInfo)null;
            if (!SqlUdtInfo.m_types2UdtInfo.TryGetValue(target, out sqlUdtInfo))
            {
                object[] customAttributes = target.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
                if (customAttributes != null && customAttributes.Length == 1)
                    sqlUdtInfo = new SqlUdtInfo((SqlUserDefinedTypeAttribute)customAttributes[0]);
                SqlUdtInfo.m_types2UdtInfo.Add(target, sqlUdtInfo);
            }
            return sqlUdtInfo;
        }
    }
}
