using MobileApp.Host.UserFilters;

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
    public DbSet<UserFilter> UserFilters { get; set; }

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
            .HasMany(e => e.Cooperators)
            .WithMany(u => u.EventsCooperated)
            .UsingEntity(j => j.ToTable("EventUserCooperated"));

        modelBuilder.Entity<UserEvent>()
            .HasMany(e => e.CooperatorsPending)
            .WithMany(u => u.EventsCooperationPending)
            .UsingEntity(j => j.ToTable("EventUserCooperationPending"));

        modelBuilder.Entity<UserEvent>()
            .HasMany(e => e.UsersAssigned)
            .WithMany(u => u.EventsAssigned)
            .UsingEntity(j => j.ToTable("EventUserAssigned"));

        modelBuilder.Entity<UserEvent>()
            .HasMany(e => e.UsersInterested)
            .WithMany(u => u.EventsInterested)
            .UsingEntity(j => j.ToTable("EventUserInterested"));

        modelBuilder.Entity<UserEvent>()
            .HasMany(e => e.UsersSkipped)
            .WithMany(u => u.EventsSkipped)
            .UsingEntity(j => j.ToTable("EventUserSkipped"));

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}