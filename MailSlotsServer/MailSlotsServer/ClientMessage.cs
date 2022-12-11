using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSlotsServer
{
    public class ClientMessage
    {
        [Key]
        public int Id { get; set; }
        public Guid UniqueField { get; set; }
        public string MessageContent { get; set; }
        public string Time { get; set; }

        public string UserName { get; set; }
    }
}
