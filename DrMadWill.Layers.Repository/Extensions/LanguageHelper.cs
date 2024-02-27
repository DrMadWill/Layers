using System.Diagnostics.CodeAnalysis;
using DrMadWill.Layers.Core.Abstractions;

namespace DrMadWill.Layers.Repository.Extensions
{
    /// <summary>
    /// Helper class for handling language-specific localization of DTOs.
    /// </summary>
    public static class LanguageHelper
    {
        /// <summary>
        /// Localizes a DTO by copying language-specific properties from an entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TDto">The DTO type.</typeparam>
        /// <param name="entity">The entity with language-specific properties.</param>
        /// <param name="dto">The DTO to be localized.</param>
        /// <param name="languageCode">The language code to determine which properties to copy.</param>
        /// <returns>The localized DTO.</returns>
        public static TDto GetLocalized<TEntity, TDto>([NotNull] TEntity entity, [NotNull] TDto dto, string languageCode)
            where TEntity : class
            where TDto : class
        {
            var dtoProps = dto.GetType().GetProperties();
            var properties = entity.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name.EndsWith($"_{languageCode}") && prop.GetValue(entity) is string value)
                {
                    var newPropName = prop.Name.Replace($"_{languageCode}", "");
                    var newProp = dtoProps.FirstOrDefault(p => p.Name == newPropName);

                    if (newProp != null)
                    {
                        newProp.SetValue(dto, value);
                    }
                }
            }

            return dto;
        }

        /// <summary>
        /// Localizes a list of DTOs by copying language-specific properties from corresponding entities.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TDto">The DTO type.</typeparam>
        /// <typeparam name="TPrimary">The type of the primary key.</typeparam>
        /// <param name="entities">The list of entities with language-specific properties.</param>
        /// <param name="dtos">The list of DTOs to be localized.</param>
        /// <param name="languageCode">The language code to determine which properties to copy.</param>
        /// <returns>The localized list of DTOs.</returns>
        public static IList<TDto> GetLocalizedList<TEntity, TDto, TPrimary>([NotNull] IList<TEntity> entities, [NotNull] IList<TDto> dtos, string languageCode)
            where TEntity : class, IOriginEntity<TPrimary>
            where TDto : class, IBaseDto<TPrimary>
        {
            foreach (var entity in entities)
                GetLocalized(entity, dtos.First(s => Equals(s.Id, entity.Id)), languageCode);
            return dtos;
        }
    }
}
