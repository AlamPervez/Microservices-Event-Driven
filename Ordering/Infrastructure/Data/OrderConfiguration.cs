using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Models;

namespace Ordering.Infrastructure.Data
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Ignore(o => o.TotalPrice);
            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderLineItems));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }


}