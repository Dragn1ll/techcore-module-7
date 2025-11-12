using Library.Data.PostgreSql;
using Library.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.IntegrationTests;

public class MyTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<BookContext>) ||
                         d.ServiceType == typeof(BookContext))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<BookContext>(options =>
            {
                options.UseInMemoryDatabase("TestDB");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookContext>();
            db.Database.EnsureCreated();
        });
    }
}
