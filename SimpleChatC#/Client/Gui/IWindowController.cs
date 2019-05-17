using Client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Gui
{
    public interface IWindowController
    {
        SimpleClient ChatClient { get; }

        void Connect(string ip, int port);

        void TerminateConnection();

        void GotoConnect();

        void GotoRegister();

        void GotoLogin();

        void GotoChat();
    }
}