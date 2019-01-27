using Client;
using Newtonsoft.Json;
using Shared;
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

namespace SimpleChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleClient client;
        public MainWindow()
        {
            InitializeComponent();
            client = new SimpleClient("127.0.0.1",25565);
            client.NewMessage += OnNewMessage;
        }

        private void OnNewMessage(string message, string sender)
        {
            Dispatcher.BeginInvoke(
                new Action(() => {
                    MessagesArea.Text += "\n["+sender+"]:"+message;
                })
            );
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            client.SendMessage(Message.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
