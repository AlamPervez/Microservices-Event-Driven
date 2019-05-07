using System;
using System.Collections.Generic;
using System.Text;

namespace Producer
{
   public class UserCreatedIntegrationEvent
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public int Age { get; set; }

    }
}
