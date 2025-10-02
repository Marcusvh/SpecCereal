using CerealLib;
using Microsoft.EntityFrameworkCore;

namespace Cereal.Context
{
    public class CerealContext: DbContext
    {
        public CerealContext(DbContextOptions<CerealContext> options) : base(options) { }
        public DbSet<Nutrition> Nutritions { get; set; }   
        public DbSet<User> Users { get; set; }
    }
}
