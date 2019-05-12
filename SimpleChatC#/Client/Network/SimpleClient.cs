using Shared.Ciphers;
using Shared.Network;
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
        private int v;

        public SimpleClient(string ip, int port, string username = "")
        {
            try
            {
                Socket connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                connection.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                this.Start(connection);
                SetupConnection();

                idle = new IdleChecker(this);
            }
            catch (Exception){}
        }

        public SimpleClient(int v)
        {
            this.v = v;
        }

        protected override void OnCommand(NetworkMessage message)
        {
            if (!HasEstablishedConnection())
            {
                EstablishConnection(message);
                return;
            }

            switch(message.Protocol)
            {
                case MessageProtocols.Message:
                    if (message.TryGetObject<Message>(out Message m))
                        NewMessage?.Invoke(m.Content, m.Sender);
                    break;

                case MessageProtocols.Users:
                    if (message.TryGetObject<IEnumerable<string>>(out IEnumerable<string> users))
                    {
                        TotalUsers?.Invoke(users);
                        ConnectionConnect?.Invoke();
                    }
                    break;

                case MessageProtocols.Connect:
                    if (message.TryGetObject<string>(out string connectedUser))
                        UserConnected?.Invoke(connectedUser);
                    break;

                case MessageProtocols.Disconnect:
                    if (message.TryGetObject<string>(out string disconnectedUser))
                        UserDisconnected?.Invoke(disconnectedUser);
                    break;

                    /*
                case MessageProtocols.KickUser:
                    if (message.Arguments.Count() == 1)
                    {
                        UserKicked?.Invoke(message.Arguments[0]);
                    }
                    break;
                    */
                case MessageProtocols.Pong:
                    if (idle != null)
                        idle.Pong();
                    break;

                case MessageProtocols.Ping:
                    Send(MessageProtocols.Pong, true);
                    break;

                case MessageProtocols.End:
                    Terminate();
                    break;

                    /*
                case MessageProtocols.SetUsername:
                    if (message.Arguments.Count == 1)
                    {
                        SetUsername?.Invoke(message.Arguments[0]);
                    }
                    break;

                case MessageProtocols.UsernameTaken:
                    UsernameTaken?.Invoke();
                    break;

                case MessageProtocols.UsernameChanged:
                    if (message.Arguments.Count == 2)
                    {
                        UsernameChanged?.Invoke(message.Arguments[0], message.Arguments[1]);
                    }
                    break;
                    */
            }
        }

        public void SendMessage(string message)
        {
            Send(MessageProtocols.Message, message);
        }

        public delegate void OnNewMessage(string message, string sender);
        public event OnNewMessage NewMessage;

        public delegate void OnUserConnected(string user);
        public event OnUserConnected UserConnected;

        public delegate void OnTotalUsers(IEnumerable<string> users);
        public event OnTotalUsers TotalUsers;

        public delegate void OnUserDisconnected(string user);
        public event OnUserDisconnected UserDisconnected;
        
        public delegate void OnConnectionConnect();
        public event OnConnectionConnect ConnectionConnect;

        public delegate void OnSetUsername(string username);
        public event OnSetUsername SetUsername;

        public delegate void OnUsernameTaken();
        public event OnUsernameTaken UsernameTaken;

        public delegate void OnUsernameChanged(string oldUsername, string changedUsername);
        public event OnUsernameChanged UsernameChanged;

        public delegate void OnUserKicked(string username);
        public event OnUserKicked UserKicked;

        private bool HasEstablishedConnection()
        {
            return cipher != null;
        }

        private void EstablishConnection(NetworkMessage message)
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