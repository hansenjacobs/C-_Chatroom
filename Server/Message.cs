using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Message
    {
        public Message(Client Sender, byte[] encodedMessage, string chatroomName)
        {
            this.Sender = Sender;
            EncodedMessage = encodedMessage;
            Body = Encoding.ASCII.GetString(encodedMessage).Trim('\0');
            ReceivedDateTime = DateTime.Now;
            ChatroomName = chatroomName;
        }

        public Message(Client sender, string message, string chatroomName)
        {
            this.Sender = sender;
            EncodedMessage = Encoding.ASCII.GetBytes(message);
            Body = message;
            ReceivedDateTime = DateTime.Now;
            ChatroomName = chatroomName;
        }

        public string Body { get; set; }
        public string ChatroomName { get; set; }
        public byte[] EncodedMessage { get; set; }
        public DateTime ReceivedDateTime { get; private set; }
        public Client Sender {get; set;}
    }
}
