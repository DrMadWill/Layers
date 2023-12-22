namespace DrMadWill.Layers.Core;

public interface IBaseDto<Type>
{
    public Type Id { get; set; }
}