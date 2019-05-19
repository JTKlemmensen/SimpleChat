using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleChat.Gui.Chat
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        private IWindowController controller;

        public ChatControl(IWindowController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            controller.TerminateConnection();
        }

        private void Send(object sender, RoutedEventArgs e)
        {
            controller.ChatClient.SendMessage(Message.Text);
            Message.Clear();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Message.Clear();
        }
    }
}