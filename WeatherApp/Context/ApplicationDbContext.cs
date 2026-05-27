using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp_.net_.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) :base(contextOptions){}


        public DbSet<WeatherModel> weatherLog { get; set; }
    }
}
