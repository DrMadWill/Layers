using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DrMadWill.Layers.Extensions;

public static class LayerUtils
{
    public static bool NotNull(this string? val)
    {
        return !string.IsNullOrEmpty(val);
    }
    public static bool NotZero(this int val)
    {
        return val != 0;
    } 
    public static bool NotNull(this int? val)
    {
        return val != null;
    }
    /// <summary>
    /// Minimal date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime Zero()
    {
        return new DateTime(1990, 1, 1);
    }
    
    /// <summary>
    /// Generate Json String From Objet | Datani stringe cevirme
    /// </summary>
    /// <param name="obj">Parsing Object</param>
    /// <returns>object string</returns>
    public static string JsonString(this object obj)
    {
        return Regex.Unescape(JsonConvert.SerializeObject(obj));
    }
    
}