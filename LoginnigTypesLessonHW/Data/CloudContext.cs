using Microsoft.EntityFrameworkCore;


namespace LoginnigTypesLessonHW.Data
{
    public class CloudContext : DbContext
    {
        public CloudContext()
        {
            Database.Migrate();
        }

        public DbSet<CloudFile> CloudFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = DESKTOP-6543DI3; DataBase = LoadingTypesLessonHW; Trusted_connection = true;");
        }
    }
}
