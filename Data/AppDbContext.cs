using Microsoft.EntityFrameworkCore;
using InventoryManagement.Entities;

namespace InventoryManagement.Data;

/// <summary>
/// This is the main class that connects our C# code to the SQL Server database.
/// EF Core uses this class to know which tables exist and how they are related.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Constructor that receives connection string and other options from Program.cs
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet<Category> represents the "Categories" table in the database
    /// </summary>
    public DbSet<Category> Categories { get; set; } = null!;

    /// <summary>
    /// DbSet<Product> represents the "Products" table in the database
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<EmailVerificationOtp> EmailVerificationOtps { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<StockHistory> StockHistories { get; set; } = null!;

    /// <summary>
    /// This method is called when EF Core is building the database model.
    /// We use it to configure relationships between tables and global rules.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Category>().ToTable("Categories");

        // Configure the relationship between Product and Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)                    // One Product belongs to One Category
            .WithMany(c => c.Products)                  // One Category can have many Products
            .HasForeignKey(p => p.CategoryId)           // The column that holds the link (foreign key)
            .OnDelete(DeleteBehavior.NoAction);          // If category is deleted, delete all its products too

        // Global Query Filter: Automatically filter out inactive records
        // This means whenever we query, only IsActive = true records are returned by default
        modelBuilder.Entity<BaseEntity>()
            .HasQueryFilter(e => e.IsActive);

            modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
        .WithMany(s => s.Products)
        .HasForeignKey(p => p.SupplierId);
    }
}