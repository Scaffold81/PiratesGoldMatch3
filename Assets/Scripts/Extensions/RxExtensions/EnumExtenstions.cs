using System;
using System.ComponentModel;
using System.Reflection;

namespace Core.Extensions
{
    public static class EnumExtenstions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name).GetCustomAttribute<TAttribute>();
        }

        public static string GetDescription<T>(this T type) where T : Enum
        {
            var attribute = type.GetAttribute<DescriptionAttribute>();
            if (attribute == null)
            {
                return type.ToString();
            }

            return attribute.Description;
        }
    }
}
