using Dispatch.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Dispatch.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DispatchOrder> DispatchOrder { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DispatchOrder>().HasKey(d => d.Id);
            modelBuilder.Entity<DispatchOrder>().Property(d => d.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Delivery>().HasKey(d => d.DispatchOrderId);

            //rename the tables
            IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType> entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in entityTypes)
            {
                entityType.Npgsql().TableName = this.DbCustomTableAndColumnName(entityType.ShortName());
            }
            //name the database columns to lowercase with underscore
            IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IMutableProperty> props = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties());
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableProperty p in props)
            {
                // exclude properties for owned types as it creates issues with table splittingB
                // ref: https://github.com/aspnet/EntityFrameworkCore/issues/12334
                if (!p.DeclaringEntityType.IsOwned())
                {
                    p.Npgsql().ColumnName = this.DbCustomTableAndColumnName(p.Name);
                }
            }
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Utility function to name a pascal case to lowercase with underscore
        /// </summary>
        /// <param name="name">pascal case string</param>
        /// <returns></returns>
        private string DbCustomTableAndColumnName(string name)
        {
            var result = System.Text.RegularExpressions.Regex.Replace(name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);
            return result.ToLower();
        }
    }
}
