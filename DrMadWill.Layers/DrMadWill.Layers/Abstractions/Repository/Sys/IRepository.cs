using DrMadWill.Layers.Core;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Abstractions.Repository.Sys;

public interface IRepository<TEntity, in TPrimary> : IDisposable
    where TEntity : class, IBaseEntity<TPrimary>, new()
{
    DbSet<TEntity> Table { get; }
}