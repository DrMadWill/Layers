namespace DrMadWill.Layers.Abstractions.Repository.Core;

public interface IBaseDto<Type>
{
    public Type Id { get; set; }
}