using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductMgt.Infrastructure;

namespace ProductMgt.DatabaseCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<ProductMgtDbContext>(builder =>
                builder.UseSqlServer("Server=10.0.19.103;Database=ProductMgt;User Id=sa;Password=Test!234",
                    optionsBuilder => { optionsBuilder.MigrationsAssembly("ProductMgt.Migrations"); }
                ));
            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            
            using (var scope = buildServiceProvider.CreateScope())
            {
                try
                {
                    var productMgtDbContext = buildServiceProvider.GetService<ProductMgtDbContext>();
                    productMgtDbContext.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
        }
    }
}