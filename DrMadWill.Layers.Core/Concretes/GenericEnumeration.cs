using System.Reflection;

namespace DrMadWill.Layers.Core.Concretes;

public abstract class GenericEnumeration<TId>
{
    private string _name { get; set; }
    public TId Id { get; set; }
    
    protected GenericEnumeration(TId id, string name) => (Id, _name) = (id, name);

    public override string ToString() => _name;

    public static IEnumerable<T> GetAll<T>()
        where T : GenericEnumeration<TId>
        => typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(s => s.GetValue(null))
            .Cast<T>();


    public override bool Equals(object? obj)
    {
        if (obj is not GenericEnumeration<TId> otherValue)
            return false;

        var typeMatches = GetType().Equals(otherValue.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }
    

    public override int GetHashCode() => Id.GetHashCode();


    public static T FromValue<T>(TId id) where T : GenericEnumeration<TId>
        => Parse<T, TId>(id, "display name", s => s.Id.Equals(id));
    
    public static T FromDisplayName<T>(string displayName) where T : GenericEnumeration<TId>
        => Parse<T, string>(displayName, "display name", s => s._name == displayName);

    protected static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate) where T : GenericEnumeration<TId>
    {
        var matchItem = GetAll<T>().FirstOrDefault(predicate);
        if (matchItem == null)
            throw new InvalidOperationException($"'{value}' is not valid {description} in {typeof(T)}");
        return matchItem;
    }
}