using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Core.Extension {
    public static class EnumExtension {
        public static string GetDescription(Enum value) {
            DisplayAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault() as DisplayAttribute;
            return attribute == null ? value.ToString() : attribute.GetDescription();
        }

        public static T GetEnumValueFromDescription<T>(string description) {
            var type = typeof(T);
            if(!type.IsEnum)
                throw new ArgumentException();

            FieldInfo[] fields = type.GetFields();

            var field = fields
                .SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a })
                .Where(a => ((DescriptionAttribute)a.Att).Description == description).SingleOrDefault();

            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
    }
}
