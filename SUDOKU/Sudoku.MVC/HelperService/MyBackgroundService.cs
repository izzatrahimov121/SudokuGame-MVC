




using Core.Entities;
using DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Sudoku.MVC.HelperService;

public class MyBackgroundService : BackgroundService
{
	private readonly ILogger<MyBackgroundService> _logger;
	private readonly IServiceProvider _services;
	public MyBackgroundService(ILogger<MyBackgroundService> logger,
							   IServiceProvider services)
	{
		_logger = logger;
		_services = services;
	}


	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				_logger.LogInformation("MyBackgroundService is running.");

				using (var scope = _services.CreateScope())
				{

					var _worldRaytingRepository = scope.ServiceProvider.GetRequiredService<IWorldRaytingRepository>();
					var _appUserRepository = scope.ServiceProvider.GetRequiredService<IAppUserRepository>();


					var users = await _appUserRepository.FindAll().ToListAsync();
					foreach (var user in users)
					{
						var raytings = await _worldRaytingRepository.FindAll().ToListAsync();
						foreach (var ray in raytings)
						{
							if (ray.UserId == user.Id)
							{
								ray.Photo = user.ProfilPhoto;
								ray.ThreeStar = user.SuccessfulGames;
								ray.TotalScore = user.TotalScore;
								ray.UserName = user.UserName;
								ray.UserId = user.Id;
								_worldRaytingRepository.Update(ray);
								await _worldRaytingRepository.SaveAsync();
							}
						}
					}


					await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); //delay
				}

				using (var scope = _services.CreateScope())
				{
					var _worldRaytingRepository = scope.ServiceProvider.GetRequiredService<IWorldRaytingRepository>();
					var _appUserRepository = scope.ServiceProvider.GetRequiredService<IAppUserRepository>();

					var rayting = await _worldRaytingRepository.FindAll().OrderByDescending(x => x.TotalScore).AsNoTracking().ToListAsync();
					for (var i = 0; i < rayting.Count; i++)
					{
						rayting[i].Rayting = i + 1;
						_worldRaytingRepository.Update(rayting[i]);
					}
					await _worldRaytingRepository.SaveAsync();

					await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); //delay
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while updating IsActive property.");
			}
		}
	}
}
