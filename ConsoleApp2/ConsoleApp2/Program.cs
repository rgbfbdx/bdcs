using System.Reflection.Emit;
using static System.Collections.Specialized.BitVector32;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }

    public ICollection<Session> Sessions { get; set; }
}

public class Member
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public DateTime JoinDate { get; set; }

    public ICollection<Session> Sessions { get; set; }
}

public class Session
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; }

    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
}

using Microsoft.EntityFrameworkCore;

public class BoardGamesContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Session> Sessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=BoardGamesClub;Trusted_Connection=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(g => g.MinPlayers)
                .IsRequired();

            entity.Property(g => g.MaxPlayers)
                .IsRequired();

            entity.HasCheckConstraint("CK_Game_MinPlayers", "[MinPlayers] > 0");
            entity.HasCheckConstraint("CK_Game_MaxPlayers", "[MaxPlayers] > 0");

            entity.HasMany(g => g.Sessions)
                .WithOne(s => s.Game)
                .HasForeignKey(s => s.GameId);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(m => m.FullName)
                .IsRequired();

            entity.Property(m => m.JoinDate)
                .HasColumnType("datetime");

            entity.HasMany(m => m.Sessions)
                .WithOne(s => s.Member)
                .HasForeignKey(s => s.MemberId);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(s => s.Date)
                .HasColumnType("datetime");
        });
    }

}


using (var context = new BoardGamesContext())
{
    context.Database.EnsureCreated();

    if (!context.Games.Any())
    {
        var games = new List<Game>
        {
            new Game { Title="Catan", Genre="Strategy", MinPlayers=3, MaxPlayers=4 },
            new Game { Title="Carcassonne", Genre="Tile", MinPlayers=2, MaxPlayers=5 },
            new Game { Title="Ticket to Ride", Genre="Family", MinPlayers=2, MaxPlayers=5 },
            new Game { Title="Gloomhaven", Genre="Adventure", MinPlayers=1, MaxPlayers=4 },
            new Game { Title="Pandemic", Genre="Co-op", MinPlayers=2, MaxPlayers=4 }
        };

        var members = new List<Member>
        {
            new Member { FullName="Ivan Petrenko", JoinDate=DateTime.Now.AddMonths(-10) },
            new Member { FullName="Olena Kovalenko", JoinDate=DateTime.Now.AddMonths(-8) },
            new Member { FullName="Andrii Shevchenko", JoinDate=DateTime.Now.AddMonths(-6) },
            new Member { FullName="Marta Bondar", JoinDate=DateTime.Now.AddMonths(-5) },
            new Member { FullName="Dmytro Koval", JoinDate=DateTime.Now.AddMonths(-3) }
        };

        context.Games.AddRange(games);
        context.Members.AddRange(members);
        context.SaveChanges();

        var rnd = new Random();
        var sessions = new List<Session>();

        for (int i = 0; i < 20; i++)
        {
            sessions.Add(new Session
            {
                GameId = games[rnd.Next(games.Count)].Id,
                MemberId = members[rnd.Next(members.Count)].Id,
                Date = DateTime.Now.AddDays(-rnd.Next(100)),
                DurationMinutes = rnd.Next(30, 180)
            });
        }

        context.Sessions.AddRange(sessions);
        context.SaveChanges();
    }

    Console.WriteLine("Database seeded!");
}


