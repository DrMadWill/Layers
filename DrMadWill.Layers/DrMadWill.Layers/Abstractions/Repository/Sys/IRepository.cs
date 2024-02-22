using DrMadWill.Layers.Core;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Abstractions.Repository.Sys;

public interface IRepository<TEntity, in TPrimary> : IDisposable
    where TEntity : class, IOriginEntity<TPrimary>,new()
{
    DbSet<TEntity> Table { get; }
}