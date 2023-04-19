using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CookieCustomerUser
    {
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int TypeUser { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }

}
