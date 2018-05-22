using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server : IRecipient
    {
        TcpListener server;
        static Queue<Message> messageQueue;
        static Dictionary <string, Client> clients;
        public Dictionary<string, Chatroom> Chatrooms { get; set; }
        ILogger logger;

        public Server(ILogger logger)
        {//"192.168.0.102" -- Jacob
         //"127.0.0.1", 8888 -- default when same machine
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            server.Start();
            messageQueue = new Queue<Message>();
            clients = new Dictionary<string, Client>();
            this.logger = logger;
            Chatrooms = new Dictionary<string, Chatroom>();
        }
        
        public void Run()
        {
            CreateChatroom("Main");
            CreateChatroom("CoolPeople");
            CreateChatroom("AveragePeople");

            Parallel.Invoke(
            ()=>
            {
                AcceptClient();
            },
            ()=>
            {
                DeliverQueueMessages();
            }
            );
            
        }
        
        private void AcceptClient()
        {
            while (true)
            { 
                TcpClient clientSocket = default(TcpClient);
                clientSocket = server.AcceptTcpClient();
                NetworkStream stream = clientSocket.GetStream();
                string remoteEndPointString = clientSocket.Client.RemoteEndPoint.ToString();
                Client client = new Client(stream, clientSocket, Chatrooms, "New User");
                client.SetUser(clients);
                new Thread(new ThreadStart(client.Start)).Start();
            }
        }

        public static void CloseClient(Client client)
        {
            clients.Remove(client.UserName);
            lock(messageQueue)
                messageQueue.Enqueue(new Message(null, $"<<{client.UserName} has left the chatroom>>"));
        }

        public void CreateChatroom(string name)
        {
            Chatroom chatroom = new Chatroom(name, this);
            Chatrooms.Add(name, chatroom);
            new Thread(new ThreadStart(chatroom.DeliverMessages)).Start();
        }

        public void DeliverMessage(Message message)
        {
            if(message.Sender != null)
            {
                logger.DoLog($"{message.ReceivedDateTime.ToString("G")} {message.Sender.UserName} >> {message.Body}");
                Console.WriteLine($"{message.ReceivedDateTime.ToString("G")} {message.Sender.UserName} >> {message.Body}");
            }
            else
            {
                logger.DoLog($"{message.ReceivedDateTime.ToString("G")} {message.Body}");
                Console.WriteLine($"{message.ReceivedDateTime.ToString("G")} {message.Body}");
            }
            
        }

        private void DeliverQueueMessages()
        {
            while (true)
            {
                if (CheckQueue())
                {
                    Message message;
                    lock (messageQueue)
                    {
                        message = messageQueue.Dequeue();
                    }
                        
                    Respond(message);
                }
            }
        }
        private void Respond(Message message)
        {
            if(message.Sender != null)
            {
                foreach(KeyValuePair<string, Client> pair in clients)
                {
                    pair.Value.Send($"{message.ReceivedDateTime.ToString("G")} {message.Sender.UserName} >> {message.Body}");
                }
                logger.DoLog(message.Body);
                Console.WriteLine($"{message.ReceivedDateTime.ToString("G")} {message.Sender.UserName} >> {message.Body}");
            }
            else
            {
                foreach(KeyValuePair<string, Client> pair in clients)
                {
                    pair.Value.Send($"{message.ReceivedDateTime.ToString("G")} {message.Body}");
                }
                logger.DoLog(message.Body);
                Console.WriteLine($"{message.ReceivedDateTime.ToString("G")} {message.Body}");
            }
        }

        private bool CheckQueue()
        {
            if (messageQueue.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }  
        }
    }
}
