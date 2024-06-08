using Entities.Concretes;
using Entities.Concretes.CrossTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.Reflection;

namespace DataAccess.Contexts;

public class TobetoPlatformContext:DbContext
{
    protected IConfiguration Configuration { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

    public TobetoPlatformContext(DbContextOptions options, IConfiguration configuration):base(options)
    {
        Configuration = configuration;
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}