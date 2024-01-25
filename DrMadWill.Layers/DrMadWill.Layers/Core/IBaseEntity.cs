namespace DrMadWill.Layers.Core
{
    /// <summary>
    /// Represents the base interface for entities in the application.
    /// </summary>
    /// <typeparam name="Type">The type of the Id property.</typeparam>
    public interface IBaseEntity<Type>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        Type? Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated (nullable).
        /// </summary>
        DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is marked as deleted (nullable).
        /// </summary>
        bool? IsDeleted { get; set; }
    }
}