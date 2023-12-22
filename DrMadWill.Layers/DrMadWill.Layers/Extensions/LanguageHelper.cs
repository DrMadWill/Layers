using System.Diagnostics.CodeAnalysis;
using DrMadWill.Layers.Core;

namespace DrMadWill.Layers.Extensions;

public static class LanguageHelper
{
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

    public static IList<TDto> GetLocalizedList<TEntity, TDto, TPrimary>([NotNull] IList<TEntity> entities, [NotNull] IList<TDto> dtos, string languageCode)
        where TEntity : class, IBaseEntity<TPrimary>
        where TDto : class, IBaseDto<TPrimary>
    {
        foreach (var entity in entities)
            GetLocalized(entity, dtos.First(s => Equals(s.Id, entity.Id)), languageCode);
        return dtos;
    }
}