using System;
using System.Threading.Tasks;

namespace EventBus
{ 
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            this.Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;           
        }

        public IntegrationEvent(Guid id, DateTime creationDate)
        {
            this.Id = id;
            this.CreationDate = creationDate;
        }

        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }
    }
}
