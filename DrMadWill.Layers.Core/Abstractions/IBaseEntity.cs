namespace DrMadWill.Layers.Core.Abstractions
{
    /// <summary>
    /// Represents the base interface for entities in the application.
    /// </summary>
    /// <typeparam name="T">The type of the Id property.</typeparam>
    public interface IBaseEntity<T> : IOriginEntity<T>,IHasDelete
    {
        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated (nullable).
        /// </summary>
        DateTime? UpdatedDate { get; set; }
    }
}