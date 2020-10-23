using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Woodchuck.Models
{
    public partial class WoodchuckContext : DbContext
    {
        public WoodchuckContext()
        {
        }

        public WoodchuckContext(DbContextOptions<WoodchuckContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<LogCategory> LogCategory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.AccountName).HasMaxLength(50);

                entity.Property(e => e.MessageLevel).HasMaxLength(50);

                entity.Property(e => e.Xid).HasColumnName("xid");
            });

            modelBuilder.Entity<LogCategory>(entity =>
            {
                entity.HasKey(e => new { e.LogId, e.CategoryId });

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.LogCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LogCategory_Category");

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.LogCategory)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LogCategory_Log");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
