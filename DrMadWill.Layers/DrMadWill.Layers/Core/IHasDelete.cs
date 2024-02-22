namespace DrMadWill.Layers.Core;

public interface IHasDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted (nullable).
    /// </summary>
    bool? IsDeleted { get; set; }
}