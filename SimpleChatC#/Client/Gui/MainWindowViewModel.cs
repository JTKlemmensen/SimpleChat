using Client;
using Client.Network;
using Shared.Gui;
using Shared.Network;
using Shared.Network.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleChat.Gui
{
    class MainWindowViewModel : ChangeNotifier
    {
        private SimpleClient _client;

        private bool _isConnected;
        private string _chatText;
        private ObservableCollection<string> _users;
        private string _username;

        private string _toggleConnectionButtonText;

        private string _serverIP;
        private bool _canChangeServerIP;
        private string _serverPort;
        private bool _canChangeServerPort;

        private string _messageToSend;

        public MainWindowViewModel()
        {
            // upgrading settings: https://stackoverflow.com/a/534335
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
            IsConnected = false;
            ToggleConnectionButtonText = "Connect";
            CanChangeServerIP = true;
            CanChangeServerPort = true;
            ChatText = "";
            Users = new ObservableCollection<string>();

            ServerIP = Properties.Settings.Default.LastUsedIP.ToString();
            ServerPort = Properties.Settings.Default.LastUsedPort.ToString();
        }

        public void Terminate()
        {
            _client?.Terminate();
            _client = null;
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; NotifyPropertyChanged(); }
        }

        public bool CanChangeServerIP
        {
            get { return _canChangeServerIP; }
            set { _canChangeServerIP = value; NotifyPropertyChanged(); }
        }

        public bool CanChangeServerPort
        {
            get { return _canChangeServerPort; }
            set { _canChangeServerPort = value; NotifyPropertyChanged(); }
        }

        public string ChatText
        {
            get { return _chatText; }
            set { _chatText = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<string> Users
        {
            get { return _users; }
            set { _users = value; NotifyPropertyChanged(); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged(); }
        }

        public string ToggleConnectionButtonText
        {
            get { return _toggleConnectionButtonText; }
            set { _toggleConnectionButtonText = value; NotifyPropertyChanged(); }
        }

        public string ServerIP
        {
            get { return _serverIP; }
            set { _serverIP = value; NotifyPropertyChanged(); }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; NotifyPropertyChanged(); }
        }

        public string MessageToSend
        {
            get { return _messageToSend; }
            set { _messageToSend = value; NotifyPropertyChanged(); }
        }

        public ICommand ToggleConnection
        {
            get { return new RelayCommand(ConnectOrDisconnect); }
        }

        private void RunOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        private void ConnectOrDisconnect()
        {
            if (IsConnected)
            {
                Terminate();
            }
            else
            {
                if (int.TryParse(ServerPort, out int port))
                {
                    Properties.Settings.Default.LastUsedIP = ServerIP;
                    Properties.Settings.Default.LastUsedPort = port;
                    Properties.Settings.Default.Save();
                    _client = new SimpleClient(ServerIP, port);
                    _client.NewMessage += ClientNewMessage;
                    _client.UserConnected += ClientUserConnected;
                    _client.TotalUsers += ClientTotalUsers;
                    _client.UserDisconnected += ClientUserDisconnected;

                    _client.SetUsername += ClientSetUsername;
                    _client.UsernameTaken += ClientUsernameTaken;
                    _client.UsernameChanged += ClientUsernameChanged;

                    _client.ConnectionConnect += ClientConnectedToServer;
                    _client.ConnectionDisconnect += ClientDisconnectedToServer;

                    _client.UserKicked += ClientUserKicked;
                }
            }
        }

        private void ClientUserKicked(string username)
        {
            if (username == Username)
            {
                Terminate();
                ChatText += "\n[You were kicked from the server]";
            }
        }

        private void ClientUsernameTaken()
        {
            // TODO:
        }

        private void ClientSetUsername(string username)
        {
            Username = username;
        }

        private void ClientUsernameChanged(string oldUsername, string changedUsername)
        {
            RunOnUIThread(() =>
            {
                Users.Remove(oldUsername);
                Users.Add(changedUsername);
            });
            ChatText += "\n[" + oldUsername + " changed their username to " + changedUsername + "]";
            SortUserList();
        }

        private void SortUserList()
        {
            RunOnUIThread(() =>
            {
                Users = new ObservableCollection<string>(Users.OrderBy(i => i));
            });
        }

        private void ClientConnectedToServer()
        {
            IsConnected = true;
            ToggleConnectionButtonText = "Disconnect";
            CanChangeServerIP = false;
            CanChangeServerPort = false;
            if (!string.IsNullOrWhiteSpace(ChatText))
            {
                ChatText += "\n";
            }
            ChatText += "[Connected to server]";
        }

        private void ClientDisconnectedToServer()
        {
            RunOnUIThread(() =>
            {
                Users.Clear();
            });
            IsConnected = false;
            ToggleConnectionButtonText = "Connect";
            CanChangeServerIP = true;
            CanChangeServerPort = true;
            ChatText += "\n[Disconnected from server]";
        }

        private void ClientUserConnected(string user)
        {
            ChatText += "\n" + user + " has connected";
            RunOnUIThread(() =>
            {
                Users.Add(user);
            });
        }

        private void ClientUserDisconnected(string user)
        {
            ChatText += "\n" + user + " has disconnected";
            RunOnUIThread(() =>
            {
                Users.Remove(user);
            });
        }

        private void ClientTotalUsers(IEnumerable<string> users)
        {
            RunOnUIThread(() =>
            {
                foreach (string user in users)
                {
                    Users.Add(user);
                }
            });
        }

        private void ClientNewMessage(string message, string sender)
        {
            ChatText += "\n[" + sender + "]: " + message;
        }

        public ICommand SendMessage
        {
            get { return new RelayCommand(SendMessageToServer); }
        }

        private void SendMessageToServer()
        {
            _client?.SendMessage(MessageToSend);
            ClearMessageCurrentlyBeingTyped();
        }

        public ICommand ClearMessage
        {
            get { return new RelayCommand(ClearMessageCurrentlyBeingTyped); }
        }

        private void ClearMessageCurrentlyBeingTyped()
        {
            MessageToSend = "";
        }

        public ICommand SetUsername
        {
            get { return new RelayCommand(SendChangedUsername); }
        }

        private void SendChangedUsername()
        {
            _client?.Send(MessageProtocols.SetUsername, Username);
        }
    }
}