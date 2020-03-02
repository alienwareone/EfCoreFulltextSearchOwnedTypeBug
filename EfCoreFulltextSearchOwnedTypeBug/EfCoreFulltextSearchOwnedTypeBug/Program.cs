using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EfCoreFulltextSearchOwnedTypeBug
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            dbContextOptionsBuilder.UseSqlServer("Data Source = .\\SQLEXPRESS; Database = EfCoreFulltextSearchOwnedTypeBug; Integrated Security = True;");

            var applicationDbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options);
            applicationDbContext.Database.EnsureCreated();

            if (!applicationDbContext.Products.Any())
            {
                applicationDbContext.Database.ExecuteSqlRaw("CREATE FULLTEXT CATALOG [FT_Catalog] AS DEFAULT;");
                applicationDbContext.Database.ExecuteSqlRaw("CREATE FULLTEXT INDEX ON dbo.Products([Lookup_SearchText]) KEY INDEX PK_Products;");

                applicationDbContext.Products.AddRange(
                    new Product { Lookup = new ProductLookup { SearchText = "Test 1" } },
                    new Product { Lookup = new ProductLookup { SearchText = "Test 2" } },
                    new Product { Lookup = new ProductLookup { SearchText = "www" } });
                applicationDbContext.SaveChanges();
            }

            var result = applicationDbContext.Products.Where(x => EF.Functions.Contains(x.Lookup.SearchText, "'www*'")).ToList();
        }
    }
}
