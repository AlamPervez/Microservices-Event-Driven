using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dispatch.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Dispatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispatchController : ControllerBase
    {
        private readonly DispatchRepository _dispatchRepository;

        public DispatchController(DispatchRepository dispatchRepository)
        {
            this._dispatchRepository = dispatchRepository ?? throw new ArgumentNullException(nameof(dispatchRepository));
        }

        // GET api/values
        [HttpGet]
        public ActionResult Get()=>Ok(_dispatchRepository.GetDispatchOrders());

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get(int id) => Ok(_dispatchRepository.GetDispatchOrderByOrderId(id));

    }
}
