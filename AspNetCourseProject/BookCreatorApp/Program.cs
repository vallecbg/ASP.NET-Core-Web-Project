namespace BookCreatorApp
{
	using System;
	using System.Linq;
	using BookCreator.Data;
	using BookCreator.Models;
	using Microsoft.AspNetCore;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;

	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateWebHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope())
			{
				var serverProvider = scope.ServiceProvider;
			}
			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();

		
	}
}