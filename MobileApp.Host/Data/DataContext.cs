namespace MobileApp.Host.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Communication> Communications { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<UserEvent> Events { get; set; }
    public DbSet<EventType> EventTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.EventsCreated)
            .WithOne(e => e.Creator)
            .HasForeignKey(e => e.CreatorId);

        modelBuilder.Entity<UserEvent>()
            .HasOne(e => e.Creator)
            .WithMany(u => u.EventsCreated)
            .HasForeignKey(e => e.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserEvent>()
            .HasMany(e => e.UsersAssigned)
            .WithMany(u => u.EventsAssigned)
            .UsingEntity(j => j.ToTable("EventUser"));

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}