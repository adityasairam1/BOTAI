using BOTAI.Models;
using Microsoft.EntityFrameworkCore;

public partial class BOTAIContext : DbContext
{
    public BOTAIContext(DbContextOptions<BOTAIContext> options)
        : base(options) { }

    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfile", "BOTAI");

            entity.HasKey(e => e.UserID);

            entity.Property(e => e.UserID)
                  .ValueGeneratedOnAdd();  // this matches your SQL sequence logic
        });
    }
}
