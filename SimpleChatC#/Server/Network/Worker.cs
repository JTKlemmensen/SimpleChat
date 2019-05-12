using Server;
using Shared.Ciphers;
using Shared.Network;
using Shared.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Worker : Connection
    {
        public string Username { get; private set; }
        private SimpleServer server;
        private IdleChecker idle;

        public Worker(SimpleServer server, Socket connection, string username) : base()
        {
            this.AddCommand(MessageProtocols.Message, MessageCommand);
            this.AddCommand(MessageProtocols.End, EndCommand);
            this.AddCommand(MessageProtocols.Ping, PingCommand);
            this.AddCommand(MessageProtocols.Pong, PongCommand);
            
            //this.AddCommand(MessageProtocols.SetUsername, SetUsernameCommand);

            this.server = server;
            this.Username = username;
            this.Start(connection);
            idle = new IdleChecker(this);
        }
        #region incoming commands from client
        private void MessageCommand(NetworkMessage message)
        {
            if (message.TryGetObject<string>(out string m))
                server.SendMessage(m, Username);
        }

        private void EndCommand(NetworkMessage message)
        {
            Terminate();
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
        /*
        private void SetUsernameCommand(NetworkMessage message)
        {
            if (message.TryGetObject<string>(out string username))
            {

                var usernameToUse = username;
                if (server.IsUsernameTaken(usernameToUse))
                {
                    Send(MessageProtocols.UsernameTaken);
                }
                else
                {
                    var oldUsername = Username;
                    Username = usernameToUse;
                    server.UsernameWasChanged(oldUsername, Username);
                    server.Broadcast(MessageProtocols.UsernameChanged, oldUsername, Username);
                }
            }
        }
        */
        #endregion
       
        protected override void EstablishConnection(NetworkMessage message)
        {
            //Generate symmetric key + IV, save it and encrypt it with the given public assymetric key and send it to the client.   
            try
            {
                Console.WriteLine("Server modtod: ");
                Console.WriteLine(message.Protocol);
                AsymmetricCipher asymmetric = new AsymmetricCipher();
                asymmetric.LoadPublicKey(message.Protocol);   
                cipher = new SymmetricCipher();

                Send(asymmetric.Encrypt(cipher.Key), asymmetric.Encrypt(cipher.IV),false);
                Send(MessageProtocols.SetUsername, Username);
                server.Broadcast(MessageProtocols.Connect, this, Username); // Tell users that it connected
                Send(MessageProtocols.Users, server.ConnectedUsers().ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                //Terminate();
            }
        }

        public override void Terminate()
        {
            Send(MessageProtocols.End);
            server.Broadcast(MessageProtocols.Disconnect, this, Username);
            base.Terminate();
            server.Remove(this);
        }
    }
}