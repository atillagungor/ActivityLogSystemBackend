using Core.DataAccess.Repositories;
using DataAccess.Abstracts;
using DataAccess.Contexts;
using Entities.Concretes.CrossTables;

namespace DataAccess.Concretes.EntityFramework;
public class EfUserOperationClaimDal : EfRepositoryBase<UserOperationClaim, Guid, TobetoLogContext>, IUserOperationClaimDal
{
    public EfUserOperationClaimDal(TobetoLogContext context) : base(context)
    {
    }
}