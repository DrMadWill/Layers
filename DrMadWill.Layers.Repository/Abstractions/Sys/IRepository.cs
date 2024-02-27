using DrMadWill.Layers.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Repository.Abstractions.Sys;

public interface IRepository<TEntity, in TPrimary> : IDisposable
    where TEntity : class, IOriginEntity<TPrimary>,new()
{
    DbSet<TEntity> Table { get; }
}