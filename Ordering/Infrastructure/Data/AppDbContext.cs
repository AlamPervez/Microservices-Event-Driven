using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Ordering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Data
{
    public class AppDbContext:DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<OrderLineItem>().HasKey(o => o.Id);
            modelBuilder.Entity<OrderLineItem>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<OrderLineItem>().Ignore(p => p.TotalPrice);
          
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
            Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, ProductName = "Apple", UnitPrice = 90 },
                new Product { Id = 2, ProductName = "Orange", UnitPrice = 15.30 },
                new Product { Id = 3, ProductName = "Banana", UnitPrice = 3.50 }
                ); 
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
