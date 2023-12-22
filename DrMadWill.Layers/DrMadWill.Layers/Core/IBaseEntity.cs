namespace DrMadWill.Layers.Core;

public interface IBaseEntity<Type>
{
    Type? Id { get; set; }
    DateTime CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
    bool? IsDeleted { get; set; }
}