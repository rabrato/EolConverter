using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.ComponentModel
{
    public static class EnumExtensions
    {
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
