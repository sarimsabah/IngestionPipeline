using Microsoft.EntityFrameworkCore;
using MyApiApp.Entities;
using MyApiApp.Entities.MasterData;
using MyApiApp.Entities.TransactionData;

namespace MyApiApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Legacy Tables (keeping for backward compatibility)
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<ItemEntity> Items { get; set; }
    public DbSet<UomEntity> ItemUoms { get; set; }

    // Staging Tables
    public DbSet<CustomerIngestionLogEntity> CustomerIngestionLogs { get; set; }
    public DbSet<ItemIngestionLogEntity> ItemIngestionLogs { get; set; }

    // Transaction Tables (Logging)
    public DbSet<CustomerTransactionEntity> CustomerTransactions { get; set; }
    public DbSet<ItemTransactionEntity> ItemTransactions { get; set; }

    // Master Data Tables
    public DbSet<MRegionEntity> MasterRegions { get; set; }
    public DbSet<MCityEntity> MasterCities { get; set; }
    public DbSet<MPaymentTermEntity> MasterPaymentTerms { get; set; }
    public DbSet<MChannelEntity> MasterChannels { get; set; }
    public DbSet<MBrandEntity> MasterBrands { get; set; }
    public DbSet<MCategoryEntity> MasterCategories { get; set; }
    public DbSet<MUomEntity> MasterUoms { get; set; }

    // Transaction Data Tables
    public DbSet<TCustomerEntity> TransactionCustomers { get; set; }
    public DbSet<TItemEntity> TransactionItems { get; set; }
    public DbSet<TItemUomConversionEntity> TransactionItemUomConversions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<CustomerEntity>(entity =>
        {
            entity.HasIndex(e => e.CustomerCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Item configuration
        modelBuilder.Entity<ItemEntity>(entity =>
        {
            entity.HasIndex(e => e.ItemCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasMany(e => e.UomList)
                .WithOne(e => e.Item)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UOM configuration
        modelBuilder.Entity<UomEntity>(entity =>
        {
            entity.HasIndex(e => new { e.ItemId, e.Uom }).IsUnique();
        });

        // Customer Ingestion Log configuration
        modelBuilder.Entity<CustomerIngestionLogEntity>(entity =>
        {
            entity.HasIndex(e => e.RequestTime);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ProcessStatus);
            entity.HasIndex(e => e.ReferenceId);
            entity.Property(e => e.RequestTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Item Ingestion Log configuration
        modelBuilder.Entity<ItemIngestionLogEntity>(entity =>
        {
            entity.HasIndex(e => e.RequestTime);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ProcessStatus);
            entity.HasIndex(e => e.ReferenceId);
            entity.Property(e => e.RequestTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Customer Transaction configuration
        modelBuilder.Entity<CustomerTransactionEntity>(entity =>
        {
            entity.Property(e => e.TransactionTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Item Transaction configuration
        modelBuilder.Entity<ItemTransactionEntity>(entity =>
        {
            entity.Property(e => e.TransactionTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Master Data Configurations
        modelBuilder.Entity<MRegionEntity>(entity =>
        {
            entity.HasIndex(e => e.RegionCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MCityEntity>(entity =>
        {
            entity.HasIndex(e => e.CityCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MPaymentTermEntity>(entity =>
        {
            entity.HasIndex(e => e.PaymentTermCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MChannelEntity>(entity =>
        {
            entity.HasIndex(e => e.ChannelCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MBrandEntity>(entity =>
        {
            entity.HasIndex(e => e.BrandCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MCategoryEntity>(entity =>
        {
            entity.HasIndex(e => e.CategoryCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MUomEntity>(entity =>
        {
            entity.HasIndex(e => e.UomCode).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Transaction Data Configurations
        modelBuilder.Entity<TCustomerEntity>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TItemEntity>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasMany(e => e.UomConversions)
                .WithOne(e => e.Item)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TItemUomConversionEntity>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
