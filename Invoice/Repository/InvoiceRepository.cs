using Microsoft.EntityFrameworkCore;
using Invoicing.Infrastructure.Data;
using Invoicing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invoicing.Repository
{
    public class InvoiceRepository
    {
        private readonly AppDbContext _context;
        public InvoiceRepository(AppDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Invoice GetInvoiceByOrderId(int orderId) => _context.Invoices.Include(i=>i.ProductsItems)
                .SingleOrDefault(p=> p.OrderId == orderId);

        public IEnumerable<Invoice> GetInvoices() => _context.Invoices.Include(i => i.ProductsItems).ToList();

        public async Task<Invoice> CreateInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
             await SaveAsync();
             return invoice;
        }
        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
