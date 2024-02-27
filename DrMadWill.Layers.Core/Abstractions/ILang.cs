namespace DrMadWill.Layers.Core.Abstractions
{
    /// <summary>
    /// Represents an interface for entities or objects that have a language-related property.
    /// </summary>
    public interface ILang
    {
        /// <summary>
        /// Gets or sets the language associated with the entity or object.
        /// </summary>
        string? Lang { get; set; }
    }
}