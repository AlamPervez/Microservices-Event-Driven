using Dispatch.Infrastructure.Data;
using Dispatch.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Repository
{
    public class DispatchRepository
    {
        private readonly AppDbContext _context;
        public DispatchRepository(AppDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public DispatchOrder GetDispatchOrderByOrderId(int orderId) => _context.DispatchOrder.Include(o => o.Delivery)
                      .SingleOrDefault(o => o.OrderId==orderId);

        public IEnumerable<DispatchOrder> GetDispatchOrders() => _context.DispatchOrder.Include(o => o.Delivery).ToList();

        public DispatchOrder CreateDispatchOrder(DispatchOrder order)
        {
            _context.DispatchOrder.Add(order);
            Save();
            return order;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
