namespace MobileApp.Host.Data;

public class DataContextUsers : IdentityDbContext<User>
{
    public DataContextUsers(DbContextOptions<DataContextUsers> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}