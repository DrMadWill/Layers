namespace DrMadWill.Layers.Core;

/// <summary>
/// Represents the base interface for DTOs (Data Transfer Objects) with an Id property.
/// </summary>
/// <typeparam name="Type">The type of the Id property.</typeparam>
public interface IBaseDto<Type>
{
    /// <summary>
    /// Gets or sets the unique identifier for the DTO.
    /// </summary>
    Type Id { get; set; }
}
