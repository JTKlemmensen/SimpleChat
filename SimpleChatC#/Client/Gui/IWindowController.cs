using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Gui
{
    public interface IWindowController
    {
        void GotoConnect();

        void GotoLogin();

        void GotoStartScreen();

        void GotoRegister();

        void GotoChat();
    }
}