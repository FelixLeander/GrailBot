using GrailBot.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace GrailBot.Database;

/// <summary>
/// Provides interaction with the Database by inheriting from <see cref="DbContext"/>
/// </summary>
public class DatabaseContext : DbContext
{
    /// <summary>
    /// Sets the path of the Database files.
    /// </summary>
    /// <param name="optionsBuilder">Provides the usage of <see cref="SqliteDbContextOptionsBuilderExtensions.UseSqlite"/></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"FileName=.\Database\Database.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WordCount>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
    }

    public DbSet<WordCount> WordCounts => Set<WordCount>();
}