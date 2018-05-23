using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Client : IRecipient
    {
        NetworkStream stream;
        TcpClient client;

        public Client(NetworkStream Stream, TcpClient Client, Dictionary<string, Chatroom> chatrooms, string userName)
        {
            stream = Stream;
            client = Client;
            UserName = userName;
            Chatrooms = chatrooms;
        }

        private string ListChatrooms
        {
            get
            {
                string output = "";
                foreach (KeyValuePair<string, Chatroom> chatroom in Chatrooms)
                {
                    output += chatroom.Key.ToString() + "\n";
                }
                return output.Trim();
            }
        }

        public Chatroom CurrentChatroom { get; set; }

        public Dictionary<string, Chatroom> Chatrooms { get; set; }

        public string UserName { get; private set; }

        public void ChatroomMenu()
        {
            Send("What chatroom would you like to join?");
            string input = "";
            do
            {
                input = GetUserInput(ListChatrooms + "\n");
            } while (!Chatrooms.ContainsKey(input));

            ChangeChatroom(Chatrooms[input]);
        }

        public void ChatroomMenu(string input)
        {
            if (!Chatrooms.ContainsKey(input))
            {
                Send("What chatroom would you like to join?");

                do
                {
                    input = GetUserInput(ListChatrooms);
                } while (!Chatrooms.ContainsKey(input));
            }

            ChangeChatroom(Chatrooms[input]);

        }

        public void ChangeChatroom(Chatroom chatroom)
        {
            if(CurrentChatroom != null)
            {
                CurrentChatroom.RemoveUser(UserName);
            }
            CurrentChatroom = chatroom;
            Send($"Welcome to the {CurrentChatroom.Name} chatroom.  To change rooms enter '>>' or enter '>>' + the name of the room, for example, '>>Main' to move to the Main chatroom.\n");
            CurrentChatroom.AddUser(UserName, this);
        }

        public void DeliverMessage(Message message)
        {
            byte[] encodedMessage;
            if (message.Sender != null)
            {
                encodedMessage = Encoding.ASCII.GetBytes($"{message.ReceivedDateTime.ToString("G")} {message.Sender.UserName} >> {message.Body}");
            }
            else
            {
                encodedMessage = Encoding.ASCII.GetBytes($"{message.ReceivedDateTime.ToString("G")} {message.Body}");
            }

            stream.Write(encodedMessage, 0, encodedMessage.Count());
        }

        public string GetUserInput(string message)
        {
            Send(message);
            byte[] recievedMessage = new byte[5000];
            stream.Read(recievedMessage, 0, recievedMessage.Length);
            return Encoding.ASCII.GetString(recievedMessage).Trim('\0');

        }

        public string Receive()
        {
            try
            {
                while (true)
                {
                    byte[] recievedMessage = new byte[5000];
                    stream.Read(recievedMessage, 0, recievedMessage.Length);
                    string messageString = Encoding.ASCII.GetString(recievedMessage);
                    if(messageString.Substring(0, 2) != ">>")
                    {
                        CurrentChatroom.EnqueueMessage(new Message(this, recievedMessage, CurrentChatroom.Name));
                    }
                    else
                    {
                        return messageString.Trim().Trim('\0');
                    }
                }
            }
            catch (SystemException)
            {
                Server.CloseClient(this);
                return "";
            }
        }

        public void Send(string messageString)
        {
            byte[] message = Encoding.ASCII.GetBytes(messageString);
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
                    Send("That username is already in use, please try again.");
                }
            } while (!usernameSet);
        }

        public void Start()
        {
            Send(UserName + " welcome to the chat server.\n");
            ChatroomMenu();
            string input = "";
            do
            {
                input = Receive();
                if(input.Length >= 2 && input.Substring(0, 2) == ">>")
                {
                    ChatroomMenu(input.Substring(2));
                }
            } while (input != "");
            CurrentChatroom.RemoveUser(UserName);
        }
    }
}
