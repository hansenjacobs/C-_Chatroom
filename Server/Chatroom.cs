using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Chatroom
    {
        public Chatroom (string name, IRecipient server)
        {
            Name = name;
            Recipients = new Dictionary<string, IRecipient>();
            Recipients.Add("server", server);
        }

        public int ChatterCount
        {
            get { return Recipients.Count - 1; }
        }
        private Queue<Message> MessageQueue { get; set; }
        public string Name { get; set; }
        public int RecipientCount
        {
            get { return Recipients.Count; }
        }
        public Dictionary<string, IRecipient> Recipients { get; private set; }
        
        public void AddUser(string username, IRecipient user)
        {
            Recipients.Add(username, user);
            Notify(new Message(null, $"<<{username} has joined the chatroom>>"));
        }

        public void RemoveUser(string username)
        {
            Recipients.Remove(username);
            Notify(new Message(null, $"<<{username} has left the chatroom>>"));
        }

        public void Notify(Message message)
        {
            foreach(KeyValuePair<string, IRecipient> recipient in Recipients)
            {
                recipient.Value.DeliverMessage(message);
            }
        }

    }
}
