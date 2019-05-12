using Newtonsoft.Json;
using Shared.Ciphers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Network
{
    public abstract class Connection
    {
        private Socket socket;
        private StreamWriter writer;
        protected SymmetricCipher cipher;
        public bool Stop { get; private set; }
        
        public void Start(Socket socket)
        {
            this.socket = socket;
            Thread t = new Thread(Run);
            t.Start();
        }

        private void Run()
        {
            try
            {
                if (socket == null)
                    return;

                NetworkStream stream = new NetworkStream(socket);
                stream.ReadTimeout = 500;
                using (StreamReader reader = new StreamReader(stream))
                using (writer = new StreamWriter(stream))
                {
                    while (!Stop)
                        try
                        {
                            string line = null;
                            if ((line = reader.ReadLine()) != null)
                                OnCommand(GetNetworkMessage(line));
                        }
                        catch (IOException)
                        {
                            // Read timeout
                        }
                }

            }catch(Exception){ }
            finally
            {
                ConnectionDisconnect?.Invoke();
            }
        }

        public delegate void OnConnectionDisconnect();
        public event OnConnectionDisconnect ConnectionDisconnect;

        public void Send(string protocol, object obj = null, bool encrypt = true)
        {
            if (writer == null || writer.BaseStream == null)
                return;

            string line = "";

            if (obj != null)
                line = JsonConvert.SerializeObject(obj);

            if (encrypt && cipher != null)
                line = NetworkUtil.InsertEscape(cipher.Encrypt(protocol)) + "," + NetworkUtil.InsertEscape(cipher.Encrypt(line));            
            else
                line = NetworkUtil.InsertEscape(protocol) + "," + NetworkUtil.InsertEscape(line);

            try
            {
                writer.WriteLine(line);
                writer.Flush();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Formats a message so its easy to work with. Decrypts the message if the cipher is not null.
        /// </summary>
        /// <param name="messageIn">The message that will be converted to an instance of NetworkMessage</param>
        /// <param name="cipher">The SymmetricCipher that will be used for decrypting the message</param>
        /// <returns></returns>
        private NetworkMessage GetNetworkMessage(string messageIn)
        {
            List<string> arguments = NetworkUtil.RemoveEscape(messageIn);
            Console.WriteLine();

            if (cipher != null)
                for (int i = 0; i < arguments.Count(); i++)
                {
                    arguments[i] = cipher.Decrypt(arguments[i]);
                    Console.Write(arguments[i] + " ");
                    Console.WriteLine();
                }

            if (arguments.Count() >= 2)
            {
                string protocol = arguments[0];
                string message = arguments[1];
                
                NetworkMessage networkMessage = new NetworkMessage()
                {
                    Protocol = protocol,
                    Message = message
                };

                return networkMessage;
            }

            return null;
        }

        public virtual void Terminate()
        {
            Stop = true;
        }

        protected abstract void OnCommand(NetworkMessage message);
    }
}