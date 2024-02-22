using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DrMadWill.Layers.Extensions
{
    /// <summary>
    /// Utility class for common extension methods.
    /// </summary>
    public static class LayerUtils
    {
        /// <summary>
        /// Checks if a string is not null or empty.
        /// </summary>
        /// <param name="val">The string to check.</param>
        /// <returns>True if the string is not null or empty; otherwise, false.</returns>
        public static bool NotNull(this string? val)
        {
            return !string.IsNullOrEmpty(val);
        }

        /// <summary>
        /// Checks if an integer is not zero.
        /// </summary>
        /// <param name="val">The integer to check.</param>
        /// <returns>True if the integer is not zero; otherwise, false.</returns>
        public static bool NotZero(this int val)
        {
            return val != 0;
        }

        /// <summary>
        /// Checks if an integer is not null.
        /// </summary>
        /// <param name="val">The nullable integer to check.</param>
        /// <returns>True if the integer is not null; otherwise, false.</returns>
        public static bool NotNull(this int? val)
        {
            return val != null;
        }

        /// <summary>
        /// Checks if a Guid is not null.
        /// </summary>
        /// <param name="val">The nullable Guid to check.</param>
        /// <returns>True if the Guid is not null; otherwise, false.</returns>
        public static bool NotNull(this Guid? val)
        {
            return val != null;
        }

        /// <summary>
        /// Checks if a DateTime is not null.
        /// </summary>
        /// <param name="val">The nullable DateTime to check.</param>
        /// <returns>True if the DateTime is not null; otherwise, false.</returns>
        public static bool NotNull(this DateTime? val)
        {
            return val != null;
        }

        /// <summary>
        /// Checks if an object is not null.
        /// </summary>
        /// <param name="val">The nullable object to check.</param>
        /// <returns>True if the object is not null; otherwise, false.</returns>
        public static bool NotNull(this object? val)
        {
            return val != null;
        }

        /// <summary>
        /// Returns a minimal date (January 1, 1990).
        /// </summary>
        /// <returns>A DateTime representing the minimal date.</returns>
        public static DateTime Zero()
        {
            return new DateTime(1990, 1, 1);
        }

        /// <summary>
        /// Generates a JSON string representation of an object.
        /// </summary>
        /// <param name="obj">The object to serialize to JSON.</param>
        /// <returns>A JSON string representing the object.</returns>
        public static string JsonString(this object obj)
        {
            return Regex.Unescape(JsonConvert.SerializeObject(obj,new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
    }
}
