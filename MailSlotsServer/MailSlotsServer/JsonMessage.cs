using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSlotsServer
{
    public class JsonMessage
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
    }
}
