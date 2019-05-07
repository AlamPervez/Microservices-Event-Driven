using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoicing.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Invoicing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceRepository _invoiceRepository;
            
        public InvoiceController(InvoiceRepository invoiceRepository)
        {
            this._invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        }
        // GET api/values
        [HttpGet]
        public ActionResult Get()=>Ok(_invoiceRepository.GetInvoices());

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get(int id) => Ok(_invoiceRepository.GetInvoiceByOrderId(id));

     
    }
}
