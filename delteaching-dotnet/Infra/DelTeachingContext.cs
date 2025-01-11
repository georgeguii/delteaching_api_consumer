using delteaching_dotnet.Domain;
using Microsoft.EntityFrameworkCore;

namespace delteaching_dotnet.Infra
{
    public class DelTeachingContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DelTeachingContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration["ConnectionStrings:SqlServer"];
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>(p =>
            {
                p.ToTable("BankAccounts");

                p.HasKey(p => p.Id);

                p.Property(p => p.Branch).HasMaxLength(40).IsRequired();
                p.Property(p => p.Number).IsRequired();
                p.Property(p => p.Type).HasColumnType("VARCHAR(50)").IsRequired();
                p.Property(p => p.HolderName).HasColumnType("VARCHAR(150)").IsRequired();
                p.Property(p => p.HolderEmail).HasColumnType("VARCHAR(300)").IsRequired();
                p.Property(p => p.HolderDocument).HasColumnType("VARCHAR(14)").IsRequired();
                p.Property(p => p.HolderType).IsRequired();
                p.Property(p => p.CreatedAt).HasDefaultValueSql("GEUTCDATE()").IsRequired();
                p.Property(p => p.UpdatedAt).IsRequired(false);
            });

        }
    }
}
