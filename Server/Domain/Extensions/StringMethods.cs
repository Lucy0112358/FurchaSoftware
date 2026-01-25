using System.ComponentModel;

namespace Domain.Extensions
{
    public static class StringMethods
    {
        /// <summary>
        /// Checks if the string is NOT null and NOT empty
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string @string)
        {
            return !string.IsNullOrWhiteSpace(@string);
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return !source.Any(predicate);
        }

        /// <summary>
        /// Checks if the string is null or empty.
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string? @string)
        {
            return string.IsNullOrWhiteSpace(@string);
        }

        /// <summary>
        /// Gets the description value of the DescriptionAttribute or empty string if not defined.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var description = value.GetAttribute<DescriptionAttribute>();
            if (description != null && description.Description.Length > 0)
            {
                return description.Description;
            }
            else
            {
                return string.Empty;
            }
        }

        private static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

    }
}
