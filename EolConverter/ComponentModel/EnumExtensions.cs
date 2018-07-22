using System;

namespace EolConverter.ComponentModel
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns an attribute from the enum value.
        /// </summary>
        /// <typeparam name="T">The type of the required attribute.</typeparam>
        /// <param name="enumValue">The value of the enum where to look for the attribute</param>
        /// <returns>The attribute of the enum value.</returns>
        public static T GetAttribute<T>(this Enum enumValue)
            where T : Attribute
        {
            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return ((T)attributes[0]);
        }
    }
}
