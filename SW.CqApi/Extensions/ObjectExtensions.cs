
using System;


namespace SW.CqApi
{
    public static class Object
    {

        private static Type GetNullableType(Type type)
        {
            if (type == typeof(string)) return typeof(string);
            if (type == typeof(object)) return typeof(object);
            return Nullable.GetUnderlyingType(type);
        }

        //public static object ConvertValue(object value, Type type)
        //{

        //    bool typeNullable = (GetNullableType(type) == null) ? false : true;

        //    if (value is null && typeNullable) return null;

        //    else if (value is null) return Activator.CreateInstance(type);

        //    if (value.GetType() == type) return value;

        //    var t = GetNullableType(type);
        //    if (t != null)
        //    {
        //        type = t;
        //        if (string.IsNullOrEmpty(value.ToString())) return null;
        //    }

        //    return Convert.ChangeType(value, type);
        //}

        public static object ConvertValueToType(object value, Type type)
        {

            bool typeNullable = (GetNullableType(type) == null) ? false : true;

            if (value is null && typeNullable) return null;

            else if (value is null) return Activator.CreateInstance(type);

            if (value.GetType() == type) return value;
            if (type.IsAssignableFrom(value.GetType())) return value;

            var nakedType = GetNullableType(type);
            if (nakedType != null)
            {
                if (string.IsNullOrWhiteSpace(value.ToString())) return null;

                if (nakedType.IsEnum) return Enum.Parse(nakedType, value.ToString());
                return Convert.ChangeType(value, nakedType);
            }

            if (type.IsEnum) return Enum.Parse(type, value.ToString());
            return Convert.ChangeType(value, type);
        }

        //public static bool TryConvertValue(object value, Type type, ref object newObject)
        //{
        //    try
        //    {
        //        newObject = ConvertValueToType(value, type);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}



    }
}
