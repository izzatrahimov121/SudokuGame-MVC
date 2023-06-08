




using Core.Entities;
using DataAccess.Contexts;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
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
					WorldRayting raytingDto = new WorldRayting();
					foreach (var user in users)
					{
						var status = 0;
						var raytings = await _worldRaytingRepository.FindAll().ToListAsync();
						foreach (var ray in raytings)
						{
							if (ray.UserId == user.Id)
							{
								status = 1;
								continue;
							}
						}
						if (status == 0)
						{
							raytingDto.Photo = user.ProfilPhoto;
							raytingDto.ThreeStar = user.SuccessfulGames;
							raytingDto.TotalScore = user.TotalScore;
							raytingDto.UserName = user.UserName;
							raytingDto.UserId = user.Id;
							raytingDto.Rayting = 0;
							await _worldRaytingRepository.CreateAsync(raytingDto);
							await _worldRaytingRepository.SaveAsync();
						}
					}

					var rayting = await _worldRaytingRepository.FindAll().OrderByDescending(x => x.TotalScore).ToListAsync();
					for (var i = 0; i < rayting.Count; i++)
					{
						rayting[i].Rayting = i + 1;
						_worldRaytingRepository.Update(rayting[i]);
					}
					await _worldRaytingRepository.SaveAsync();


					await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); //30 second delay
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while updating IsActive property.");
			}
		}
	}
}
