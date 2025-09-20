using AnyOneApi.Host.Addresses;
using AnyOneApi.Host.Categories;
using AnyOneApi.Host.Chats;
using AnyOneApi.Host.Communications;
using AnyOneApi.Host.Events;
using AnyOneApi.Host.EventTypes;
using AnyOneApi.Host.Locations;
using AnyOneApi.Host.Messages;
using AnyOneApi.Host.UserFilters;
using AnyOneApi.Host.Users;

namespace AnyOneApi.Host.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatParticipant> Participants { get; set; }
    public DbSet<Communication> Communications { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<UserEvent> Events { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<Message> Messages { get; set; }
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

        modelBuilder.Entity<UserEvent>()
            .HasMany(e => e.UsersPending)
            .WithMany(u => u.EventsPending)
            .UsingEntity(j => j.ToTable("EventUserPending"));

        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Participants)
            .WithOne(p => p.Chat)
            .HasForeignKey(p => p.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}