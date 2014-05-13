using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.CustomMarshalers;
using System.Text;

namespace RabbitMQ
{
    public static class ComSerializer
    {
        public static string ToJson(object @object)
        {
            var json = new StringBuilder();
            Map(@object, GetType((IDispatchInfo)@object), json);
            return json.ToString();
        }

        private static void Map(dynamic source, Type type, StringBuilder json)
        {
            json.Append("{");
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = source[property.Name];
                var valueType = value.GetType();
                File.AppendAllText(@"c:\temp\object.txt", property.Name + ": " + valueType.Name + "\r\n");
                if (valueType.IsPrimitive || valueType == typeof(string) ||
                    valueType == typeof(DateTime) || valueType == typeof(decimal))
                    json.Append(source.Format("\"{0}\":\"{1}\",", property.Name, value));
                else
                {
                    json.Append(source.Format("\"{0}\":", property.Name));
                    Map(value, GetType((IDispatchInfo)value), json);
                    json.Append(",");
                }
            }
            json.Append("}");
        }

        private const int Ok = 0;
        private const int LocaleSystemDefault = 2 << 10;

        public static Type GetType(IDispatchInfo dispatch)
        {
            int typeInfoCount;
            var result = dispatch.GetTypeInfoCount(out typeInfoCount);
            Type type;
            if (result == Ok && typeInfoCount > 0)
            {
                dispatch.GetTypeInfo(0, LocaleSystemDefault, out type);
            }
            else
            {
                Marshal.ThrowExceptionForHR(result);
                throw new TypeLoadException();
            }
            return type;
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00020400-0000-0000-C000-000000000046")]
        public interface IDispatchInfo
        {
            [PreserveSig]
            int GetTypeInfoCount(out int typeInfoCount);

            void GetTypeInfo(int typeInfoIndex, int lcid,
                [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef =
                typeof(TypeToTypeInfoMarshaler))] out Type typeInfo);
        }
    }
}
