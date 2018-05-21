using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Message
    {
        public Message(Client Sender, byte[] encodedMessage)
        {
            this.Sender = Sender;
            EncodedMessage = encodedMessage;
            Body = Encoding.ASCII.GetString(encodedMessage).Trim();
            ReceivedDateTime = DateTime.Now;
        }

        public string Body { get; set; }
        public byte[] EncodedMessage { get; set; }
        public DateTime ReceivedDateTime { get; private set; }
        public Client Sender {get; set;}
    }
}
