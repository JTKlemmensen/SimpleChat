using SharedCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Server
{
    class MainWindowViewModel : ChangeNotifier
    {
        private SimpleServer _server;
        private bool _isServerStarted;

        private string _serverPort;
        private string _toggleServerButtonTitle;
        private bool _isServerPortFieldEnabled;
        private ObservableCollection<string> _users;

        private int _selectedUserIndex;
        private bool _isUserSelected;

        public MainWindowViewModel()
        {
            _isServerStarted = false;
            ToggleServerButtonTitle = "Start";
            IsServerPortFieldEnabled = true;
            Users = new ObservableCollection<string>();
            _selectedUserIndex = -1;
            _isUserSelected = false;
        }

        public void Terminate()
        {
            _server?.Terminate();
        }

        public string ToggleServerButtonTitle
        {
            get { return _toggleServerButtonTitle; }
            set { _toggleServerButtonTitle = value; NotifyPropertyChanged(); }
        }

        public bool IsServerPortFieldEnabled
        {
            get { return _isServerPortFieldEnabled; }
            set { _isServerPortFieldEnabled = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<string> Users
        {
            get { return _users; }
            set { _users = value; NotifyPropertyChanged(); }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; NotifyPropertyChanged(); }
        }

        public int SelectedUserIndex
        {
            get { return _selectedUserIndex; }
            set { _selectedUserIndex = value; NotifyPropertyChanged(); IsUserSelected = value >= 0; }
        }

        public bool IsUserSelected
        {
            get { return _isUserSelected; }
            set { _isUserSelected = value; NotifyPropertyChanged(); }
        }

        public ICommand ToggleServer
        {
            get { return new RelayCommand(ToggleServerOffOn); }
        }

        private void ToggleServerOffOn()
        {
            if (_isServerStarted)
            {
                _server.Terminate();
                _server = null;
            }
            else
            {
                if (int.TryParse(ServerPort, out int port))
                {
                    _server = new SimpleServer(port);
                    _server.UserConnected += ServerUserConnected;
                    _server.UserDisconnected += ClientUserDisconnected;
                    _server.ServerStarted += ServerStarted;
                    _server.ServerClosed += ServerClosed;
                    _server.UsernameChanged += ServerUsernameChanged;

                    _isServerStarted = true;
                }
            }
        }

        private void RunOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        private void ServerUsernameChanged(string oldUsername, string changedUsername)
        {
            RunOnUIThread(() =>
            {
                Users.Remove(oldUsername);
                Users.Add(changedUsername);
                SortUserList();
            });
        }

        private void ServerClosed()
        {
            ToggleServerButtonTitle = "Start Server";
            IsServerPortFieldEnabled = true;
        }

        private void ServerStarted()
        {
            ToggleServerButtonTitle = "Close Server Connections";
            IsServerPortFieldEnabled = false;
        }

        private void ClientUserDisconnected(string user)
        {
            RunOnUIThread(() =>
            {
                Users.Remove(user);
            });
        }

        private void ServerUserConnected(string user)
        {
            RunOnUIThread(() =>
            {
                Users.Add(user);
                SortUserList();
            });
        }

        private void SortUserList()
        {
            RunOnUIThread(() =>
            {
                Users = new ObservableCollection<string>(Users.OrderBy(i => i));
            });
        }

        public ICommand KickUser
        {
            get { return new RelayCommand(KickSelectedUser); }
        }

        private void KickSelectedUser()
        {
            _server.Broadcast(MessageProtocols.KickUser, _users[SelectedUserIndex]);
            _server.Broadcast(MessageProtocols.Disconnect, _users[SelectedUserIndex]);
        }
    }
}
