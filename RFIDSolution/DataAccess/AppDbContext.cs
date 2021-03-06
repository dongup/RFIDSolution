using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DAL.Shared;
using RFIDSolution.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RFIDSolution.DataAccess.DAL.Entities;

namespace RFIDSolution.Shared.DAL
{
    public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, int, UserClaimEntity, UserRoleEntity,
        UserLoginEntity, RoleClaimEntity, UserTokenEntity>
    //public class AppDbContext : DbContext
    {
        public DbSet<LogEntity> LOG { get; set; }
        public DbSet<ConfigurationEntity> CONFIG { get; set; }
        public DbSet<AntennaEntity> ANTENNAS { get; set; }

        public DbSet<ProductEntity> PRODUCT { get; set; }
        public DbSet<TransferEntity> PRODUCT_TRANSFER { get; set; }
        public DbSet<TransferDetailEntity> PRODUCT_TRANSFER_DTL { get; set; }
        public DbSet<ProductAlertEntity> PRODUCT_ALERT { get; set; }
        public DbSet<InventoryEntity> INVENTORY { get; set; }
        public DbSet<InventoryDetailEntity> INVENTORY_DTL { get; set; }
        public DbSet<RFIDTagEntity> RFID_TAG { get; set; }
        public DbSet<ReaderLogEntity> READER_LOGS { get; set; }

        public DbSet<ModelEntity> MODEL_DEF { get; set; }
        public DbSet<CategoryEntity> CAT_DEF { get; set; }
        public DbSet<DepartmentEntity> DEPT_DEF { get; set; }

        public static string ConnStr { get; set; }

        public AppDbContext()
        {

        }

        public AppDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext(string connStr)
        {
            ConnStr = connStr;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedData(builder);

            builder.Entity<UserRoleEntity>(x =>
            {
                x.HasOne(a => a.User).WithMany(a => a.UserRoles).HasForeignKey(a => a.UserId);
                x.HasOne(a => a.Role).WithMany(a => a.UserRoles).HasForeignKey(a => a.RoleId);
            });

            ConfigCollumnComment(builder);
            ConfigSoftDelete(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnStr);
            base.OnConfiguring(optionsBuilder);
        }

        public void ConfigCollumnComment(ModelBuilder builder)
        {
            builder.Entity<ProductEntity>()
                .Property(x => x.LR)
                .HasComment("Bên của giầy: 1 - Left; 2 - Right");

            builder.Entity<ProductEntity>()
                .Property(x => x.PRODUCT_STATUS)
                .HasComment("Trạng thái của giầy: 1 - Available; 2 - NotAvailable; 3 - OnHold");

            builder.Entity<TransferEntity>()
               .Property(x => x.TRANSFER_STATUS)
               .HasComment("Trạng thái của inout: 1 - chưa trả; 2 - đã trả");

            builder.Entity<TransferDetailEntity>()
               .Property(x => x.TRANSFER_STATUS)
               .HasComment("Trạng thái giầy lấy: 1 - Ok; 2 - không ok");

            builder.Entity<TransferDetailEntity>()
               .Property(x => x.RETURN_STATUS)
               .HasComment("Trạng thái giầy trả: 1 - Ok; 2 - không ok");

            builder.Entity<ProductAlertEntity>()
               .Property(x => x.ALERT_CONF_STATUS)
               .HasComment("Trạng thái xử lý: 1 - chưa xử lý; 2 - đã xử lý");

            builder.Entity<InventoryEntity>()
               .Property(x => x.INVENTORY_STATUS)
               .HasComment("Trạng thái xử lý: 1 - chờ xử lý; 2 - đã hoàn thành");

            builder.Entity<InventoryDetailEntity>()
              .Property(x => x.STATUS)
              .HasComment("Trạng thái xử lý: 1 - Tìm thấy; 2 - Không tìm thấy");

            builder.Entity<RFIDTagEntity>()
             .Property(x => x.TAG_STATUS)
             .HasComment("Trạng thái xử lý: 1 - Sẵn sàng; 2 - Bị hủy");
        }

        public void ConfigSoftDelete(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                //// 1. Add the IsDeleted property
                //entityType.AddProperty("IsDeleted", typeof(bool)).SetDefaultValue(false);

                // 2. Create the query filter
                var parameter = Expression.Parameter(entityType.ClrType);

                // EF.Property<bool>(post, "IsDeleted")
                var propertyMethodInfo = typeof(EF).GetMethod("Property").MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IS_DELETED"));

                // EF.Property<bool>(post, "IsDeleted") == false
                BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));

                // post => EF.Property<bool>(post, "IsDeleted") == false
                var lambda = Expression.Lambda(compareExpression, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        public override int SaveChanges()
        {
            //UpdateSoftDeleteStatuses();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            //UpdateSoftDeleteStatuses();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            var entries = ChangeTracker.Entries();

            var countEntry = entries.Count();

            for (int i = 0; i < countEntry; i++)
            {
                var currentEntry = entries.FirstOrDefault();
                if (currentEntry.Entity.GetType() == typeof(UserEntity)) break;
                if (currentEntry == null) break;
                switch (currentEntry.State)
                {
                    case EntityState.Added:
                        currentEntry.CurrentValues["IsDeleted"] = false;
                        break;
                    case EntityState.Deleted:
                        currentEntry.State = EntityState.Modified;
                        currentEntry.CurrentValues["IsDeleted"] = true;
                        break;
                }
            }
        }

        public void SeedData(ModelBuilder builder)
        {
            builder.Entity<RoleEntity>().HasData(
                new RoleEntity
                {
                    Id = 1,
                    Name = AppRoles.Admin,
                    NormalizedName = AppRoles.Admin.ToUpper()
                },
                new RoleEntity
                {
                    Id = 2,
                    Name = AppRoles.User,
                    NormalizedName = AppRoles.User.ToUpper()
                }
            );
        }

        public void DetachAllEntities()
        {
            //Console.WriteLine("=============Detach all entity....===============");
            var changedEntriesCopy = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

    }
}
