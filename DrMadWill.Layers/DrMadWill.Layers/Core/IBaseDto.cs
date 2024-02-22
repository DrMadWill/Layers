namespace DrMadWill.Layers.Core;

/// <summary>
/// Represents the base interface for DTOs (Data Transfer Objects) with an Id property.
/// </summary>
/// <typeparam name="T">The type of the Id property.</typeparam>
public interface IBaseDto<T>
{
    /// <summary>
    /// Gets or sets the unique identifier for the DTO.
    /// </summary>
    T Id { get; set; }
}
