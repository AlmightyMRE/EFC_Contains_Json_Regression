using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

using var ctx = new MyContext();

// Once the setup ran, you can comment this out
//Setup();
Console.WriteLine("Setup done");
DoTheThing();
Console.WriteLine("Did the thing");



void DoTheThing()
{
    var biggestLicenses = ctx.Profiles
        .OrderByDescending(x => x.Customers.Count)
        .Select(x => x.Id)
        .Take(3)
        .ToList();

    var query = ctx.Customers
        .Include(x => x.Profile)
        .OrderByDescending(x => x.LastEdited)
        .Where(x => biggestLicenses.Contains(x.Profile.Id))
        .Select(x => new { x.Id, x.LastEdited, x.Name15 })
        .Skip(0)
        .Take(25);

    Console.WriteLine(query.ToQueryString());

    var people = query.ToList();
}
void Setup()
{
    ctx.Database.EnsureDeleted();
    ctx.Database.EnsureCreated();
    int personsCount = 2_000_000;
    int licenseCount = 350;

    var licenses = Enumerable.Range(0, licenseCount).Select(x => new Profile()).ToList();
    ctx.BulkInsert(licenses, options =>
    {
        options.SetOutputIdentity = true;
    });

    var distribution = GenerateRandomDistribution(personsCount, licenseCount);

    for (int i = 1; i < distribution.Count + 1; i++)
    {
        var license = ctx.Profiles.Find(i)!;

        var customers = Enumerable.Range(0, distribution[i - 1]).Select(x => new Customer()
        {
            Profile = license,
            ProfileId = license.Id
        }).ToList();
        ctx.BulkInsert(customers, options =>
        {
            options.SetOutputIdentity = true;
        });
    }

    // Our distribution doesnt have thaaat much of a good scale, let's, merge the top 20 into 1
    var biggestLicenses = ctx.Profiles.OrderByDescending(x => x.Customers.Count).Take(20).ToList();
    var biggest = biggestLicenses[0];
    biggestLicenses = biggestLicenses[1..];
    var biggestLicenseIds = biggestLicenses.Select(x => x.Id).ToList();

    ctx.Customers
        .Where(x => biggestLicenseIds.Contains(x.ProfileId))
        .ExecuteUpdate(x => x.SetProperty(y => y.Profile, biggest));

    ctx.SaveChanges();
}
static List<int> GenerateRandomDistribution(int totalItems, int numChunks)
{
    List<int> distribution = new List<int>();

    int remainingItems = totalItems;
    int remainingChunks = numChunks;

    for (int i = 0; i < numChunks - 1; i++)
    {
        int maxChunkSize = remainingItems / remainingChunks * 2;
        int chunkSize = Random.Shared.Next(1, maxChunkSize);
        distribution.Add(chunkSize);

        remainingItems -= chunkSize;
        remainingChunks--;
    }

    distribution.Add(remainingItems); // The last chunk gets whatever is left

    // Shuffle the distribution to make it random
    distribution = distribution.OrderBy(x => Random.Shared.Next()).ToList();

    return distribution;
}


public class MyContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasOne(x => x.Profile)
            .WithMany(x => x.Customers)
            .HasForeignKey(x => x.ProfileId);

        modelBuilder.Entity<Customer>()
            .HasIndex(x => x.LastEdited);

        modelBuilder.Entity<Customer>()
            .HasIndex(x => new { x.ProfileId, x.Locked });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer($"Server=WHOLESOME-DESKT;Database={typeof(MyContext).Assembly.GetName().Name};User Id=dev;Password=admin;TrustServerCertificate=True;Trusted_Connection=True");
    }
}
public class Customer
{
    public int Id { get; set; }

    public int ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;

    public DateTime LastEdited { get; set; } = Random.Shared.RandomDateTime();
    public bool Locked { get; set; } = Random.Shared.Next(0, 2) == 1;

    // Add some data with nvarchar(max), including some contents
    public string Name1 { get; set; } = Guid.NewGuid().ToString();
    public string Name2 { get; set; } = Guid.NewGuid().ToString();
    public string Name3 { get; set; } = Guid.NewGuid().ToString();
    public string Name4 { get; set; } = Guid.NewGuid().ToString();
    public string Name5 { get; set; } = Guid.NewGuid().ToString();
    public string Name6 { get; set; } = Guid.NewGuid().ToString();
    public string Name7 { get; set; } = Guid.NewGuid().ToString();
    public string Name8 { get; set; } = Guid.NewGuid().ToString();
    public string Name9 { get; set; } = Guid.NewGuid().ToString();
    public string Name10 { get; set; } = Guid.NewGuid().ToString();
    public string Name11 { get; set; } = Guid.NewGuid().ToString();
    public string Name12 { get; set; } = Guid.NewGuid().ToString();
    public string Name13 { get; set; } = Guid.NewGuid().ToString();
    public string Name14 { get; set; } = Guid.NewGuid().ToString();
    public string Name15 { get; set; } = Guid.NewGuid().ToString();
    public string Name16 { get; set; } = Guid.NewGuid().ToString();
    public string Name17 { get; set; } = Guid.NewGuid().ToString();
    public string Name18 { get; set; } = Guid.NewGuid().ToString();
    public string Name19 { get; set; } = Guid.NewGuid().ToString();
    public string Name20 { get; set; } = Guid.NewGuid().ToString();
    public string Name21 { get; set; } = Guid.NewGuid().ToString();
    public string Name22 { get; set; } = Guid.NewGuid().ToString();
    public string Name23 { get; set; } = Guid.NewGuid().ToString();
    public string Name24 { get; set; } = Guid.NewGuid().ToString();
    public string Name25 { get; set; } = Guid.NewGuid().ToString();
    public string Name26 { get; set; } = Guid.NewGuid().ToString();
    public string Name27 { get; set; } = Guid.NewGuid().ToString();
    public string Name28 { get; set; } = Guid.NewGuid().ToString();
    public string Name29 { get; set; } = Guid.NewGuid().ToString();
    public string Name30 { get; set; } = Guid.NewGuid().ToString();
}
public class Profile
{
    public int Id { get; set; }
    public ICollection<Customer> Customers { get; set; } = null!;
}



public static class Ext
{
    public static DateTime RandomDateTime(this Random rnd)
    {
        DateTime start = new DateTime(1995, 1, 1);
        int range = (DateTime.Today - start).Days;
        return start.AddDays(rnd.Next(range));
    }
}