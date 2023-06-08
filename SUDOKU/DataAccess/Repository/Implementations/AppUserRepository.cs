using Core.Entities;
using DataAccess.Contexts;
using DataAccess.Repository.Interfaces;

namespace DataAccess.Repository.Implementations;

public class AppUserRepository : Repository<AppUser>, IAppUserRepository
{
	public AppUserRepository(AppDbContext context) : base(context)
	{
	}
}
