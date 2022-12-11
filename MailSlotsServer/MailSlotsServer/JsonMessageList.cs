using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSlotsServer
{
    public class JsonMessageList
    {
        public List<JsonMessage> MessageList { get; set; }

        public JsonMessageList()
        {
            MessageList = new List<JsonMessage>();
        }
    }
}
