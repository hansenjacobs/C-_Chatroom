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
        public string UserId;
        public Queue<Message> messageQueue;
        public Client(NetworkStream Stream, TcpClient Client, Queue <Message> messageQueue)
        {
            stream = Stream;
            client = Client;
            UserId = "495933b6-1762-47a1-b655-483510072e73";
            this.messageQueue = messageQueue;
        }

        public string GetUserInput(string message)
        {
            Send(Encoding.ASCII.GetBytes(message));
            byte[] recievedMessage = new byte[256];
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
                    UserId = input;
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
