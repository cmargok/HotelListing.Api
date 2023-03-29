using HotelListing.API.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Handlers.Security.Data;
using HoteListing.Data.Configurations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotelListing.API.Data
{
    public class HotelListingDBContext : IdentityDbContext<ApiUser>
    {
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }
        public HotelListingDBContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfiguration());
        }


    }

    public class HotelListingDbContextFactory : IDesignTimeDbContextFactory<HotelListingDBContext>
    {
        public HotelListingDBContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HotelListingDBContext>();
            var conn = config.GetConnectionString("HotelConnectionString");
            optionsBuilder.UseSqlServer(conn);

            return new HotelListingDBContext(optionsBuilder.Options);
        }
    }
}
