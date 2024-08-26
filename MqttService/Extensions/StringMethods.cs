namespace MqttService.Extensions
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

        /// <summary>
        /// Checks if the string is null or empty.
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string? @string)
        {
            return string.IsNullOrWhiteSpace(@string);
        }

    }
}
