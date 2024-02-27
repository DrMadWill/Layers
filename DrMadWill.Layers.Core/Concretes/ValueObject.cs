namespace DrMadWill.Layers.Core.Concretes;

public abstract class ValueObject 
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            return false;
        return ReferenceEquals(left, null) || left.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        => !EqualOperator(left, right);
    
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetHashCode() != GetHashCode())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(s => s != null ? s.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public ValueObject? GetCopy() => MemberwiseClone() as ValueObject;
}