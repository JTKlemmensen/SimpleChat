using Client.Network;
using Shared.Network.Constants;
using SimpleChat.Gui.Connect;
using SimpleChat.Gui.Login;
using SimpleChat.Gui.Register;
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
using System.Windows.Shapes;

namespace SimpleChat.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowController
    {
        private SimpleClient client;

        public MainWindow()
        {
            InitializeComponent();
            GotoConnect();
        }

        public SimpleClient ChatClient => client;

        public void GotoLogin()
        {
            Content.Children.Clear();
            Content.Children.Add(new LoginControl(this));
        }

        public void GotoConnect()
        {
            Content.Children.Clear();
            Content.Children.Add(new ConnectControl(this));
        }

        public void GotoRegister()
        {
            Content.Children.Clear();
            Content.Children.Add(new RegisterControl(this));
        }

        public void GotoChat()
        {
            
        }

        public void TerminateConnection()
        {
            if (client != null)
                client.Terminate();
            client = null;
            GotoConnect();
        }

        public void Connect(string ip, int port)
        {
            client = new SimpleClient(ip, port);
            client.ConnectionDisconnect += OnSimpleClientDisconnected;
            client.ConnectionConnect += OnSimpleClientConnected;

            client.FailedAction += Client_FailedAction;
            client.LoginSuccess += Client_LoginSuccess;
        }

        private void Client_LoginSuccess(List<string> users)
        {
            MessageBox.Show("Successfully logged in! However, the chat is not fully implemented yet",
                              "Confirmation",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
        }

        private void Client_FailedAction(ResponseCodes code)
        {
            if(ResponseCodes.Bad_Login == code)
            {
                MessageBox.Show("Failed to log in",
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);
            }
        }

        private void OnSimpleClientDisconnected()
        {
            client = null;
            RunOnUIThread(()=> GotoConnect());
        }

        private void OnSimpleClientConnected()
        {
            RunOnUIThread(() => GotoLogin());
        }

        private void RunOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}