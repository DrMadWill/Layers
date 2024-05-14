using System.Reflection;

namespace DrMadWill.Layers.Core.Concretes;

public abstract class Enumeration : IComparable
{

    public string Name { get; set; }
    public int Id { get; set; }
    
    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration
        => typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(s => s.GetValue(null))
            .Cast<T>();


    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        var typeMatches = GetType().Equals(otherValue.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }
    

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        => Math.Abs(firstValue.Id - secondValue.Id);

    public static T FromValue<T>(int id) where T : Enumeration
        => Parse<T, int>(id, "display name", s => s.Id == id);
    
    public static T FromDisplayName<T>(string displayName) where T : Enumeration
        => Parse<T, string>(displayName, "display name", s => s.Name == displayName);

    protected static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchItem = GetAll<T>().FirstOrDefault(predicate);
        if (matchItem == null)
            throw new InvalidOperationException($"'{value}' is not valid {description} in {typeof(T)}");
        return matchItem;
    }
    
    public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other).Id);
}