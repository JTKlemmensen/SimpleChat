using Shared.Ciphers;
using Shared.Network;
using Shared.Network.Constants;
using Shared.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Network
{
    public class SimpleClient : Connection
    {
        private IdleChecker idle;
        private AsymmetricCipher asymCipher;

        public SimpleClient(string ip, int port, string username = "") : base()
        {
            try
            {
                this.AddCommand(MessageProtocols.Connect, ConnectCommand);
                this.AddCommand(MessageProtocols.Disconnect, DisconnectCommand);
                this.AddCommand(MessageProtocols.Message, MessageCommand);
                this.AddCommand(MessageProtocols.Users, UsersCommand);
                this.AddCommand(MessageProtocols.Ping, PingCommand);
                this.AddCommand(MessageProtocols.Pong, PongCommand);

                //this.AddCommand(MessageProtocols.SetUsername, SetUsernameCommand);
                //this.AddCommand(MessageProtocols.UsernameTaken, UsernameTakenCommand);
                //this.AddCommand(MessageProtocols.UsernameChanged, UsernameChangedCommand);

                Socket connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                connection.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                this.Start(connection);
                SetupConnection();

                idle = new IdleChecker(this);
            }
            catch (Exception){}
        }

        #region incoming commands from server
        private void ConnectCommand(NetworkMessage message)
        {
            if (message.TryGetObject<string>(out string connectedUser))
                UserConnected?.Invoke(connectedUser);
        }

        private void DisconnectCommand(NetworkMessage message)
        {
            if (message.TryGetObject<string>(out string disconnectedUser))
                UserDisconnected?.Invoke(disconnectedUser);
        }

        private void MessageCommand(NetworkMessage message)
        {
            if (message.TryGetObject<Message>(out Message m))
                NewMessage?.Invoke(m.Content, m.Sender);
        }

        private void UsersCommand(NetworkMessage message)
        {
            if (message.TryGetObject<IEnumerable<string>>(out IEnumerable<string> users))
            {
                TotalUsers?.Invoke(users);
                ConnectionConnect?.Invoke();
            }
        }

        private void PingCommand(NetworkMessage message)
        {
            Send(MessageProtocols.Pong, true);
        }

        private void PongCommand(NetworkMessage message)
        {
            if (idle != null)
                idle.Pong();
        }

        private void EndCommand(NetworkMessage message)
        {
            Terminate();
        }

        /*
        private void SetUsernameCommand(NetworkMessage message)
        {
            if (message.Arguments.Count == 1)
            {
                SetUsername?.Invoke(message.Arguments[0]);
            }
        }

        private void UsernameTakenCommand(NetworkMessage message)
        {
            UsernameTaken?.Invoke();
        }

        private void UsernameChangedCommand(NetworkMessage message)
        {
            if (message.Arguments.Count == 2)
            {
                UsernameChanged?.Invoke(message.Arguments[0], message.Arguments[1]);
            }
        }
        */
        #endregion
        #region outgoing events
        public delegate void NewMessageEventHandler(string message, string sender);
        public event NewMessageEventHandler NewMessage;

        public delegate void UserConnectedEventHandler(string user);
        public event UserConnectedEventHandler UserConnected;

        public delegate void TotalUsersEventHandler(IEnumerable<string> users);
        public event TotalUsersEventHandler TotalUsers;

        public delegate void UserDisconnectedEventHandler(string user);
        public event UserDisconnectedEventHandler UserDisconnected;
        
        public delegate void ConnectionConnectEventHandler();
        public event ConnectionConnectEventHandler ConnectionConnect;

        public delegate void SetUsernameEventHandler(string username);
        public event SetUsernameEventHandler SetUsername;

        public delegate void UsernameTakenEventHandler();
        public event UsernameTakenEventHandler UsernameTaken;

        public delegate void UsernameChangedEventHandler(string oldUsername, string changedUsername);
        public event UsernameChangedEventHandler UsernameChanged;

        public delegate void UserKickedEventHandler(string username);
        public event UserKickedEventHandler UserKicked;
        #endregion

        public void SendMessage(string message)
        {
            Send(MessageProtocols.Message, message);
        }

        protected override void EstablishConnection(NetworkMessage message)
        {
            try
            {
                if (message.TryGetObject<string>(out string iv))
                {
                    cipher = new SymmetricCipher();
                    string Key = asymCipher.Decrypt(message.Protocol);
                    string IV = asymCipher.Decrypt(iv);

                    cipher.Key = Key;
                    cipher.IV = IV;
                }
                else
                    throw new Exception();
            }
            catch (Exception)
            {
                Terminate();
            }
        }

        private void SetupConnection()
        {
            asymCipher = new AsymmetricCipher();
            Console.WriteLine("CLIENT sender: ");
            Console.WriteLine(asymCipher.PublicKey());
            Send(asymCipher.PublicKey(),"");
        }

        public override void Terminate()
        {
            Send(MessageProtocols.End, true);
            base.Terminate();
        }
    }
}