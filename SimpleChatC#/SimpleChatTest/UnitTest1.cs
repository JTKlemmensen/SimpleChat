using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;

namespace SimpleChatTest
{
    [TestClass]
    public class UnitTest1
    {
        private bool HasChanged;
        [TestMethod]
        public void TestMethod1()
        {
            HasChanged = false;
            SimpleServer server = new SimpleServer(25565);
            SimpleClient client = new SimpleClient(25565);
            client.NewMessage += Client_NewMessage;
            new SimpleClient(25565);
            Thread.Sleep(2000);

            Assert.AreEqual(true,HasChanged);
        }

        private void Client_NewMessage(string str, string sender)
        {
            HasChanged = true;
        }
    }
}
