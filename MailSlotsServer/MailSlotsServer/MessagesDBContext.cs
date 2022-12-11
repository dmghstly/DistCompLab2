using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace MailSlotsServer
{
    public class MessagesDBContext : DbContext
    {
        public MessagesDBContext() : base("MailslotsConnecction")
        {
            Database.Initialize(force: false);
        }

        public DbSet<ClientMessage> ClientMessages { get; set; }
        public DbSet<Client> Users { get; set; }
    }
}
