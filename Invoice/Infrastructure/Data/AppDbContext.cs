using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoicing.Models;

namespace Invoicing.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Invoice>().HasKey(i=>i.Id);
            modelBuilder.Entity<Invoice>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductItem>().HasKey(p => p.Id);           
            modelBuilder.Entity<ProductItem>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductItem>().Ignore(p => p.TotalPrice);
          
            //rename the tables
            var entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                entityType.Npgsql().TableName = DbCustomTableAndColumnName(entityType.ShortName());
            }
            //name the database columns to lowercase with underscore
            var props = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties());
            foreach (var p in props)
            {
                // exclude properties for owned types as it creates issues with table splitting
                // ref: https://github.com/aspnet/EntityFrameworkCore/issues/12334
                if (!p.DeclaringEntityType.IsOwned())
                {
                    p.Npgsql().ColumnName = DbCustomTableAndColumnName(p.Name);
                }

            }

            //seed the database
     
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
