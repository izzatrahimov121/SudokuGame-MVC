using Core.Entities;
using DataAccess.Contexts;
using DataAccess.Repository.Interfaces;

namespace DataAccess.Repository.Implementations;

public class WorldRaytingRepository : Repository<WorldRayting>,IWorldRaytingRepository
{
	public WorldRaytingRepository(AppDbContext context) : base(context)
	{
	}
}
