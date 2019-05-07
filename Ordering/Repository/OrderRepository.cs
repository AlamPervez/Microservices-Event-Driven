using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Data;
using Ordering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Repository
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Order GetOrder(int orderId) => _context.Orders.Include(o=>o.OrderLineItems)
            .ThenInclude(l=>l.Product)
            .SingleOrDefault(o => o.Id == orderId);

        public IEnumerable<Order> GetOrders() => _context.Orders.Include(o=>o.OrderLineItems)
            .ThenInclude(l=>l.Product)
            .ToList();

        public Order CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            Save();
            return order;
        }
        public void Save()
        {
          _context.SaveChanges();
        }
    }
}
