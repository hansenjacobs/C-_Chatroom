using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        NetworkStream stream;
        TcpClient client;
        private Queue<Message> messageQueue;

        public Client(NetworkStream Stream, TcpClient Client, Queue <Message> messageQueue, string userName)
        {
            stream = Stream;
            client = Client;
            UserName = userName;
            this.messageQueue = messageQueue;
        }

        public string UserName { get; private set; }

        public string GetUserInput(string message)
        {
            Send(Encoding.ASCII.GetBytes(message));
            byte[] recievedMessage = new byte[5000];
            stream.Read(recievedMessage, 0, recievedMessage.Length);
            return Encoding.ASCII.GetString(recievedMessage).Trim();

        }

        public void Send(byte[] message)
        {
            //byte[] message = Encoding.ASCII.GetBytes(Message);
            stream.Write(message, 0, message.Count());
        }

        public void SetUser(Dictionary<string, Client> clients)
        {
            bool usernameSet = false;
            do
            {
                string input = GetUserInput("Please enter your username:");
                if (!clients.ContainsKey(input))
                {
                    string oldUserName = UserName;
                    UserName = input;
                    clients.Remove(oldUserName);
                    clients.Add(UserName, this);
                    usernameSet = true;
                }
                else
                {
                    Send(Encoding.ASCII.GetBytes("That username is already in use, please try again."));
                }
            } while (!usernameSet);
        }

        public void Receive()
        {
            while (true)
            {
                byte[] recievedMessage = new byte[5000];
                stream.Read(recievedMessage, 0, recievedMessage.Length);
                messageQueue.Enqueue(new Message(this, recievedMessage));
                Console.WriteLine(Encoding.ASCII.GetString(recievedMessage));
            }
        }
    }
}
