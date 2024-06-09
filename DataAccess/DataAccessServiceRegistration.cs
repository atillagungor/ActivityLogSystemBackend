using DataAccess.Abstracts;
using DataAccess.Concretes.EntityFramework;
using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;
public static class DataAccessServiceRegistration
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        

        services.AddDbContext<TobetoLogContext>(options => options.UseSqlServer(configuration.GetConnectionString("TobetoLogContext"))); 
        
        services.AddScoped<IUserDal, EfUserDal>();
        services.AddScoped<IOperationClaimDal, EfOperationClaimDal>();
        services.AddScoped<IUserOperationClaimDal, EfUserOperationClaimDal>();

        return services;
    }
}