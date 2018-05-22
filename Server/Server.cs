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
    class Server
    {
        TcpListener server;
        static Queue<Message> messageQueue;
        static Dictionary <string, Client> clients;
        ILogger logger;

        public Server(ILogger logger)
        {//"192.168.0.102" -- Jacob
         //"127.0.0.1", 8888 -- default when same machine
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            server.Start();
            messageQueue = new Queue<Message>();
            clients = new Dictionary<string, Client>();
            this.logger = logger;
        }
        
        public void Run()
        {
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
                    //find better condition later 
                TcpClient clientSocket = default(TcpClient);
                clientSocket = server.AcceptTcpClient();
                NetworkStream stream = clientSocket.GetStream();
                string remoteEndPointString = clientSocket.Client.RemoteEndPoint.ToString();
                clients.Add(remoteEndPointString, new Client(stream, clientSocket, messageQueue, remoteEndPointString));
                Client client = clients[clientSocket.Client.RemoteEndPoint.ToString()];
                logger.DoLog(client.UserName + " has Connected: " + DateTime.Now.ToString());
                client.SetUser(clients);
                messageQueue.Enqueue(new Message(null, $"<<{client.UserName} has joined the chatroom from {remoteEndPointString}>>"));
                Thread clientReceive = new Thread(new ThreadStart(client.Receive));
                clientReceive.Start();
            }
        }

        public static void CloseClient(Client client)
        {
            clients.Remove(client.UserName);
            messageQueue.Enqueue(new Message(null, $"<<{client.UserName} has left the chatroom>>"));
        }

        private void DeliverQueueMessages()
        {
            while (true)
            {
                if (CheckQueue())
                {
                    Respond(messageQueue.Dequeue());
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
