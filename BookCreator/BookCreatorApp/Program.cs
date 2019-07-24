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

                SeedBookGenresIfDbEmpty(serverProvider).GetAwaiter().GetResult();
                //SeedBooksIfDbEmpty(serverProvider).GetAwaiter().GetResult();
            }
			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();

        private static async Task SeedBookGenresIfDbEmpty(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<BookCreatorContext>();

            //Make sure the database is created
            dbContext.Database.EnsureCreated();

            var genres = new[]
            {
                new BookGenre()
                {
                    Genre = "Fantasy"
                },
                new BookGenre()
                {
                    Genre = "Thriller"
                },
                new BookGenre()
                {
                    Genre = "Horror"
                },
                new BookGenre()
                {
                    Genre = "Other"
                }
            };

            var noGenres = dbContext.BooksGenres.Any();
            if (!noGenres)
            {
                await dbContext.BooksGenres.AddRangeAsync(genres);
                await dbContext.SaveChangesAsync();
            }
        }

        //private static async Task SeedBooksIfDbEmpty(IServiceProvider serviceProvider)
        //{
        //    var dbContext = serviceProvider.GetRequiredService<BookCreatorContext>();

        //    //Make sure the database is created
        //    dbContext.Database.EnsureCreated();

        //    var books = new[]
        //    {
        //        new Book()
        //        {
        //            Id = "1",
        //            BookGenreId = 
        //        }
        //    };

        //    var noGenres = dbContext.BooksGenres.Any();
        //    if (!noGenres)
        //    {
        //        await dbContext.BooksGenres.AddRangeAsync(genres);
        //        await dbContext.SaveChangesAsync();
        //    }
        //}
    }
}