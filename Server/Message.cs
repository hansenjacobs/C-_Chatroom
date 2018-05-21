using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Message
    {
        public Client sender;
        public Message(Client Sender, byte[] encodedMessage)
        {
            sender = Sender;
            EncodedMessage = encodedMessage;
            Body = Encoding.ASCII.GetString(encodedMessage);
        }

        public string Body { get; set; }
        public byte[] EncodedMessage { get; set; }
    }
}
