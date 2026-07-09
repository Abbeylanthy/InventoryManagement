using Microsoft.EntityFrameworkCore;
using InventoryManagement.Entities;

namespace InventoryManagement.Data;

/// This is the main class that connects our C# code to the SQL Server database.
/// EF Core uses this class to know which tables exist and how they are related.
public class AppDbContext : DbContext
{
    /// <summary>
    /// Constructor that receives connection string and other options from Program.cs
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    /// DbSet<Category> represents the "Categories" table in the database
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<EmailVerificationOtp> EmailVerificationOtps { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Role> Roles { get; set; }
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set;} = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
    public DbSet<WalletWithdrawal> WalletWithdrawals { get; set; } = null!;
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<StockHistory> StockHistories { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<Feedback> Feedbacks { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;

    /// <summary>
    /// This method is called when EF Core is building the database model.
    /// We use it to configure relationships between tables and global rules.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


         modelBuilder.Entity<UserRole>()
        .HasKey(ur => new { ur.UserId, ur.RoleId });

    modelBuilder.Entity<UserRole>()
        .HasOne(ur => ur.User)
        .WithMany(u => u.UserRoles)
        .HasForeignKey(ur => ur.UserId);

    modelBuilder.Entity<UserRole>()
        .HasOne(ur => ur.Role)
        .WithMany(r => r.UserRoles)
        .HasForeignKey(ur => ur.RoleId);


        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Category>().ToTable("Categories");

        // Configure the relationship between Product and Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)                    // One Product belongs to One Category
            .WithMany(c => c.Products)                  // One Category can have many Products
            .HasForeignKey(p => p.CategoryId)           // The column that holds the link (foreign key)
            .OnDelete(DeleteBehavior.NoAction);          // If category is deleted, delete all its products too

    

            modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
        .WithMany(s => s.Products)
        .HasForeignKey(p => p.SupplierId);

        modelBuilder.Entity<RolePermission>()
    .HasKey(rp => new { rp.RoleId, rp.PermissionId }); 

modelBuilder.Entity<RolePermission>()
    .HasOne(rp => rp.Role)
    .WithMany(r => r.RolePermissions)
    .HasForeignKey(rp => rp.RoleId); 

modelBuilder.Entity<RolePermission>()
    .HasOne(rp => rp.Permission)
    .WithMany(p => p.RolePermissions)
    .HasForeignKey(rp => rp.PermissionId); 

    modelBuilder.Entity<Cart>()
    .HasOne(c => c.Customer)
    .WithMany()
    .HasForeignKey(c => c.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<CartItem>()
    .HasOne(ci => ci.Cart)
    .WithMany(c => c.Items)
    .HasForeignKey(ci => ci.CartId);

modelBuilder.Entity<CartItem>()
    .HasOne(ci => ci.Product)
    .WithMany()
    .HasForeignKey(ci => ci.ProductId)
    .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Cart>()
    .HasIndex(c => c.CustomerId)
    .IsUnique();

    modelBuilder.Entity<Order>()
    .HasOne(o => o.Customer)
    .WithMany()
    .HasForeignKey(o => o.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<OrderItem>()
    .HasOne(oi => oi.Order)
    .WithMany(o => o.Items)
    .HasForeignKey(oi => oi.OrderId);

modelBuilder.Entity<OrderItem>()
    .HasOne(oi => oi.Product)
    .WithMany()
    .HasForeignKey(oi => oi.ProductId)
    .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Wallet>()
    .HasOne(w => w.Customer)
    .WithMany()
    .HasForeignKey(w => w.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<WalletTransaction>()
    .HasOne(wt => wt.Wallet)
    .WithMany(w => w.Transactions)
    .HasForeignKey(wt => wt.WalletId);

    modelBuilder.Entity<Wallet>()
    .HasIndex(w => w.CustomerId)
    .IsUnique();

    modelBuilder.Entity<Payment>()
    .HasIndex(p => p.Reference)
    .IsUnique();

    modelBuilder.Entity<Notification>()
    .HasOne(n => n.User)
    .WithMany()
    .HasForeignKey(n => n.UserId)
    .OnDelete(DeleteBehavior.Restrict);
    } 
}